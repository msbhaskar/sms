namespace StudentManagementSystem.Shared
{
    using System.Configuration;

    using Microsoft.Practices.Unity;

    public static class Application
    {
        public static string ApplicationName { get; }

        public static string DatabaseConnection { get; }

        public static string DatabaseName { get; }

        public static IUnityContainer UnityContainer { get; }

        static Application()
        {
            ApplicationName = ConfigurationManager.AppSettings["AppName"];
            DatabaseConnection = ConfigurationManager.AppSettings["DatabaseConnectionString"];
            DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
        }
    }
}
