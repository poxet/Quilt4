using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class UserViewModel
    {
        public IEnumerable<IUser> Users { get; set; }
        public IEnumerable<string> ApplicationNames { get; set; }
        public IEnumerable<ISession> Sessions { get; set; }
        public IEnumerable<IMachine> Machines { get; set; }
        public string InitiativeName { get; set; }
        public string InitiativeUniqueIdentifier { get; set; }
    }
}