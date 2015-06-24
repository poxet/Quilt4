using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class ApplicationModel
    {
        public string Initiative { get; set; }
        public string Application { get; set; }
        public IEnumerable<VersionModel> Versions { get; set; }
        
    }

    public class VersionModel
    {
        public string UniqueIdentifier { get; set; }
        public string Version { get; set; }
        public IEnumerable<IIssueType> IssueTypes { get; set; }
        public IEnumerable<ISession> Sessions { get; set; } 
    }
}