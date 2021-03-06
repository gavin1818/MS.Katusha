﻿using System.Linq;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Base;
using MS.Katusha.Repositories.DB.Context;

namespace MS.Katusha.Repositories.DB
{
    public class ResourceLookupRepositoryDB : BaseRepositoryDB<ResourceLookup>, IResourceLookupRepository
    {

        public ResourceLookupRepositoryDB(IKatushaDbContext context) : base(context)
        {
        }

        public ResourceLookup[] GetActiveValues()
        {
            return DbContext.Set<ResourceLookup>().Where(r => !r.Deleted).OrderBy(r=>r.LookupName).ThenBy(r=>r.Language).ToArray();
        }
    }

}
