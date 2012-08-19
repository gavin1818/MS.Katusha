﻿// This software is part of the Autofac IoC container
// Copyright © 2011 Autofac Contributors
// http://autofac.org
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Linq;

namespace Autofac.Integration.WebApi
{
    /// <summary>
    /// A filter provider for performing property injection on filter attributes.
    /// </summary>
    public class AutofacWebApiFilterProvider : IFilterProvider
    {
        class FilterContext
        {
            public ILifetimeScope LifetimeScope { get; set; }
            public Type ControllerType { get; set; }
            public List<FilterInfo> Filters { get; set; }
        }

        readonly ILifetimeScope _rootLifetimeScope;
        readonly ActionDescriptorFilterProvider _filterProvider = new ActionDescriptorFilterProvider();

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacWebApiFilterProvider"/> class.
        /// </summary>
        public AutofacWebApiFilterProvider(ILifetimeScope lifetimeScope)
        {
            _rootLifetimeScope = lifetimeScope;
        }

        /// <summary>
        /// Returns the collection of filters associated with <paramref name="actionDescriptor"/>.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>A collection of filters with instances property injected.</returns>
        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            var filters = _filterProvider.GetFilters(configuration, actionDescriptor).ToList();

            foreach (var filterInfo in filters)
                _rootLifetimeScope.InjectProperties(filterInfo.Instance);

            var descriptor = actionDescriptor as ReflectedHttpActionDescriptor;
            if (descriptor == null) return filters;

            // Use a fake scope to resolve the metadata for the filter.
            var rootLifetimeScope = configuration.DependencyResolver.GetRootLifetimeScope();
            using (var lifetimeScope = rootLifetimeScope.BeginLifetimeScope(AutofacWebApiDependencyResolver.ApiRequestTag))
            {
                var filterContext = new FilterContext
                {
                    LifetimeScope = lifetimeScope,
                    ControllerType = actionDescriptor.ControllerDescriptor.ControllerType,
                    Filters = filters
                };

                ResolveControllerScopedFilter<IAutofacActionFilter, ActionFilterWrapper>(
                    filterContext, m => new ActionFilterWrapper(m));
                ResolveControllerScopedFilter<IAutofacAuthorizationFilter, AuthorizationFilterWrapper>(
                    filterContext, m => new AuthorizationFilterWrapper(m));
                ResolveControllerScopedFilter<IAutofacExceptionFilter, ExceptionFilterWrapper>(
                    filterContext, m => new ExceptionFilterWrapper(m));

                ResolveActionScopedFilter<IAutofacActionFilter, ActionFilterWrapper>(
                    filterContext, descriptor.MethodInfo, m => new ActionFilterWrapper(m));
                ResolveActionScopedFilter<IAutofacAuthorizationFilter, AuthorizationFilterWrapper>(
                    filterContext, descriptor.MethodInfo, m => new AuthorizationFilterWrapper(m));
                ResolveActionScopedFilter<IAutofacExceptionFilter, ExceptionFilterWrapper>(
                    filterContext, descriptor.MethodInfo, m => new ExceptionFilterWrapper(m));
            }

            return filters;
        }

        static void ResolveControllerScopedFilter<TFilter, TWrapper>(
            FilterContext filterContext, Func<IFilterMetadata, TWrapper> wrapperFactory) 
            where TFilter : class 
            where TWrapper : IFilter
        {
            var filters = filterContext.LifetimeScope.Resolve<IEnumerable<Lazy<TFilter, IFilterMetadata>>>();
            foreach (var filter in filters)
            {
                var metadata = filter.Metadata;
                if (filterContext.ControllerType.IsAssignableFrom(metadata.ControllerType)
                    && metadata.FilterScope == FilterScope.Controller
                    && metadata.MethodInfo == null)
                {
                    var wrapper = wrapperFactory(filter.Metadata);
                    filterContext.Filters.Add(new FilterInfo(wrapper, filter.Metadata.FilterScope));
                }
            }
        }

        static void ResolveActionScopedFilter<TFilter, TWrapper>(
            FilterContext filterContext, MethodInfo methodInfo, Func<IFilterMetadata, TWrapper> wrapperFactory) 
                where TFilter : class 
                where TWrapper : IFilter
        {
            var filters = filterContext.LifetimeScope.Resolve<IEnumerable<Lazy<TFilter, IFilterMetadata>>>();
            foreach (var filter in filters)
            {
                var metadata = filter.Metadata;
                if (filterContext.ControllerType.IsAssignableFrom(metadata.ControllerType)
                    && metadata.FilterScope == FilterScope.Action
                    && metadata.MethodInfo.GetBaseDefinition() == methodInfo.GetBaseDefinition())
                {
                    var wrapper = wrapperFactory(filter.Metadata);
                    filterContext.Filters.Add(new FilterInfo(wrapper, filter.Metadata.FilterScope));
                }
            }
        }
    }
}