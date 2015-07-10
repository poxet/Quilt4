using System;

namespace Quilt4.Interface
{
    public interface ICounterBusiness
    {
        void ClearSessionCounters();
        void UpdateSessionCounters();
    }
}