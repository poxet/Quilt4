using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface ICounterBusiness
    {
        void RegisterCounter(ICounter counter);
        IEnumerable<ICounter> GetRawData(string counterName, Predicate<ICounter> selection = null);
        IEnumerable<ICounter> GetAggregatedData(string counterName, Predicate<ICounter> predicate = null);
        ICounterCollection GetAggregatedCount<TKey>(string counterName, Precision precision = Precision.Ticks, Func<ICounter, TKey> grouping = null, Predicate<ICounter> selection = null);
    }
}