namespace StudentManagementSystem.DependencyManager
{
    using System;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            RegisterCommonMvcTypes(container);

            // container.RegisterFactory()
        }

        /// <summary>Registers a factory method that returns the requested <see cref="Type" /></summary>
        /// <typeparam name="TFrom">The <see cref="T:System.Type"/> requested. This is usually an interface.</typeparam>
        /// <param name="factory"> The factory method. </param>
        /// <returns>object that this method was called on</returns>
        public static IUnityContainer RegisterFactory<TFrom>(this IUnityContainer container, Func <TFrom> factory, LifetimeManager lifetimeManager = null)
        {
            container.RegisterType<TFrom>(lifetimeManager, new InjectionFactory(c => factory()));
            return container;
        }

        /// <summary>Create Lifetime manager for instance registration.</summary>
        /// <returns>The <see cref="LifetimeManager"/></returns>
        public static LifetimeManager ApplicationInstanceLifetimeManager()
        {
            return (LifetimeManager)new ContainerControlledLifetimeManager();
        }


        /// <summary>Creates a lifetime manager per request, this will use the HttpContext in IIS Hosted mode and Current Thread context in Unit test mode.</summary>
        /// <returns>The <see cref="LifetimeManager"/>.</returns>
        public static LifetimeManager PerRequestLifetimeManager()
        {
            return HostingEnvironment.IsHosted ? (LifetimeManager)new PerRequestLifetimeManager() : new PerThreadLifetimeManager();
        }



        private static void RegisterCommonMvcTypes(IUnityContainer container)
        {
            container.RegisterType<HttpContext>(new InjectionFactory(c => HttpContext.Current))
                .RegisterType<HttpContextBase>(new InjectionFactory(c => new HttpContextWrapper(HttpContext.Current)))
                .RegisterType<HttpRequestBase>(new InjectionFactory(c => c.Resolve<HttpContextBase>().Request))
                .RegisterType<HttpResponseBase>(new InjectionFactory(c => c.Resolve<HttpContextBase>().Response))
                .RegisterType<RouteCollection>(new InjectionFactory(c => RouteTable.Routes))
                .RegisterType<GlobalFilterCollection>(new InjectionFactory(c => GlobalFilters.Filters))
                .RegisterType<ViewEngineCollection>(new InjectionFactory(c => ViewEngines.Engines))
                .RegisterType<ControllerBuilder>(new InjectionFactory(c => ControllerBuilder.Current));
        }
    }
}
