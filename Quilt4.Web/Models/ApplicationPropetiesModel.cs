namespace Quilt4.Web.Models
{
    public class ApplicationPropetiesModel
    {
        public string InitiativeId { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationGroupName { get; set; }
        public string TicketPrefix { get; set; }
        public int? KeepLatestVersions { get; set; }
    }
}