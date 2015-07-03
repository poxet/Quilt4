using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Quilt4.Interface
{
    public interface IEventLogAgent
    {
        Exception AssureEventLogSource();
        void DeleteLog();
        void WriteToEventLog(Exception exception);
        IEnumerable<EventLogEntry> GetEventLogData();
    }
}