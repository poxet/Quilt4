using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class IssueViewModel
    {
        public string InitiativeId { get; set; }
        public string InitiativeUniqueIdentifier { get; set; }
        public string InitiativeName { get; set; }
        public string ApplicationName { get; set; }
        public string Version { get; set; }
        public IEnumerable<ISession> Sessions { get; set; }
        public IEnumerable<IUser> Users { get; set; }
        public IEnumerable<IIssueType> IssueTypes { get; set; }
        public IEnumerable<IMachine> Machines { get; set; }
        public string ApplicationVersionId { get; set; }
        public string UniqueIdentifier { get; set; }
        public string VersionName { get; set; }
        public string DevColor { get; set; }
        public string CiColor { get; set; }
        public string ProdColor { get; set; }
        public List<EnvironmentViewModel> Environments { get; set; }
    }

}