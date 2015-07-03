using System;
using System.Diagnostics;

namespace Quilt4.Web.Models
{
    public class EventLogItemModel
    {
        public string Icon { get; set; }
        public EventLogEntryType EntryType { get; set; }
        public DateTime TimeGenerated { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
    }
}