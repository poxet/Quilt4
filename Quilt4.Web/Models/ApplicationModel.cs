using System.Collections.Generic;

namespace Quilt4.Web.Models
{
    public class ApplicationModel
    {
        public string Initiative { get; set; }
        public string InitiativeName { get; set; }
        public string InitiativeUniqueIdentifier { get; set; }
        public string Application { get; set; }
        public List<VersionViewModel> Versions { get; set; }
        public bool ShowArchivedVersions { get; set; }
        public List<VersionViewModel> ArchivedVersions { get; set; }
        public string DevColor { get; set; }
        public string CiColor { get; set; }
        public string ProdColor { get; set; }
    }
}