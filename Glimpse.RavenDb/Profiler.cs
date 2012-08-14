﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Glimpse.Core.Extensibility;
using Newtonsoft.Json.Linq;
using Raven.Client.Connection;
using Raven.Client.Connection.Profiling;
using Raven.Client.Document;
using Raven.Json.Linq;

namespace Glimpse.RavenDb
{
	[GlimpsePlugin(ShouldSetupInInit = true)]
	public class Profiler : IGlimpsePlugin
	{
		private const string GlimpseTimerCategory = "RavenDb";

		public string Name { get { return "RavenDb"; } }

		public void SetupInit() {
			var key = ConfigurationManager.AppSettings["Glimpse.RavenDb.DocumentStoreApplicationKey"];
			if (!String.IsNullOrEmpty(key)) {
				var store = HttpContext.Current.Application[key] as DocumentStore;
				if (store != null)
					AttachTo(store);
			}

			var fields = ConfigurationManager.AppSettings["Glimpse.RavenDb.HiddenFields"];
			if (!String.IsNullOrEmpty(fields)) {
				HideFields(fields.Split(','));
			}
		}

		public object GetData(HttpContextBase context) {
			var data = new List<object[]>();
			data.Add(new object[] { "Key", "Value" });
			data.Add(new object[] { "Stores", GetStoreList() });
			data.Add(new object[] { "Sessions", GetSessionList() });
			data.Add(new object[] { "Requests", GetRequestList() });
			return data;
		}

		private List<object[]> GetStoreList() {
			List<object[]> data = new List<object[]>();
			data.Add(new object[] { "Url", "Database", "Conn. String Name", "Identity Separator", "Max Requests Per Session", "Embedded?" });
			data.AddRange(stores.Keys.Select(store => new object[] {
				store.Url,
				store.DefaultDatabase,
				store.ConnectionStringName,
				store.Conventions.IdentityPartsSeparator,
				store.Conventions.MaxNumberOfRequestsPerSession,
				IsEmbedded(store)
			}));
			return data;
		}

		private List<object[]> GetSessionList() {
			List<object[]> data = new List<object[]>();
			if (stores.Keys.All(d => IsEmbedded(d))) {
				// All the profiled stores are embedded and do not support profiling
				data.Add(new object[] { "Message" });
				data.Add(new object[] { "Profiling is currently not supported for EmbeddableDocumentStore." });
			} else {
				data.Add(new object[] { "Session Id", "Request Count", "At", "Duration" });

				var sessions = from id in ContextualSessionList
							   from store in stores.Keys
							   let info = store.GetProfilingInformationFor(id)
							   where info != null
							   select info;
				data.AddRange(sessions.Select(session => new object[] {
					session.Id,
					session.Requests.Count,
					session.At,
					session.Requests.Sum(d => d.DurationMilliseconds)
				}));
			}
			return data;
		}

		private List<object[]> GetRequestList() {
			List<object[]> data = new List<object[]>();
			data.Add(new object[] { "At", "Duration", "HttpMetod", "Url", "Data", "HttpResult", "Status", "Result" });
			var requests = from id in ContextualSessionList
						   from store in stores.Keys
						   let info = store.GetProfilingInformationFor(id)
						   where info != null
						   from request in info.Requests
						   select request;
			data.AddRange(requests.Select(req => new object[] {
		        req.At,
				req.DurationMilliseconds,
		        req.Method,
		        HttpUtility.UrlDecode(HttpUtility.UrlDecode(req.Url)),
		        ParseJsonResult(req.PostedData),
		        req.HttpResult,
		        req.Status.ToString(),
		        ParseJsonResult(req.Result)
		    }));
			return data;
		}

		public static object ParseJsonResult(string json)
		{
		    return json;
            //try {
            //    var token = RavenJToken.Parse(json);
            //    return Visit(token);
            //} catch {
            //    return json;
            //}
		}

