using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface ICounterBusiness
    {
        void RegisterCounter(ICounter counter);
        IEnumerable<ICounter> GetRawData(string counterName, Predicate<ICounter> predicate = null);
        IEnumerable<ICounter> GetAggregatedData(string counterName, Predicate<ICounter> predicate = null);
    }
}