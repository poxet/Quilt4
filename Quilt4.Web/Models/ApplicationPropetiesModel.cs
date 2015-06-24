using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quilt4.Web.Models
{
    public class ApplicationPropetiesModel
    {
        public string InitiativeId { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationGroupName { get; set; }
        public string TicketPrefix { get; set; }
    }
}