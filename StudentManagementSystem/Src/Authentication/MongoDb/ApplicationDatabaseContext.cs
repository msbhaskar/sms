namespace StudentManagementSystem.Authentication.MongoDb
{
    using StudentManagementSystem.Authentication.Models;

    public class ApplicationDatabaseContext : IdentityDatabaseContext<ApplicationUser, ApplicationUserRole, string>
    {
    }
}