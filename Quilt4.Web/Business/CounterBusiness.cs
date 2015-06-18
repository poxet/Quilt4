using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        //TODO: Make it possible to specify precision in time.
        //Seconds
        //Minutes
        //Hours
        //Days
        //Weeks ???
        //Months
        //Years
        //TODO: Add empty padding values so that each 'strata' gets a value. (If there are no entry points at all)
        //var grouped = items.GroupBy(x => GetKey(x.DateTime));
        public ICounterCollection GetAggregatedCount<TKey>(string counterName, Func<ICounter,TKey> grouping, Predicate<ICounter> selection = null)
        {
            var response = GetRawData(counterName, selection);

            //TODO: How do I dynamically provide data to select what filters should be used            
            //var g = new Func<ICounter, TKey>(() => { });
            //var grouped = response.GroupBy(x => x.DateTime);
            var grouped = response.GroupBy(grouping);
            //var grouped = response.GroupBy(x => new { x.DateTime, x.Environment });
            //var grouped = response.GroupBy(x => new { x.DateTime, grouping.Invoke(x) });

            //var lambda = Expression.Lambda<Func<ICounter, object>>(body, arg); 

            //Lambda for dateTime
            //var arg = Expression.Parameter(typeof(ICounter), "transaction");
            //var body = Expression.Convert(Expression.Property(arg, "DateTime"), typeof(object)); 
            //var lambda = Expression.Lambda<Func<ICounter, object>>(body, arg); 
            //var keySelector = lambda.Compile();


            //Expression.Convert(,)
            //Expression.() grouping

            //var x = Expression.Lambda<Func<ICounter, bool>>(Expression.Not(expr.Body), expr.Parameters[0]);
            //var body2 = Expression.AndAlso(body, expr2.Body);
            //var lambda2 = Expression.Lambda<Func<ICounter, bool>>(body, expr1.Parameters[0]);

            //var grouped = response.GroupBy(x => keySelector);

            var lines = new List<ICounterLine>();

            foreach (var item in grouped)
            {
                var key = item.Key.ToString();
                var line = new CounterLine(key, new[] { item.Sum(x => x.Count) });
                lines.Add(line);
            }

            var counterCollection = new CounterCollection(new[] { "Total" }, lines.ToArray());
            return counterCollection;
        }
    }
}