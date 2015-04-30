using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
}