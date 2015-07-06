using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class IssueTypeModel
    {
        public string Application { get; set; }
        public string Version { get; set; }
        public IIssueType IssueType { get; set; }
        public IEnumerable<ISession> Sessions { get; set; }
        public IEnumerable<IUser> Users { get; set; }
        public string InitiativeUniqueIdentifier { get; set; }
        public string InitiativeName { get; set; }
        public string ApplicationName { get; set; }
        public string VersionUniqueIdentifier { get; set; }
    }
}