using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using MS.Katusha.DependencyManagement;
using MS.Katusha.Web.Helpers.Binders;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web
{
    public static class DependencyConfig
    {
        public const int GlobalPageSize = 40;
        private static IContainer _container;

        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            DependencyRegistrar.Build(builder);
            _container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));

            ModelBinders.Binders[typeof(SearchProfileCriteriaModel)] = new SearchCriteriaBinder();
            ModelBinders.Binders[typeof(SearchStateCriteriaModel)] = new SearchCriteriaBinder();
            ModelBinders.Binders[typeof(ProfileModel)] = new ProfileModelBinder();
            ModelBinders.Binders[typeof(RegisterModel)] = new ProfileModelBinder();
            ModelBinders.Binders[typeof(FacebookProfileModel)] = new FacebookProfileModelBinder();
        }

    }
}