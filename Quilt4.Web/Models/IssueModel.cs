using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class IssueModel
    {
        public string InitiativeId { get; set; }
        public string ApplicationName { get; set; }
        public string Version { get; set; }
        public IEnumerable<ISession> Sessions { get; set; }
        public IEnumerable<IUser> Users { get; set; }
        public IEnumerable<IIssueType> IssueTypes { get; set; }
        public IEnumerable<IMachine> Machines { get; set; }
        public string ApplicationVersionId { get; set; }
        public string UniqueIdentifier { get; set; }
        public string VersionName { get; set; }
 
    }
}