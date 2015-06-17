using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<ICounter> GetRawData(string counterName, Predicate<ICounter> predicate = null)
        {
            var counters = _repository.GetAllCounters(counterName).ToList();

            var response = counters.FindAll(predicate ?? (x => true));

            return response;
        }

        public IEnumerable<ICounter> GetAggregatedData(string counterName, Predicate<ICounter> predicate = null)
        {
            var response = GetRawData(counterName, predicate);

            //TODO: Some sort of information on how to aggregate this data

            return response;
        }
    }
}