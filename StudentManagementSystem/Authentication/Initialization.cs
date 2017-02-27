namespace StudentManagementSystem.Authentication
{
    using System.Web;

    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using Microsoft.Practices.Unity;

    using StudentManagementSystem.Authentication.Managers;
    using StudentManagementSystem.Authentication.Models;
    using StudentManagementSystem.DependencyManager;

    public static class Initialization
    {
        public static void RegisterDependencies(IUnityContainer container)
        {
            container.RegisterFactory<ApplicationUserManager>(
                () => container.Resolve<HttpContextBase>().GetOwinContext().GetUserManager<ApplicationUserManager>(),
                UnityConfig.PerRequestLifetimeManager())
                .RegisterFactory<ApplicationSignInManager>(
                    () => container.Resolve<HttpContextBase>().GetOwinContext().Get<ApplicationSignInManager>(),
                    UnityConfig.PerRequestLifetimeManager())
                .RegisterFactory(
                    () => container.Resolve<HttpContextBase>().GetOwinContext().Authentication,
                    UnityConfig.PerRequestLifetimeManager());

        }
    }
}
