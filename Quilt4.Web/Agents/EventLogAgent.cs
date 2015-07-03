using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Agents
{
    public class EventLogAgent : IEventLogAgent
    {
        public void DeleteLog()
        {
            if (EventLog.SourceExists(Constants.EventSourceName))
            {
                EventLog.DeleteEventSource(Constants.EventSourceName);
            }

            if (EventLog.GetEventLogs().Any(x => x.Log == Constants.EventLogName))
            {
                EventLog.Delete(Constants.EventLogName);
            }
        }

        public IEnumerable<EventLogEntry> GetEventLogData()
        {
            var myLog = new EventLog(Constants.EventLogName);
            foreach (EventLogEntry entry in myLog.Entries)
            {
                yield return entry;
            }
        }

        public Exception AssureEventLogSource()
        {
            try
            {
                if (!EventLog.SourceExists(Constants.EventSourceName))
                {
                    EventLog.CreateEventSource(new EventSourceCreationData(Constants.EventSourceName, Constants.EventLogName));
                }
                return null;
            }
            catch (Exception exception)
            {
                return exception;
            }
        }

        public void WriteToEventLog(string message, EventLogEntryType eventLogEntryType)
        {
            AssureEventLogSource();

            var myLog = new EventLog(Constants.EventLogName);
            myLog.Source = Constants.EventSourceName;
            myLog.WriteEntry(message, eventLogEntryType);
        }

        public void WriteToEventLog(Exception exception)
        {
            var message = GetMessageFromException(exception);
            WriteToEventLog(message, EventLogEntryType.Error);
        }

        private string GetMessageFromException(Exception exception, bool appendStackTrace = true)
        {
            if (exception == null)
                return null;

            var sb = new StringBuilder();

            sb.AppendFormat("{0} ", exception.Message);
            if (exception.Data.Count > 0)
            {
                sb.Append("[");
                var sb2 = new StringBuilder();
                foreach (DictionaryEntry data in exception.Data)
                {
                    sb2.Append(data.Key + ": " + data.Value + ", ");
                }
                sb.Append(sb2.ToString().TrimEnd(',', ' '));
                sb.Append("]");
            }

            var subMessage = GetMessageFromException(exception.InnerException, false);
            if (!string.IsNullOrEmpty(subMessage))
            {
                sb.AppendFormat(" / {0}", subMessage);
            }

            if (appendStackTrace)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendFormat("@ {0}", exception.StackTrace);
            }

            return sb.ToString();
        }
    }
}