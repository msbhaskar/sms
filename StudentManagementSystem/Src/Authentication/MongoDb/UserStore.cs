namespace StudentManagementSystem.Authentication.MongoDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity;

    using MongoDB.Bson;
    using MongoDB.Driver;

    internal class UserStore<TUser, TRole, TKey>
        : IUserLoginStore<TUser, TKey>,
        IUserRoleStore<TUser, TKey>,
        IUserClaimStore<TUser, TKey>,
        IUserPasswordStore<TUser, TKey>,
        IUserSecurityStampStore<TUser, TKey>,
        IUserEmailStore<TUser, TKey>,
        IUserLockoutStore<TUser, TKey>,
        IUserPhoneNumberStore<TUser, TKey>,
        IQueryableUserStore<TUser, TKey>,
        IUserTwoFactorStore<TUser, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        public UserStore(IIdentityDatabaseContext<TUser, TRole, TKey> databaseContext)
        {
            if (databaseContext == null) throw new ArgumentNullException(nameof(databaseContext));

            DatabaseContext = databaseContext; ;
        }

        protected IIdentityDatabaseContext<TUser, TRole, TKey> DatabaseContext { get; set; }


        #region IUserStore<TUser> (base inteface for the other interfaces)

        /// <summary>
        /// Gets the user identifier for the specified <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose identifier should be retrieved.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the identifier for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetUserIdAsync(TUser user)
        {

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(ConvertIdToString(user.Id));
        }

        /// <summary>
        /// Gets the user name for the specified <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose name should be retrieved.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the name for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetUserNameAsync(TUser user)
        {

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        /// <summary>
        /// Sets the given <paramref name="userName" /> for the specified <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose name should be set.</param>
        /// <param name="userName">The user name to set.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetUserNameAsync(TUser user, string userName)
        {

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.UserName = userName;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the normalized user name for the specified <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose normalized name should be retrieved.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the normalized user name for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetNormalizedUserNameAsync(TUser user)
        {

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedUserName ?? Normalize(user.UserName));
        }

        /// <summary>
        /// Sets the given normalized name for the specified <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose name should be set.</param>
        /// <param name="normalizedUserName">The normalized name to set.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetNormalizedUserNameAsync(TUser user, string normalizedUserName)
        {

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.NormalizedUserName = Normalize(normalizedUserName);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Creates the specified <paramref name="user"/> in the user store, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to create.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.</returns>
        public virtual async Task<IdentityResult> CreateAsync(TUser user)
        {

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (await UserDetailsAlreadyExists(user)) return IdentityResult.Failed("DuplicateUserName: " +user);

            try
            {
                ConfigureDefaults(user);
                user.CreatedDate = DateTime.UtcNow;
                await DatabaseContext.UserCollection.InsertOneAsync(user);
            }
            catch (MongoWriteException)
            {
                return IdentityResult.Failed("DuplicateUserName"  + user);
            }

            return IdentityResult.Success;
        }

        Task IUserStore<TUser, TKey>.UpdateAsync(TUser user)
        {
            return this.UpdateAsync(user);
        }

        Task IUserStore<TUser, TKey>.DeleteAsync(TUser user)
        {
            return this.DeleteAsync(user);
        }

        Task IUserStore<TUser, TKey>.CreateAsync(TUser user)
        {
            return this.CreateAsync(user);
        }

        /// <summary>
        /// Updates the specified <paramref name="user"/> in the user store, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to update.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
        public virtual async Task<IdentityResult> UpdateAsync(TUser user)
        {

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (await this.UserDetailsAlreadyExists(user))
            {
                return IdentityResult.Failed("DuplicateUserName" + user);
            }

            ConfigureDefaults(user);

            var filter = Builders<TUser>.Filter.Eq(x => x.Id, user.Id);
            var updateOptions = new UpdateOptions { IsUpsert = true };
            await DatabaseContext.UserCollection.ReplaceOneAsync(filter, user, updateOptions);

            return IdentityResult.Success;
        }

        /// <summary>
        /// Deletes the specified <paramref name="user"/> from the user store, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to delete.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
        public virtual async Task<IdentityResult> DeleteAsync(TUser user)
        {

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            var filter = Builders<TUser>.Filter.Eq(x => x.Id, user.Id);
            await DatabaseContext.UserCollection.DeleteOneAsync(filter);

            return IdentityResult.Success;
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>

        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userID"/> if it exists.
        /// </returns>
        public virtual Task<TUser> FindByIdAsync(TKey userId)
        {
            ThrowIfDisposed();
            if (userId == null) return Task.FromResult((TUser)null);

            var filter = Builders<TUser>.Filter.Eq(x => x.Id, userId);
            var options = new FindOptions { AllowPartialResults = false };

            return DatabaseContext.UserCollection.Find(filter, options).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified normalized user name.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>

        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userID"/> if it exists.
        /// </returns>
        public virtual Task<TUser> FindByNameAsync(string normalizedUserName)
        {

            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(normalizedUserName)) return Task.FromResult((TUser)null);

            var filter = Builders<TUser>.Filter.Eq(x => x.NormalizedUserName, Normalize(normalizedUserName));
            var options = new FindOptions { AllowPartialResults = false };

            return DatabaseContext.UserCollection.Find(filter, options).SingleOrDefaultAsync();
        }

        #endregion

        #region IUserLoginStore<TUser>

        /// <summary>
        /// Adds an external <see cref="UserLoginInfo"/> to the specified <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to add the login to.</param>
        /// <param name="login">The external <see cref="UserLoginInfo"/> to add to the specified <paramref name="user"/>.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual async Task AddLoginAsync(TUser user, UserLoginInfo login)
        {

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (login == null) throw new ArgumentNullException(nameof(login));
            EnsureLoginsNotNull(user);

            // check if login already exists for this provider and remove old details
            await RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);

            // add new login details to user object in memory and DB
            user.Logins.Add(login);
            var update = Builders<TUser>.Update.Push(x => x.Logins, login);
            await DoUserDetailsUpdate(user.Id, update, null);
        }

        public virtual async Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            EnsureLoginsNotNull(user);

            var existingLogins = user.Logins.Where(l => l.AreEqual(login)).ToList();
            if (existingLogins.Any())
            {
                foreach (var el in existingLogins)
                {
                    user.Logins.Remove(el);
                }

                var updateRemove = Builders<TUser>.Update.PullAll(x => x.Logins, existingLogins);
                await DoUserDetailsUpdate(user.Id, updateRemove, null);
            }
        }

        /// <summary>
        /// Attempts to remove the provided login information from the specified <paramref name="user"/>, as an asynchronous operation.
        /// and returns a flag indicating whether the removal succeed or not.
        /// </summary>
        /// <param name="user">The user to remove the login information from.</param>
        /// <param name="loginProvider">The login provide whose information should be removed.</param>
        /// <param name="providerKey">The key given by the external login provider for the specified user.</param>

        /// <returns>
        /// The <see cref="Task"/> that contains a flag the result of the asynchronous removing operation. The flag will be true if
        /// the login information was existed and removed, otherwise false.
        /// </returns>
        public virtual async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey)
        {
            await this.RemoveLoginAsync(user, new UserLoginInfo(loginProvider, providerKey));
        }

        /// <summary>
        /// Retrieves the associated logins for the specified <param ref="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose associated logins to retrieve.</param>

        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
        /// </returns>
        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            EnsureLoginsNotNull(user);

            IList<UserLoginInfo> logins = user.Logins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey)).ToList();
            return Task.FromResult(logins);
        }

        public virtual async Task<TUser> FindAsync(UserLoginInfo login)
        {
            return await FindByLoginAsync(login.LoginProvider, login.ProviderKey);
        }

        /// <summary>
        /// Retrieves the user associated with the specified login provider and login provider key, as an asynchronous operation..
        /// </summary>
        /// <param name="loginProvider">The login provider who provided the <paramref name="providerKey"/>.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>

        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified login provider and key.
        /// </returns>
        public virtual async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey)
        {

            ThrowIfDisposed();

            var loginBuilder = Builders<UserLoginInfo>.Filter;
            var loginFilter = loginBuilder.Regex(x => x.LoginProvider, new BsonRegularExpression(loginProvider, "i")) & loginBuilder.Regex(x => x.ProviderKey, new BsonRegularExpression(providerKey, "i"));

            var fBuilder = Builders<TUser>.Filter;
            var filter = fBuilder.ElemMatch(x => x.Logins, loginFilter);

            return await DatabaseContext.UserCollection.Find(filter).SingleOrDefaultAsync();
        }

        #endregion

        #region IUserRoleStore<TUser>

        /// <summary>
        /// Add a the specified <paramref name="user"/> to the named role, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to add to the named role.</param>
        /// <param name="normalizedRoleName">The name of the role to add the user to.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual async Task AddToRoleAsync(TUser user, string normalizedRoleName)
        {
            // TODO: tests (case insensitive)

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentNullException(nameof(normalizedRoleName));

            // get role details
            var roleFilter = Builders<TRole>.Filter.Eq(x => x.NormalizedName, Normalize(normalizedRoleName));
            var role = await DatabaseContext.RoleCollection.Find(roleFilter).SingleOrDefaultAsync();

            if (role == null)
            {
                throw new InvalidOperationException($"Role {normalizedRoleName} does not exist.");
            }

            // check if role already exists for user, no need to do anything else if its already on the user
            EnsureRolesNotNull(user);
            if (user.Roles.Any(r => ConvertIdToString(r.Id) == ConvertIdToString(role.Id))) return;

            // add role to user
            user.Roles.Add(role);
            var update = Builders<TUser>.Update.Push(x => x.Roles, role);
            await DoUserDetailsUpdate(user.Id, update, null);
        }

        /// <summary>
        /// Add a the specified <paramref name="user"/> from the named role, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to remove the named role from.</param>
        /// <param name="normalizedRoleName">The normalized name of the role to remove.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual async Task RemoveFromRoleAsync(TUser user, string normalizedRoleName)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentNullException(nameof(normalizedRoleName));
            EnsureRolesNotNull(user);

            // get role details
            var existingRoles = user.Roles.Where(r => r.NormalizedName.Equals(normalizedRoleName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (existingRoles.Any())
            {
                foreach (var er in existingRoles)
                {
                    user.Roles.Remove(er);
                }

                // update in database
                var update = Builders<TUser>.Update.PullAll(x => x.Roles, existingRoles);
                await DoUserDetailsUpdate(user.Id, update, null);
                return;
            }


            var roleFilter = Builders<TRole>.Filter.Regex(x => x.NormalizedName, Normalize(normalizedRoleName));
            var roleFromDb = await DatabaseContext.RoleCollection.Find(roleFilter).SingleOrDefaultAsync();
            if (roleFromDb != null)
            {
                user.Roles.Remove(roleFromDb);
                var update = Builders<TUser>.Update.Pull(x => x.Roles, roleFromDb);
                await DoUserDetailsUpdate(user.Id, update, null);
            }
        }

        /// <summary>
        /// Gets a list of role names the specified <paramref name="user"/> belongs to, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose role names to retrieve.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing a list of role names.</returns>
        public virtual Task<IList<string>> GetRolesAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            EnsureRolesNotNull(user);

            IList<string> roleNames = user.Roles.Select(r => r.Name).ToList();
            return Task.FromResult(roleNames);
        }

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user"/> is a member of the give named role, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose role membership should be checked.</param>
        /// <param name="normalizedRoleName">The normalized name of the role to be checked.</param>

        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing a flag indicating whether the specified <see cref="user"/> is
        /// a member of the named role.
        /// </returns>
        public virtual Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentNullException(nameof(normalizedRoleName));
            EnsureRolesNotNull(user);

            return Task.FromResult(user.Roles.Any(r => r.Name.Equals(normalizedRoleName, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Returns a list of Users who are members of the named role.
        /// </summary>
        /// <param name="normalizedRoleName">The normalized name of the role whose membership should be returned.</param>

        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing a list of users who are in the named role.
        /// </returns>
        public virtual async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName)
        {
            // TODO: tests (case insensitive)

            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentNullException(nameof(normalizedRoleName));

            var roleFilter = Builders<IdentityRole<TKey>>.Filter.Eq(x => x.NormalizedName, Normalize(normalizedRoleName));

            var fBuilder = Builders<TUser>.Filter;
            var filter = fBuilder.ElemMatch(x => x.Roles, roleFilter);

            return await DatabaseContext.UserCollection.Find(filter).ToListAsync();
        }

        #endregion

        #region IUserClaimStore<TUser>

        /// <summary>
        /// Gets a list of all (ie both User.Claims and User.Roles.Claims) <see cref="Claim"/>s to be belonging to the specified <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="user">The role whose claims to retrieve.</param>

        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a list of <see cref="Claim"/>s.
        /// </returns>
        public virtual Task<IList<Claim>> GetClaimsAsync(TUser user)
        {

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            EnsureClaimsNotNull(user);
            EnsureRolesNotNull(user);

            IList<Claim> result = user.AllClaims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
            return Task.FromResult(result);
        }

        public virtual async Task AddClaimAsync(TUser user, Claim claim)
        {
            await this.AddClaimsAsync(user, new[] { claim });
        }

        public virtual async Task RemoveClaimAsync(TUser user, Claim claim)
        {
            await this.RemoveClaimsAsync(user, new[] { claim });
        }

        /// <summary>
        /// Add claims to a user as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to add the claim to.</param>
        /// <param name="claims">The collection of <see cref="Claim"/>s to add.</param>

        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            EnsureClaimsNotNull(user);
            if (claims == null) return;

            // find new claims
            var newClaimsList = claims.Select(x => new IdentityClaim(x))
                                      .Where(x => !user.Claims.Any(uc => uc.Equals(x))).ToList();
            foreach (var c in newClaimsList)
            {
                user.Claims.Add(c);
            }

            // if no new claims - nothing else to do
            if (!newClaimsList.Any()) return;

            // update user claims in the database
            var update = Builders<TUser>.Update.PushEach(x => x.Claims, newClaimsList);
            await DoUserDetailsUpdate(user.Id, update, null);
        }

        /// <summary>
        /// Replaces the given <paramref name="claim"/> on the specified <paramref name="user"/> with the <paramref name="newClaim"/>
        /// </summary>
        /// <param name="user">The user to replace the claim on.</param>
        /// <param name="claim">The claim to replace.</param>
        /// <param name="newClaim">The new claim to replace the existing <paramref name="claim"/> with.</param>

        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (claim == null) throw new ArgumentNullException(nameof(claim));
            if (newClaim == null) throw new ArgumentNullException(nameof(newClaim));
            EnsureClaimsNotNull(user);


            var matchedClaims = user.Claims.Where(uc => uc.Equals(new IdentityClaim(claim))).ToList();
            if (matchedClaims.Any())
            {
                foreach (var matchedClaim in matchedClaims)
                {
                    matchedClaim.ClaimValue = newClaim.Value;
                    matchedClaim.ClaimType = newClaim.Type;
                }

                var update = Builders<TUser>.Update.Set(x => x.Claims, user.Claims);
                await DoUserDetailsUpdate(user.Id, update, null);
            }
        }

        /// <summary>
        /// Removes the specified <paramref name="claims"/> from the given <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the specified <paramref name="claims"/> from.</param>
        /// <param name="claims">A collection of <see cref="Claim"/>s to remove.</param>

        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            EnsureClaimsNotNull(user);
            if (!user.Claims.Any()) return;
            if (claims == null || !claims.Any()) return;

            var existingClaimsList = user.Claims.Where(uc => claims.Any(c => c.Type == uc.ClaimType && c.Value == uc.ClaimValue)).ToList();
            if (!existingClaimsList.Any()) return;

            foreach (var c in existingClaimsList)
            {
                user.Claims.Remove(c);
            }

            // update user claims in the database
            var update = Builders<TUser>.Update.PullAll(x => x.Claims, existingClaimsList);
            await DoUserDetailsUpdate(user.Id, update, null);
        }

        /// <summary>
        /// Returns a list of users who contain the specified <see cref="Claim"/> or have an <see cref="IdentityRole"/> with the specified <see cref="Claim"/>.
        /// </summary>
        /// <param name="claim">The claim to look for.</param>

        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a list of <typeparamref name="TUser"/> who
        /// contain the specified claim.
        /// </returns>
        public virtual async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim)
        {
            ThrowIfDisposed();
            if (claim == null) throw new ArgumentNullException(nameof(claim));

            var claimBuilder = Builders<IdentityClaim>.Filter;
            var claimFilter = claimBuilder.Eq(x => x.ClaimType, claim.Type) & claimBuilder.Eq(x => x.ClaimValue, claim.Value);

            var fBuilder = Builders<TUser>.Filter;
            var filter = Builders<TUser>.Filter.Or(fBuilder.ElemMatch(x => x.AllClaims, claimFilter));

            return await DatabaseContext.UserCollection.Find(filter).ToListAsync();
        }

        #endregion

        #region IUserPasswordStore<TUser>

        /// <summary>
        /// Sets the password hash for the specified <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose password hash to set.</param>
        /// <param name="passwordHash">The password hash to set.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the password hash for the specified <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose password hash to retrieve.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, returning the password hash for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetPasswordHashAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        /// Gets a flag indicating whether the specified <paramref name="user"/> has a password, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to return a flag for, indicating whether they have a password or not.</param>

        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, returning true if the specified <paramref name="user"/> has a password
        /// otherwise false.
        /// </returns>
        public virtual Task<bool> HasPasswordAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        #endregion

        #region IUserSecurityStampStore<TUser>

        /// <summary>
        /// Sets the provided security <paramref name="stamp"/> for the specified <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>
        /// <param name="stamp">The security stamp to set.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetSecurityStampAsync(TUser user, string stamp)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get the security stamp for the specified <paramref name="user" />, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the security stamp for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetSecurityStampAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.SecurityStamp);
        }

        #endregion

        #region IUserEmailStore<TUser>

        /// <summary>
        /// Sets the <paramref name="email"/> address for a <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose email should be set.</param>
        /// <param name="email">The email to set.</param>

        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task SetEmailAsync(TUser user, string email)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.Email = email;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the email address for the specified <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose email should be returned.</param>

        /// <returns>The task object containing the results of the asynchronous operation, the email address for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetEmailAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Gets a flag indicating whether the email address for the specified <paramref name="user"/> has been verified, true if the email address is verified otherwise
        /// false, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose email confirmation status should be returned.</param>

        /// <returns>
        /// The task object containing the results of the asynchronous operation, a flag indicating whether the email address for the specified <paramref name="user"/>
        /// has been confirmed or not.
        /// </returns>
        public virtual Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.EmailConfirmed);
        }

        /// <summary>
        /// Sets the flag indicating whether the specified <paramref name="user"/>'s email address has been confirmed or not, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose email confirmation status should be set.</param>
        /// <param name="confirmed">A flag indicating if the email address has been confirmed, true if the address is confirmed otherwise false.</param>

        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the user, if any, associated with the specified, normalized email address, as an asynchronous operation.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>

        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
        /// </returns>
        public virtual async Task<TUser> FindByEmailAsync(string normalizedEmail)
        {
            // TODO: tests (case insensitive)

            ThrowIfDisposed();

            var filter = Builders<TUser>.Filter.Eq(x => x.NormalizedEmail, Normalize(normalizedEmail));

            return await DatabaseContext.UserCollection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns the normalized email for the specified <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose email address to retrieve.</param>

        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the normalized email address if any associated with the specified user.
        /// </returns>
        public virtual Task<string> GetNormalizedEmailAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.NormalizedEmail ?? Normalize(user.Email));
        }

        /// <summary>
        /// Sets the normalized email for the specified <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose email address to set.</param>
        /// <param name="normalizedEmail">The normalized email to set for the specified <paramref name="user"/>.</param>

        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task SetNormalizedEmailAsync(TUser user, string normalizedEmail)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.NormalizedEmail = Normalize(normalizedEmail);
            return Task.FromResult(0);
        }

        #endregion

        #region IUserLockoutStore<TUser>

        /// <summary>
        /// Gets the last <see cref="DateTimeOffset"/> a user's last lockout expired, if any, as an asynchronous operation.
        /// Any time in the past should be indicates a user is not locked out.
        /// </summary>
        /// <param name="user">The user whose lockout date should be retrieved.</param>

        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a <see cref="DateTimeOffset"/> containing the last time
        /// a user's lockout expired, if any.
        /// </returns>
        public virtual Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.LockoutEnd ?? default(DateTimeOffset));
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.LockoutEnd = lockoutEnd;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Locks out a user until the specified end date has passed, as an asynchronous operation. Setting a end date in the past immediately unlocks a user.
        /// </summary>
        /// <param name="user">The user whose lockout date should be set.</param>
        /// <param name="lockoutEnd">The <see cref="DateTimeOffset"/> after which the <paramref name="user"/>'s lockout should end.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.LockoutEnd = lockoutEnd;
            return Task.FromResult(0);
        }


        /// <summary>
        /// Records that a failed access has occurred, incrementing the failed access count, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose cancellation count should be incremented.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the incremented failed access count.</returns>
        public virtual Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Resets a user's failed access count, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose failed access count should be reset.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>This is typically called after the account is successfully accessed.</remarks>
        public virtual Task ResetAccessFailedCountAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Retrieves the current failed access count for the specified <paramref name="user"/>, as an asynchronous operation..
        /// </summary>
        /// <param name="user">The user whose failed access count should be retrieved.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the failed access count.</returns>
        public virtual Task<int> GetAccessFailedCountAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Retrieves a flag indicating whether user lockout can enabled for the specified user, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose ability to be locked out should be returned.</param>

        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, true if a user can be locked out, otherwise false.
        /// </returns>
        public virtual Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.LockoutEnabled);
        }

        /// <summary>
        /// Set the flag indicating if the specified <paramref name="user"/> can be locked out, as an asynchronous operation..
        /// </summary>
        /// <param name="user">The user whose ability to be locked out should be set.</param>
        /// <param name="enabled">A flag indicating if lock out can be enabled for the specified <paramref name="user"/>.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        #endregion

        #region IUserPhoneNumberStore<TUser>

        /// <summary>
        /// Sets the telephone number for the specified <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose telephone number should be set.</param>
        /// <param name="phoneNumber">The telephone number to set.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the telephone number, if any, for the specified <paramref name="user"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose telephone number should be retrieved.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the user's telephone number, if any.</returns>
        public virtual Task<string> GetPhoneNumberAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        /// Gets a flag indicating whether the specified <paramref name="user"/>'s telephone number has been confirmed, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to return a flag for, indicating whether their telephone number is confirmed.</param>

        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, returning true if the specified <paramref name="user"/> has a confirmed
        /// telephone number otherwise false.
        /// </returns>
        public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <summary>
        /// Sets a flag indicating if the specified <paramref name="user"/>'s phone number has been confirmed, as an asynchronous operation..
        /// </summary>
        /// <param name="user">The user whose telephone number confirmation status should be set.</param>
        /// <param name="confirmed">A flag indicating whether the user's telephone number has been confirmed.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        #endregion

        #region IUserTwoFactorStore<TUser>

        /// <summary>
        /// Sets a flag indicating whether the specified <paramref name="user "/>has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="enabled">A flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled.</param>

        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user "/>has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>

        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing a flag indicating whether the specified
        /// <paramref name="user "/>has two factor authentication enabled or not.
        /// </returns>
        public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            // TODO: tests

            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.TwoFactorEnabled);
        }

        #endregion

        #region IQueryableUserStore<TUser>

        /// <summary>
        /// Returns an <see cref="IQueryable{T}"/> collection of users.
        /// </summary>
        /// <value>An <see cref="IQueryable{T}"/> collection of users.</value>
        public virtual IQueryable<TUser> Users
        {
            get
            {
                ThrowIfDisposed();
                return DatabaseContext.UserCollection.AsQueryable();
            }
        }

        #endregion

        #region IDisposable

        private bool _disposed = false; // To detect redundant calls


        public virtual void Dispose()
        {
            _disposed = true;
        }

        /// <summary>
        /// Throws if disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException"></exception>
        protected virtual void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        #endregion

        #region HELPER METHODS

        /// <summary>
        /// User userNames are distinct, and should never have two users with the same name
        /// </summary>
        /// <remarks>
        /// Can override to have different "distinct user details" implementation if necessary.
        /// </remarks>
        /// <param name="user"></param>

        /// <returns></returns>
        protected virtual async Task<bool> UserDetailsAlreadyExists(TUser user)
        {
            ConfigureDefaults(user);
            // if the result does exist, make sure its not for the same user object (ie same userName, but different Ids)
            var fBuilder = Builders<TUser>.Filter;
            var filter = fBuilder.Eq(x => x.NormalizedUserName, Normalize(user.NormalizedUserName)) & fBuilder.Ne(x => x.Id, user.Id);

            var result = await DatabaseContext.UserCollection.Find(filter).FirstOrDefaultAsync();
            return result != null;
        }

        protected virtual TKey ConvertIdFromString(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return default(TKey);
            }
            return (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
        }

        protected virtual string ConvertIdToString(TKey id)
        {
            if (id == null || id.Equals(default(TKey)))
            {
                return null;
            }
            return id.ToString();
        }

        /// <summary>
        /// update sub-set of user details in database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="update"></param>
        /// <param name="options"></param>

        /// <returns></returns>
        protected virtual Task<UpdateResult> DoUserDetailsUpdate(TKey userId, UpdateDefinition<TUser> update, UpdateOptions options = null)
        {
            var filter = Builders<TUser>.Filter.Eq(x => x.Id, userId);
            update.Set(x => x.LastEditedDate, DateTime.UtcNow);
            return DatabaseContext.UserCollection.UpdateOneAsync(filter, update, options);
        }

        protected virtual void EnsureLoginsNotNull(TUser user)
        {
            if (user.Logins == null) user.Logins = new List<UserLoginInfo>();
        }

        protected virtual void EnsureClaimsNotNull(TUser user)
        {
            if (user.Claims == null) user.Claims = new List<IdentityClaim>();
        }

        protected virtual void EnsureRolesNotNull(TUser user)
        {
            if (user.Roles == null) user.Roles = new List<TRole>().Cast<IdentityRole<TKey>>().ToList();
        }

        /// <summary>
        /// Configure any default settings for the user (Default fills in missing NormalizedEmail and NormalizedUserName from Email and UserName)
        /// </summary>
        /// <returns></returns>
        protected virtual void ConfigureDefaults(TUser user)
        {
            if (string.IsNullOrWhiteSpace(user.NormalizedUserName) || !user.NormalizedUserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase)) user.NormalizedUserName = Normalize(user.UserName);
            if (string.IsNullOrWhiteSpace(user.NormalizedEmail) || !user.NormalizedEmail.Equals(user.Email, StringComparison.OrdinalIgnoreCase)) user.NormalizedEmail = Normalize(user.Email);
            user.LastEditedDate = DateTime.UtcNow;
        }


        /// <summary>
        /// Used to ensure consistent formatting of normalized string values. Uses the ILookupNormalizer if its supplied, otherwise converts strings to lowercase and trims;
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected virtual string Normalize(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            return str.ToLower().Trim();
        }

        #endregion
    }

    internal class UserStore<TUser, TRole> : UserStore<TUser, TRole, string>
        where TUser : IdentityUser<string>
        where TRole : IdentityRole<string>
    {
        public UserStore(IIdentityDatabaseContext<TUser, TRole, string> databaseContext) : base(databaseContext) { }
    }
}