[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(StudentManagementSystem.Web.UnityWebActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(StudentManagementSystem.Web.UnityWebActivator), "Shutdown")]
namespace StudentManagementSystem.Web
{
    using System.Linq;
    using System.Web.Mvc;

    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Mvc;

    using StudentManagementSystem.Shared;

    /// <summary>Provides the bootstrapping for integrating Unity with ASP.NET MVC.</summary>
    public static class UnityWebActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start()
        {
            var container = UnityConfig.GetConfiguredContainer();

            InitializeDependencies(container);

            FilterProviders.Providers.Remove(Enumerable.OfType<FilterAttributeFilterProvider>(FilterProviders.Providers).First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(UnityPerRequestHttpModule));
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }

        private static void InitializeDependencies(IUnityContainer container)
        {
            Authentication.Initialization.RegisterDependencies(container);
        }
    }
}