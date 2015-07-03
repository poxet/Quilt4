using System.Collections.Generic;

namespace Quilt4.Web.Models
{
    public class EventLogStatusViewModel
    {
        public string EventLogCheckMessage { get; set; }
        public List<EventLogItemModel> EventLogData { get; set; }
    }
}