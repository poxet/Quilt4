using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface ICounterBusiness
    {
        void ClearSessionCounters();
        DateTime GetLastSessionCounterTime();
        void UpdateSessionCounters(IEnumerable<ISession> sessions);
    }
}