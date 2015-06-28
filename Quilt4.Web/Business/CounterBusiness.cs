using System.Collections.Generic;
using Quilt4.Interface;
using Quilt4.Web.BusinessEntities;

namespace Quilt4.Web.Business
{
    public class CounterBusiness : ICounterBusiness
    {
        private readonly IInfluxDbAgent _influxDbAgent;

        public CounterBusiness(IInfluxDbAgent influxDbAgent)
        {
            _influxDbAgent = influxDbAgent;
        }

        public void Register(string counterName, int count, Dictionary<string, string> data)
        {
            _influxDbAgent.WriteAsync(new Serie());
        }
    }
}