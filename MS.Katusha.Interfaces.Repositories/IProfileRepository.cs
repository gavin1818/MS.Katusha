﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using Raven.Abstractions.Data;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IProfileRepository : IFriendlyNameRepository<Profile>
    {
    }

    public interface IProfileRepositoryDB : IProfileRepository
    {
    }

    public interface IProfileRepositoryRavenDB : IProfileRepository
    {
        IList<Profile> Search(Expression<Func<Profile, bool>> filter, int pageNo, int pageSize, out int total);
        IDictionary<string, IEnumerable<FacetValue>> FacetSearch(Expression<Func<Profile, bool>> filter);
        void SetFaceting();
    }
}