﻿using MS.Katusha.Domain;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly KatushaDbContext _dbContext;

        public ConfigurationService(IKatushaDbContext dbContext)
        {
            _dbContext = dbContext as KatushaDbContext;
        }

        public string[] ResetDatabaseResources()
        {
            ReloadResources.Delete(_dbContext);
            var result = ReloadResources.Set(_dbContext);
            var resourceManager = new ResourceManager();
            resourceManager.LoadConfigurationDataFromDb(new ConfigurationDataRepositoryDB(_dbContext));
            resourceManager.LoadResourceFromDb(new ResourceRepositoryDB(_dbContext));
            resourceManager.LoadResourceLookupFromDb(new ResourceLookupRepositoryDB(_dbContext));
            return result;
            
        }
    }

}