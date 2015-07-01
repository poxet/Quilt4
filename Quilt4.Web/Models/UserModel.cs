using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class UserModel
    {
        public IEnumerable<IUser> Users { get; set; }
        public IEnumerable<string> ApplicationName { get; set; }
        public IEnumerable<ISession> Sessions { get; set; }
        public IEnumerable<string> Machines { get; set; }
    }
}