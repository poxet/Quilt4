using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Quilt4.BusinessEntities;
using Quilt4.Interface;

namespace Quilt4.Web.Business
{
    public class CounterBusiness : ICounterBusiness
    {
        private readonly IRepository _repository;

        public CounterBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public void RegisterCounter(ICounter data)
        {
            //TODO: Store counter data
            throw new NotImplementedException();
        }

        public IEnumerable<ICounter> GetRawData(string counterName, Predicate<ICounter> selection = null)
        {
            var counters = _repository.GetAllCounters(counterName).ToList();

            var response = counters.FindAll(selection ?? (x => true));

            return response;
        }

        public IEnumerable<ICounter> GetAggregatedData(string counterName, Predicate<ICounter> selection = null)
        {
        //    var response = GetRawData(counterName, selection);

        //    //TODO: Some sort of information on how to aggregate this data
        //    var grouped = response.GroupBy(x => x.DateTime);

        //    foreach (var item in grouped)
        //    {
        //        yield return new Quilt4.BusinessEntities.Counter(counterName, item.Key, item.Average(x => x.Duration), item.Sum(x => x.Count), null, null, null, null);
        //    }
            throw new NotImplementedException();
        }

        //TODO: Make it possible to get more 'columns' specified by input parameters
        //TODO: Add empty padding values so that each 'strata' gets a value. (If there are no entry points at all)
        //var grouped = items.GroupBy(x => GetKey(x.DateTime));
        public ICounterCollection GetAggregatedCount<TKey>(string counterName, Precision precision = Precision.Ticks, Func<ICounter,TKey> grouping = null, Predicate<ICounter> selection = null)
        {
            var response = GetRawData(counterName, selection).ToArray();

            IEnumerable<IGrouping<DateTime, ICounter>> grouped;
            switch (precision)
            {
                case Precision.Ticks:
                    grouped = response.GroupBy(x => x.DateTime);
                    break;
                case Precision.Seconds:
                    grouped = response.GroupBy(x => DateTime.Parse(x.DateTime.ToString("s")));
                    break;
                case Precision.Minutes:
                    grouped = response.GroupBy(x => DateTime.Parse(x.DateTime.ToString("s").Substring(0,16)));
                    break;
                case Precision.Hours:
                    grouped = response.GroupBy(x => DateTime.Parse(x.DateTime.ToString("s").Substring(0, 13) + ":00:00"));
                    break;
                case Precision.Days:
                    grouped = response.GroupBy(x => DateTime.Parse(x.DateTime.ToString("yyyy-MM-dd")));
                    break;
                case Precision.Months:
                    grouped = response.GroupBy(x => DateTime.Parse(x.DateTime.ToString("yyyy-MM") + "-01"));
                    break;
                case Precision.Years:
                    grouped = response.GroupBy(x => DateTime.Parse(x.DateTime.ToString("yyyy") + "-01-01"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unknown precision {0}.", precision));
            }
            
            var names = GetNames(grouping, response);

            var lines = new List<ICounterLine>();

            foreach (var item in grouped)
            {                
                var key = item.Key.ToString(CultureInfo.InvariantCulture);

                var counts = new List<int> { item.Sum(x => x.Count) };
                if (grouping != null)
                {
                    var subGroup = item.GroupBy(grouping).ToArray();
                    foreach (var name in names)
                    {
                        var element = subGroup.SingleOrDefault(x => x.Key.ToString() == name);

                        var value = 0;
                        if (element != null)
                        {
                            value = element.Sum(x => x.Count);
                        }
                        counts.Add(value);
                    }
                }

                var line = new CounterLine(key, counts.ToArray());
                lines.Add(line);
            }

            names.Insert(0, "Total");
            var counterCollection = new CounterCollection(names.ToArray(), lines.ToArray());
            return counterCollection;
        }

        private static List<string> GetNames<TKey>(Func<ICounter, TKey> grouping, ICounter[] response)
        {
            var names = new List<string>();
            if (grouping != null)
            {
                var gbp = response.GroupBy(grouping);
                names.AddRange(gbp.Select(gb => gb.Key.ToString()));
            }
            return names;
        }
    }
}