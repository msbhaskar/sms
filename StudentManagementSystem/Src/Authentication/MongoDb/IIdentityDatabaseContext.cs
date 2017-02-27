namespace StudentManagementSystem.Authentication.MongoDb
{
    using System;

    using MongoDB.Driver;

    public interface IIdentityDatabaseContext<TUser, TRole, TKey>
        where TRole : IdentityRole<TKey>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        IMongoCollection<TUser> UserCollection { get; set; }

        IMongoCollection<TRole> RoleCollection { get; set; }
    }
}