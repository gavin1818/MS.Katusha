﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Base;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;

namespace MS.Katusha.Repositories.RavenDB
{
    public class ProfileRepositoryRavenDB : BaseFriendlyNameRepositoryRavenDB<Profile>, IProfileRepositoryRavenDB
    {
        public ProfileRepositoryRavenDB(IKatushaRavenStore documentStore)
            : base(documentStore)
        { }

        public IDictionary<string, IEnumerable<FacetValue>> FacetSearch<T>(Expression<Func<T, bool>> filter, string facetName)
        {
            using (var session = DocumentStore.OpenSession()) {
                return Queryable.Where(session.Query<T>(facetName + "Index"), filter).ToFacets("facets/" + facetName);
                //.AsProjection<Profile>()
            }
        }

        public IList<T> Search<T>(Expression<Func<T, bool>> filter, int pageNo, int pageSize, out int total)
        {
            using (var session = DocumentStore.OpenSession()) {
                RavenQueryStatistics stats;
                var query = Queryable.Skip(session.Query<T>().Statistics(out stats).Where(filter), (pageNo - 1)*pageSize).Take(pageSize).ToList();
                total = stats.TotalResults;
                return query;
            }
        }
    }
}