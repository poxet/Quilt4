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
    }
}