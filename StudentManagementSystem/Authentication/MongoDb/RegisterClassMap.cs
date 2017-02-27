namespace StudentManagementSystem.Authentication.MongoDb
{
    using System;

    using MongoDB.Bson.Serialization.Conventions;

    public class RegisterClassMap<TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private static RegisterClassMap<TUser, TRole, TKey> _thisClassMap;

        public static void Init()
        {
            if (_thisClassMap == null)
            {
                _thisClassMap = new RegisterClassMap<TUser, TRole, TKey>();
            }

            _thisClassMap.Configure();
        }


        public virtual void Configure()
        {
            this.RegisterConventions();

            this.RegisterRoleClassMap();
            this.RegisterUserClassMap();
        }

        public virtual void RegisterConventions()
        {
            var conv = new ConventionPack();
            conv.Add(new IgnoreIfDefaultConvention(true));
            conv.Add(new IgnoreExtraElementsConvention(true));

            // apply these conventions to StudentManagementSystem.Authentication.MongoDb and items that inherit it
            ConventionRegistry.Register("StudentManagementSystem.Authentication.MongoDb", conv, t => t.Namespace.StartsWith(typeof(IdentityRole<TKey>).Namespace) || (t.BaseType != null && t.BaseType.Namespace.StartsWith(typeof(IdentityRole<TKey>).Namespace)));
        }

        public virtual void RegisterRoleClassMap() { }

        public virtual void RegisterUserClassMap() { }
    }
}