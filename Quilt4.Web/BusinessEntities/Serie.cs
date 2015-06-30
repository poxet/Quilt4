using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.Web.BusinessEntities
{
    public class Serie : ISerie
    {
        public Serie(string counterName, Dictionary<string, object> data)
        {
            CounterName = counterName;
            Data = data;
        }

        public string CounterName { get; private set; }
        public Dictionary<string, object> Data { get; private set; }
    }
}