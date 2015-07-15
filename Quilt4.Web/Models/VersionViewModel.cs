using System;
using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class VersionViewModel
    {
        public bool Checked { get; set; }
        public string Build { get; set; }
        public string VersionId { get; set; }
        public string Version { get; set; }
        public string VersionIdentifier { get; set; }
        public string InitiativeIdentifier { get; set; }
        public string ApplicationIdentifier { get; set; }
        public int IssueTypeCount { get; set; }
        public int IssueCount { get; set; }
        //TODO: Ta bort dessa properties. De skall laddas separat med ett jquery anrop.
        public int MachineCount { get; set; }
        public int SessionCount { get; set; }
        public DateTime FirstSessionTime { get; set; }
        public DateTime LastSessionTime { get; set; }
        public List<EnvironmentViewModel> Environments { get; set; }
        public IEnumerable<string> Environment { get; set; }
    }
}