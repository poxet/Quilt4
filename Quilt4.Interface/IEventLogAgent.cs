using System;

namespace Quilt4.Interface
{
    public interface IEventLogAgent
    {
        Exception AssureEventLogSource();
        void DeleteLog();
        void WriteToEventLog(Exception exception);
    }
}