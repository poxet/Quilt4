using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class VersionModel
    {
        public string UniqueIdentifier { get; set; }
        public string Version { get; set; }
        public string Build { get; set; }
        public IEnumerable<IIssueType> IssueTypes { get; set; }
        public IEnumerable<ISession> Sessions { get; set; }
        public IEnumerable<IMachine> Machines { get; set; }
    }
}