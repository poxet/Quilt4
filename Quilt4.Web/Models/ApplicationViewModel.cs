using System;
using System.Collections.Generic;

namespace Quilt4.Web.Models
{
    public class ApplicationViewModel
    {
        public Guid InitiativeId { get; set; }
        public string InitiativeName { get; set; }
        public string Application { get; set; }
        public string InitiativeUniqueIdentifier { get; set; }
        public List<VersionViewModel> Versions { get; set; }
        //public bool ShowArchivedVersions { get; set; }
        //public List<VersionViewModel> ArchivedVersions { get; set; }
    }
}