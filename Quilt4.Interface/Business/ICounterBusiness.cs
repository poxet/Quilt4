using System;

namespace Quilt4.Interface
{
    public interface ICounterBusiness
    {
        void ClearSessionCounters();
        DateTime GetLastSessionCounterTime();
        void UpdateSessionCounters();
    }
}