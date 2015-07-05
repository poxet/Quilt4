using System;
using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class VersionViewModel
    {
        public string UniqueIdentifier { get; set; }
        public string VersionId { get; set; }
        public string Version { get; set; }
        public string Build { get; set; }
        public IEnumerable<IIssueType> IssueTypes { get; set; }
        public IEnumerable<ISession> Sessions { get; set; }
        public IEnumerable<IMachine> Machines { get; set; }
        public bool Checked { get; set; }
        public string InitiativeIdentifier { get; set; }
        public string ApplicationIdentifier { get; set; }
    }
}