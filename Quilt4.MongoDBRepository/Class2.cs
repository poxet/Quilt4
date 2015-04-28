using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Quilt4.MongoDBRepository
{
    public class IdentityUser : IUser<string>
    {
        /// <summary>
        /// Unique key for the user
        /// </summary>
        /// <value>The identifier.</value>
        /// <returns>The unique key for the user</returns>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Get or sets the email of the user;
        /// </summary>
        /// <value>The email of the user.</value>
        public virtual string Email { get; set; }

        /// <summary>
        /// Get or set if the email is confirmed
        /// </summary>
        /// <value>True or False</value>
        public virtual bool EmailConfirmed { get; set; }

        /// <summary>
        /// Get or set the phone number of the user.
        /// </summary>
        /// <value>The user's phone number</value>
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// Get or set if user's phone number is confirmed
        /// </summary>
        /// <value>True or False</value>
        public virtual bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Get or set if Two Factor is enabled
        /// </summary>
        /// <value>True or False</value>
        public virtual bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Gets or sets the password hash.
        /// </summary>
        /// <value>The password hash.</value>
        public virtual string PasswordHash { get; set; }
        /// <summary>
        /// Gets or sets the security stamp.
        /// </summary>
        /// <value>The security stamp.</value>
        public virtual string SecurityStamp { get; set; }
        /// <summary>
        /// Gets the roles.
        /// </summary>
        /// <value>The roles.</value>
        public virtual List<string> Roles { get; private set; }
        /// <summary>
        /// Gets the claims.
        /// </summary>
        /// <value>The claims.</value>
        public virtual List<IdentityUserClaim> Claims { get; private set; }
        /// <summary>
        /// Gets the logins.
        /// </summary>
        /// <value>The logins.</value>
        public virtual List<UserLoginInfo> Logins { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUser"/> class.
        /// </summary>
        public IdentityUser()
        {
            this.Claims = new List<IdentityUserClaim>();
            this.Roles = new List<string>();
            this.Logins = new List<UserLoginInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUser"/> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        public IdentityUser(string userName)
            : this()
        {
            this.UserName = userName;
        }
    }

    /// <summary>
    /// Class IdentityUserClaim.
    /// </summary>
    public class IdentityUserClaim
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string Id { get; set; }
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        public virtual string UserId { get; set; }
        /// <summary>
        /// Gets or sets the type of the claim.
        /// </summary>
        /// <value>The type of the claim.</value>
        public virtual string ClaimType { get; set; }
        /// <summary>
        /// Gets or sets the claim value.
        /// </summary>
        /// <value>The claim value.</value>
        public virtual string ClaimValue { get; set; }
    }

    /// <summary>
    ///     Class UserStore.
    /// </summary>
    /// <typeparam name="TUser">The type of the t user.</typeparam>
    public class UserStore<TUser> 
        : IUserLoginStore<TUser>, IUserClaimStore<TUser>, IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>, IUserSecurityStampStore<TUser>, IUserStore<TUser>, IUserEmailStore<TUser>,
        IUserPhoneNumberStore<TUser>, IUserTwoFactorStore<TUser, string>
        , IUserLockoutStore<TUser, string>
        where TUser : IdentityUser
    {
        #region Private Methods & Variables

        /// <summary>
        ///     The database
        /// </summary>
        private readonly IMongoDatabase db;

        /// <summary>
        ///     The _disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The AspNetUsers collection name
        /// </summary>
        private const string collectionName = "AspNetUsers";

        /// <summary>
        ///     Gets the database from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>MongoDatabase.</returns>
        private IMongoDatabase GetDatabaseFromUrl(MongoUrl url)
        {
            var client = new MongoClient(url);
            if (url.DatabaseName == null)
            {
                throw new Exception("No database name specified in connection string");
            }
            return client.GetDatabase(url.DatabaseName); // WriteConcern defaulted to Acknowledged
        }

        /// <summary>
        ///     Uses connectionString to connect to server and then uses databae name specified.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dbName">Name of the database.</param>
        /// <returns>MongoDatabase.</returns>
        private IMongoDatabase GetDatabase(string connectionString, string dbName)
        {
            var client = new MongoClient(connectionString);
            return client.GetDatabase(dbName);
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserStore{TUser}" /> class. Uses DefaultConnection name if none was
        ///     specified.
        /// </summary>
        public UserStore()
            : this("DefaultConnection")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserStore{TUser}" /> class. Uses name from ConfigurationManager or a
        ///     mongodb:// Url.
        /// </summary>
        /// <param name="connectionNameOrUrl">The connection name or URL.</param>
        public UserStore(string connectionNameOrUrl)
        {
            if (connectionNameOrUrl.ToLower().StartsWith("mongodb://"))
            {
                db = GetDatabaseFromUrl(new MongoUrl(connectionNameOrUrl));
            }
            else
            {
                string connStringFromManager =
                    ConfigurationManager.ConnectionStrings[connectionNameOrUrl].ConnectionString;

                db = GetDatabaseFromUrl(new MongoUrl(connStringFromManager));

            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserStore{TUser}" /> class. Uses name from ConfigurationManager or a
        ///     mongodb:// Url.
        ///     Database can be specified separately from connection server.
        /// </summary>
        /// <param name="connectionNameOrUrl">The connection name or URL.</param>
        /// <param name="dbName">Name of the database.</param>
        public UserStore(string connectionNameOrUrl, string dbName)
        {
            if (connectionNameOrUrl.ToLower().StartsWith("mongodb://"))
            {
                db = GetDatabase(connectionNameOrUrl, dbName);
            }
            else
            {
                db = GetDatabase(ConfigurationManager.ConnectionStrings[connectionNameOrUrl].ConnectionString, dbName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStore{TUser}"/> class using a already initialized Mongo Database.
        /// </summary>
        /// <param name="mongoDatabase">The mongo database.</param>
        public UserStore(IMongoDatabase mongoDatabase)
        {
            db = mongoDatabase;
        }


        #endregion

        #region Methods

        /// <summary>
        ///     Adds the claim asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="claim">The claim.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task AddClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            if (!user.Claims.Any(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value))
            {
                user.Claims.Add(new IdentityUserClaim
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }


            return Task.FromResult(0);
        }

        /// <summary>
        ///     Gets the claims asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{IList{Claim}}.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            IList<Claim> result = user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
            return Task.FromResult(result);
        }

        /// <summary>
        ///     Removes the claim asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="claim">The claim.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            user.Claims.RemoveAll(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            return Task.FromResult(0);
        }


        /// <summary>
        ///     Creates the user asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task CreateAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return db.GetCollection<TUser>(collectionName).InsertOneAsync(user);
        }

        /// <summary>
        ///     Deletes the user asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task DeleteAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(db.GetCollection<TUser>(collectionName).DeleteOneAsync<TUser>(x => x.Id == user.Id));
        }

        /// <summary>
        ///     Finds the by identifier asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task{`0}.</returns>
        public Task<TUser> FindByIdAsync(string userId)
        {
            ThrowIfDisposed();

            return (db.GetCollection<TUser>(collectionName).Find<TUser>(x => x.Id == userId)).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Finds the by name asynchronous.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>Task{`0}.</returns>
        public Task<TUser> FindByNameAsync(string userName)
        {
            ThrowIfDisposed();
            return db.GetCollection<TUser>(collectionName).Find(x => x.UserName == userName).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Updates the user asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task UpdateAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(db.GetCollection<TUser>(collectionName).ReplaceOneAsync(x => x.Id == user.Id, user));
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
        }

        /// <summary>
        ///     Adds the login asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="login">The login.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            if (!user.Logins.Any(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey))
            {
                user.Logins.Add(login);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        ///     Finds the user asynchronous.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <returns>Task{`0}.</returns>
        public async Task<TUser> FindAsync(UserLoginInfo login)
        {
            throw new NotImplementedException();
            //return (await db.GetCollection<TUser>(collectionName)
            //        .Find(x => x.Logins.Any(y => y.ProviderKey == login.ProviderKey
            //             && y.LoginProvider == login.LoginProvider)).ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        ///     Gets the logins asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{IList{UserLoginInfo}}.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            throw new NotImplementedException();
            //ThrowIfDisposed();
            //if (user == null)
            //    throw new ArgumentNullException("user");

            //return Task.FromResult(user.Logins.ToIList());
        }

        /// <summary>
        ///     Removes the login asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="login">The login.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            user.Logins.RemoveAll(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);

            return Task.FromResult(0);
        }

        /// <summary>
        ///     Gets the password hash asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{System.String}.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<string> GetPasswordHashAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        ///     Determines whether [has password asynchronous] [the specified user].
        /// </summary>
        /// <param name="user">The user.</param>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<bool> HasPasswordAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PasswordHash != null);
        }

        /// <summary>
        ///     Sets the password hash asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="passwordHash">The password hash.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Adds to role asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="role">The role.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task AddToRoleAsync(TUser user, string role)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            if (!user.Roles.Contains(role, StringComparer.InvariantCultureIgnoreCase))
                user.Roles.Add(role);

            return Task.FromResult(true);
        }

        /// <summary>
        ///     Gets the roles asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{IList{System.String}}.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult<IList<string>>(user.Roles);
        }

        /// <summary>
        ///     Determines whether [is in role asynchronous] [the specified user].
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="role">The role.</param>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<bool> IsInRoleAsync(TUser user, string role)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.Roles.Contains(role, StringComparer.InvariantCultureIgnoreCase));
        }

        /// <summary>
        ///     Removes from role asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="role">The role.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task RemoveFromRoleAsync(TUser user, string role)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            user.Roles.RemoveAll(r => String.Equals(r, role, StringComparison.InvariantCultureIgnoreCase));

            return Task.FromResult(0);
        }

        /// <summary>
        ///     Gets the security stamp asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{System.String}.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<string> GetSecurityStampAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        ///     Sets the security stamp asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="stamp">The stamp.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Throws if disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException"></exception>
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);


        }

        #endregion

        /// <summary>
        /// Find user by Email asynchronous
        /// </summary>
        /// <param name="email">The user email</param>
        /// <returns>User</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<TUser> FindByEmailAsync(string email)
        {
            ThrowIfDisposed();
            if (email == "")
                throw new ArgumentNullException("email");

            return db.GetCollection<TUser>(collectionName).Find(x => x.Email == email).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get email user asynchronous
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Email</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<string> GetEmailAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Get email is confirmed asynchronous
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>True or False</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");


            return Task.FromResult(user.EmailConfirmed);
        }

        /// <summary>
        /// Set user's email
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="email">Email</param>        
        /// <exception cref="System.ArgumentNullException">user</exception>        
        public Task SetEmailAsync(TUser user, string email)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");


            user.Email = email;

            return db.GetCollection<TUser>(collectionName).ReplaceOneAsync(x => x.Id == user.Id, user);
        }

        /// <summary>
        /// Set if email is confirmed
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="confirmed">Is Confirmed?</param>
        /// <exception cref="System.ArgumentNullException">user</exception>   
        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");


            user.EmailConfirmed = confirmed;
            return db.GetCollection<TUser>(collectionName).ReplaceOneAsync(x => x.Id == user.Id, user);
        }

        /// <summary>
        /// Get user's phone number
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Phone Number</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>   
        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");


            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        /// Get if user confirmed phone number
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>True or False</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>   
        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");


            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <summary>
        /// Set phone number asynchronous
        /// </summary>
        /// <param name="user"></param>
        /// <param name="phoneNumber"></param>
        /// <exception cref="System.ArgumentNullException">user</exception>   
        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");


            user.PhoneNumber = phoneNumber;
            return db.GetCollection<TUser>(collectionName).ReplaceOneAsync(x => x.Id == user.Id, user);
        }

        /// <summary>
        /// Set phone number confirmed asynchronous
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="confirmed">Is Confirmed?</param>
        /// <exception cref="System.ArgumentNullException">user</exception>  
        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {

            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            user.PhoneNumberConfirmed = confirmed;
            return db.GetCollection<TUser>(collectionName).ReplaceOneAsync(x => x.Id == user.Id, user);
        }

        /// <summary>
        /// Get Two Factor
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>True or False</returns>
        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.TwoFactorEnabled);
        }


        /// <summary>
        /// Set two factor
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="enabled">Use Two Factor?</param>
        /// <returns></returns>
        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return db.GetCollection<TUser>(collectionName).ReplaceOneAsync(x => x.Id == user.Id, user);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }
    }
}