		private static object Visit(RavenJToken token) {
			switch (token.Type) {
			case JTokenType.Object:
				List<object[]> data = new List<object[]>();
				data.Add(new object[] { "Key", "Value" });
				var obj = (RavenJObject)token;
				foreach (var child in obj) {
					if (!jsonKeysToHide.Contains(child.Key)) {
						data.Add(new object[] { child.Key, Visit(child.Value) });
					}
				}
				return data;
			case JTokenType.Array:
				var arr = (RavenJArray)token;
				if (arr.Length == 0)
					return null;
				List<object[]> arrayItems = new List<object[]>();
				for (int i = 0; i < arr.Length; i++) {
					arrayItems.Add(new object[] { Visit(arr[i]) });
				}

				if (arr[0].Type == JTokenType.Object) {
					// Handle objects in a special way by pivoting them
					List<object[]> pivotData = new List<object[]>();
					var keys = arrayItems.SelectMany(d => (List<object[]>)d[0]).Cast<object[]>().Where(d => (string)d[0] != "Key" || (string)d[1] != "Value").Select(d => (string)d[0]).Distinct().ToArray();
					pivotData.Add(keys);
					foreach (var row in arrayItems.Select(d => d[0]).Cast<List<object[]>>()) {
						object[] vals = new object[keys.Length];
						for (int i = 0; i < keys.Length; i++) {
							var keyVal = row.FirstOrDefault(d => (string)d[0] == keys[i]);
							if (keyVal != null) {
								vals[i] = keyVal[1];
							}
						}
						pivotData.Add(vals);
					}

					return pivotData;
				} else {
					arrayItems.Insert(0, new[] { "Values" });
					return arrayItems;
				}
			case JTokenType.String:
				return ParseJsonResult((string)((RavenJValue)token).Value);
			case JTokenType.Boolean:
			case JTokenType.Float:
			case JTokenType.Integer:
			case JTokenType.Bytes:
			case JTokenType.Date:
				return ((RavenJValue)token).Value;
			default:
				return null;
			}
		}

		private static ConcurrentDictionary<DocumentStore, object> stores = new ConcurrentDictionary<DocumentStore, object>();
		private static List<string> jsonKeysToHide = new List<string>();

		/// <summary>
		/// Attach a DocumentStore instance to the profiler
		/// </summary>
		/// <param name="store">The instance to profile</param>
		public static void AttachTo(DocumentStore store) {
			GlimpseTimer.Moment("Document Store Created", GlimpseTimerCategory);
			store.SessionCreatedInternal += TrackSession;
			store.AfterDispose += StopTrackingStore;
			store.JsonRequestFactory.ConfigureRequest += BeginRequest;
			store.JsonRequestFactory.LogRequest += EndRequest;
			stores.TryAdd(store, null);
		}

		/// <summary>
		/// Set a list of fields to be hidden in your json result
		/// </summary>
		/// <param name="keys">The field names that will be hidden</param>
		public static void HideFields(params string[] keys) {
			jsonKeysToHide.AddRange(keys);
		}

		private static void StopTrackingStore(object sender, EventArgs e) {
			object _;
			GlimpseTimer.Moment("Document Store Disposed", GlimpseTimerCategory);
			stores.TryRemove(sender as DocumentStore, out _);
		}

		private static void TrackSession(InMemoryDocumentSessionOperations session) {
			GlimpseTimer.Moment("Session Created", GlimpseTimerCategory);
			ContextualSessionList.Add(session.Id);
		}

		private static void BeginRequest(object sender, WebRequestEventArgs e) {
			GlimpseTimer.Start("Query - " + e.Request.RequestUri.PathAndQuery, GlimpseTimerCategory);
			//GlimpseTimer.Moment();
		}

		private static void EndRequest(object sender, RequestResultArgs e) {
			GlimpseTimer.Stop("Query - " + e.Url);
		}

		private static List<Guid> ContextualSessionList {
			get {
				const string key = "Glimpse.RavenDb.SessionList";
				if (HttpContext.Current == null)
					return new List<Guid>();
				if (!HttpContext.Current.Items.Contains(key))
					HttpContext.Current.Items.Add(key, new List<Guid>());
				return HttpContext.Current.Items[key] as List<Guid>;
			}
		}

		private bool IsEmbedded(DocumentStore store) {
			return store.GetType().Name == "EmbeddableDocumentStore";
		}
	}
}