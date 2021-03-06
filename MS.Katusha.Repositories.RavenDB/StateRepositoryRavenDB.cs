using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using Raven.Client;
using Raven.Client.Linq;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Repositories.RavenDB
{
    public class StateRepositoryRavenDB : IStateRepositoryRavenDB
    {
        private readonly IDocumentStore _documentStore;

        public StateRepositoryRavenDB(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public State GetState(long profileId)
        {
            using (var session = _documentStore.OpenSession()) {
                return session.Query<State>().AsNoTracking().FirstOrDefault(p => p.ProfileId == profileId);
            }
        }

        public void Delete(State entity)
        {
            var name = String.Format("{0}s/{1}", typeof(State).Name.ToLower(CultureInfo.CreateSpecificCulture("en-US")), entity.Id);
            _documentStore.DatabaseCommands.Delete(name, null);
        }

        public IList<State> Query<TKey>(Expression<Func<State, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<State, TKey>> orderByClause, bool @ascending)
        {
            using (var session = _documentStore.OpenSession()) {
                RavenQueryStatistics stats;
                var q = session.Query<State>().Statistics(out stats).AsNoTracking();
                if (filter != null) q = q.Where(filter);
                if (orderByClause != null) q = (ascending) ? q.OrderBy(orderByClause) : q.OrderByDescending(orderByClause);
                var query  = q.Skip((pageNo - 1)*pageSize).Take(pageSize).ToList();
                total = stats.TotalResults;
                return query;
            }
        }

        public void Update(State state)
        {
            //var entity = new State {Id = profile.Id, ProfileId = profile.Id, Gender = profile.Gender, LastOnline = DateTime.Now};
            state.LastOnline = DateTime.Now;
            using (var session = _documentStore.OpenSession()) {
                session.Store(state);
                session.SaveChanges();
            }
        }

        public IList<T> Search<T>(Expression<Func<T, bool>> filter, int pageNo, int pageSize, out int total, Expression<Func<T, object>> orderByClause, bool ascending = false)
        {
            using (var session = _documentStore.OpenSession()) {
                RavenQueryStatistics stats;
                var q = session.Query<T>().Statistics(out stats).Where(filter);
                if (orderByClause != null) q = (ascending) ? q.OrderBy(orderByClause) : q.OrderByDescending(orderByClause);
                var query = Queryable.Skip(q, (pageNo - 1) * pageSize).Take(pageSize).ToList();
                total = stats.TotalResults;
                return query;
            }
        }


        public State GetState(Profile profile)
        {
            using (var session = _documentStore.OpenSession()) {
                return session.Query<State>().FirstOrDefault(p => p.ProfileGuid == profile.Guid);
            }
        }

    }
}