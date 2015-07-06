using System.Collections.Generic;

namespace Quilt4.Web.Models
{
    public class ApplicationPropetiesModel
    {
        public string InitiativeId { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationGroupName { get; set; }
        public string TicketPrefix { get; set; }
        public string DevColor { get; set; }
        public string ProdColor { get; set; }
        public string CiColor { get; set; }
        public IEnumerable<string> Environments { get; set; }
    }
}