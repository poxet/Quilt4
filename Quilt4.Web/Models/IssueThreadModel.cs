using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class IssueThreadModel
    {
        public string InitiativeUniqueIdentifier { get; set; }
        public string InitiativeName { get; set; }
        public IEnumerable<IIssue> Issues { get; set; }
        public IEnumerable<ISession> Sessions { get; set; }
        public IEnumerable<IUser> Users { get; set; }
    }
}