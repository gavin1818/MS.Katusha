using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using MS.Katusha.Domain;
using MS.Katusha.Infrastructure;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Repositories.RavenDB;
using MS.Katusha.Services;
using NLog;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Database.Server;

namespace MS.Katusha.Web.Helpers
{
    public static class DependencyHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static DocumentStore RavenStore;
        public static string RootFolder = HttpContext.Current.Server.MapPath(@"~\");

        public static void RegisterRaven()
        {
            //RavenStore = new EmbeddableDocumentStore { DataDirectory = ConfigurationManager.AppSettings["Root_Folder"] + @"App_Data\MS.Katusha.RavenDB", UseEmbeddedHttpServer = true };
            //RavenStore = new EmbeddableDocumentStore { DataDirectory = Environment.GetEnvironmentVariable("MS.KATUSHA_HOME") + @"App_Data\MS.Katusha.RavenDB", UseEmbeddedHttpServer = true };
            RavenStore = new EmbeddableDocumentStore { DataDirectory = RootFolder + @"App_Data\MS.Katusha.RavenDB", UseEmbeddedHttpServer = true };
            RavenStore.Initialize();
            //IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), RavenStore);

            //// Start the HTTP server manually
            //var server = new RavenDbHttpServer(RavenStore.Configuration, RavenStore.DocumentDatabase);
            //server.Start();
            //NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080);
        }


        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<ResourceManager>().As<IResourceManager>().SingleInstance();

            builder.RegisterType<UserService>().As<IUserService>().InstancePerHttpRequest();
            builder.RegisterType<ProfileService>().As<IProfileService>().InstancePerHttpRequest();
            builder.RegisterType<SearchService>().As<ISearchService>().InstancePerHttpRequest();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().InstancePerHttpRequest();

            builder.RegisterType<KatushaMemoryCacheContext>().As<IKatushaCacheContext>().InstancePerHttpRequest();
            //builder.RegisterType<KatushaRavenCacheContext>().As<IKatushaCacheContext>().InstancePerHttpRequest();
            builder.RegisterType<CacheObjectRepositoryRavenDB>().WithParameter(new TypedParameter(typeof(IDocumentStore), RavenStore)).As<IRepository<CacheObject>>().InstancePerHttpRequest();
            builder.RegisterType<ProfileRepositoryRavenDB>().WithParameter(new TypedParameter(typeof(IDocumentStore), RavenStore)).As<IProfileRepositoryRavenDB>().InstancePerHttpRequest();
            
            builder.RegisterType<ConversationRepositoryDB>().As<IConversationRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<UserRepositoryDB>().As<IUserRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<ProfileRepositoryDB>().As<IProfileRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<CountriesToVisitRepositoryDB>().As<ICountriesToVisitRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<SearchingForRepositoryDB>().As<ISearchingForRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<LanguagesSpokenRepositoryDB>().As<ILanguagesSpokenRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<PhotoRepositoryDB>().As<IPhotoRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<VisitRepositoryDB>().As<IVisitRepositoryDB>().InstancePerHttpRequest();

            builder.RegisterType<KatushaDbContext>().As<IKatushaDbContext>().InstancePerHttpRequest();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            
#if DEBUG
            logger.Info("Dependencies resolved");
#endif
        }
    }
}