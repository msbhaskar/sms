namespace StudentManagementSystem.Authentication.MongoDb
{
    using System;
    using System.Linq;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public class IdentityDatabaseContext<TUser, TRole, TKey> : IIdentityDatabaseContext<TUser, TRole, TKey>, IDisposable
        where TRole : IdentityRole<TKey>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; } = "StudentManagementSystemDb";
        public string UserCollectionName { get; set; } = "Users";
        public string RoleCollectionName { get; set; } = "Roles";

        /// <summary>
        /// When the <see cref="UserCollection"/> and <see cref="RoleCollection"/> collections are initially fetched - ensure the indexes are created (default: false)
        /// </summary>
        public bool EnsureCollectionIndexes { get; set; } = false;

        public MongoCollectionSettings CollectionSettings { get; set; } = new MongoCollectionSettings { WriteConcern = WriteConcern.WMajority };
        public CreateCollectionOptions CreateCollectionOptions { get; set; } = new CreateCollectionOptions { AutoIndexId = true };
        public CreateIndexOptions CreateIndexOptions { get; set; } = new CreateIndexOptions { Background = true, Sparse = true };


        public virtual IMongoClient Client
        {
            get
            {
                if (this._client == null)
                {
                    if (this._database != null)
                    {
                        this._client = this._database.Client;
                    }

                    if (string.IsNullOrWhiteSpace(this.ConnectionString))
                    {
                        throw new NullReferenceException($"The parameter '{nameof(this.ConnectionString)}' in '{typeof(IdentityDatabaseContext<TUser, TRole, TKey>).FullName}' is null and must be set before calling '{nameof(this.Client)}'. This is usually configured as part of Startup.cs");
                    }
                    this._client = new MongoClient(this.ConnectionString);
                }
                return this._client;
            }
            set { this._client = value; }
        }
        private IMongoClient _client;

        public virtual IMongoDatabase Database
        {
            get
            {
                if (this._database == null)
                {
                    if (string.IsNullOrWhiteSpace(this.DatabaseName))
                    {
                        throw new NullReferenceException($"The parameter '{nameof(this.DatabaseName)}' in '{typeof(IdentityDatabaseContext<TUser, TRole, TKey>).FullName}' is null and must be set before calling '{nameof(this.Database)}'. This is usually configured as part of Startup.cs");
                    }
                    this._database = this.Client.GetDatabase(this.DatabaseName);
                }
                return this._database;
            }
            set { this._database = value; }
        }
        private IMongoDatabase _database;


        /// <summary>
        /// Initiates the user collection. If <see cref="EnsureCollectionIndexes"/> == true, will call <see cref="EnsureUserIndexesCreated"/>
        /// </summary>
        public virtual IMongoCollection<TUser> UserCollection
        {
            get
            {
                if (this._userCollection == null)
                {
                    if (string.IsNullOrWhiteSpace(this.UserCollectionName))
                    {
                        throw new NullReferenceException($"The parameter '{nameof(this.UserCollectionName)}' in '{typeof(IdentityDatabaseContext<TUser, TRole, TKey>).FullName}' is null and must be set before calling '{nameof(this.UserCollection)}'. This is usually configured as part of Startup.cs");
                    }
                    if (this.EnsureCollectionIndexes)
                    {
                        this.EnsureUserIndexesCreated();
                    }

                    this._userCollection = this.Database.GetCollection<TUser>(this.UserCollectionName, this.CollectionSettings);
                }
                return this._userCollection;
            }
            set { this._userCollection = value; }
        }
        private IMongoCollection<TUser> _userCollection;


        /// <summary>
        /// Initiates the role collection. If <see cref="EnsureCollectionIndexes"/> == true, will call <see cref="EnsureRoleIndexesCreated"/>
        /// </summary>
        public virtual IMongoCollection<TRole> RoleCollection
        {
            get
            {
                if (this._roleCollection == null)
                {
                    if (string.IsNullOrWhiteSpace(this.RoleCollectionName))
                    {
                        throw new NullReferenceException($"The parameter '{nameof(this.RoleCollectionName)}' in '{typeof(IdentityDatabaseContext<TUser, TRole, TKey>).FullName}' is null and must be set before calling '{nameof(this.RoleCollection)}'. This is usually configured as part of Startup.cs");
                    }
                    if (this.EnsureCollectionIndexes)
                    {
                        this.EnsureRoleIndexesCreated();
                    }
                    this._roleCollection = this.Database.GetCollection<TRole>(this.RoleCollectionName, this.CollectionSettings);
                }
                return this._roleCollection;
            }
            set { this._roleCollection = value; }
        }
        private IMongoCollection<TRole> _roleCollection;

        /// <summary>
        /// Ensures the user collection is instantiated, and has the standard indexes applied. Called when <see cref="UserCollection"/> is called if <see cref="EnsureCollectionIndexes"/> == true.
        /// </summary>
        /// <remarks>
        /// Indexes Created:
        /// Users: NormalizedUserName, NormalizedEmail, Logins.LoginProvider, Roles.NormalizedName, Claims.ClaimType + Roles.Claims.ClaimType
        /// </remarks>
        public virtual void EnsureUserIndexesCreated()
        {
            // only check on app startup
            if (_doneUserIndexes) return;
            _doneUserIndexes = true;

            // ensure collection exists
            if (!this.CollectionExists(this.UserCollectionName))
            {
                this.Database.CreateCollectionAsync(this.UserCollectionName, this.CreateCollectionOptions).Wait();
            }

            // ensure NormalizedUserName index exists
            var normalizedNameIndex = Builders<TUser>.IndexKeys.Ascending(x => x.NormalizedUserName);
            this.UserCollection.Indexes.CreateOneAsync(normalizedNameIndex, this.CreateIndexOptions);

            // ensure NormalizedEmail index exists
            var normalizedEmailIndex = Builders<TUser>.IndexKeys.Ascending(x => x.NormalizedEmail);
            this.UserCollection.Indexes.CreateOneAsync(normalizedEmailIndex, this.CreateIndexOptions);

            // ensure Roles.NormalizedName index exists
            var roleNameIndex = Builders<TUser>.IndexKeys.Ascending("Roles_NormalizedName");
            this.UserCollection.Indexes.CreateOneAsync(roleNameIndex, this.CreateIndexOptions);

            // ensure LoginProvider index exists
            var loginProviderIndex = Builders<TUser>.IndexKeys.Ascending("Logins_LoginProvider");
            this.UserCollection.Indexes.CreateOneAsync(loginProviderIndex, this.CreateIndexOptions);

            // ensure claims index exists
            var claimsProviderIndex = Builders<TUser>.IndexKeys.Ascending("AllClaims_ClaimType");
            this.UserCollection.Indexes.CreateOneAsync(claimsProviderIndex, this.CreateIndexOptions);
        }

        /// <summary>
        /// Singleton property. Only want to check collection indexes are done once, not on every call
        /// </summary>
        protected static bool _doneUserIndexes = false;
        protected static bool _doneRoleIndexes = false;

        /// <summary>
        /// Ensures the role collection is instantiated, and has the standard indexes applied. Called when <see cref="RoleCollection"/> is called if <see cref="EnsureCollectionIndexes"/> == true.
        /// </summary>
        /// <remarks>
        /// Indexes Created:
        /// Roles: NormalizedName
        /// </remarks>
        public virtual void EnsureRoleIndexesCreated()
        {
            // only check on app startup
            if (_doneRoleIndexes) return;
            _doneRoleIndexes = true;

            // ensure collection exists
            if (!this.CollectionExists(this.RoleCollectionName))
            {
                this.Database.CreateCollectionAsync(this.RoleCollectionName, this.CreateCollectionOptions).Wait();
            }

            // ensure NormalizedName index exists
            var normalizedNameIndex = Builders<TRole>.IndexKeys.Ascending(x => x.NormalizedName);
            this.RoleCollection.Indexes.CreateOneAsync(normalizedNameIndex, this.CreateIndexOptions);
        }

        /// <summary>
        /// WARNING: Permanently deletes user collection, including all indexes and data.
        /// </summary>
        public virtual void DeleteUserCollection()
        {
            this.Database.DropCollectionAsync(this.UserCollectionName).Wait();
            _doneUserIndexes = false;
        }

        /// <summary>
        /// WARNING: Permanently deletes role collection, including all indexes and data.
        /// </summary>
        public virtual void DeleteRoleCollection()
        {
            this.Database.DropCollectionAsync(this.RoleCollectionName).Wait();
            _doneRoleIndexes = false;
        }

        /// <summary>
        /// check if the collection already exists
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        protected virtual bool CollectionExists(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var cursorTask = this.Database.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            cursorTask.Wait();

            var cursor = cursorTask.Result.ToListAsync();
            cursor.Wait();

            return cursor.Result.Any();
        }

        public void Dispose()
        {
        }
    }
}