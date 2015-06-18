using System;
using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class Counter : ICounter
    {
        public Counter(string counterName, DateTime dateTime, decimal? duration, int count, Dictionary<string, string> path, Dictionary<string, string> data, string level, string environment)
        {
            CounterName = counterName;
            DateTime = dateTime;
            Duration = duration;
            Count = count;
            Path = path;
            Data = data;
            Level = level;
            Environment = environment;
        }

        public string CounterName { get; private set; }
        public DateTime DateTime { get; private set; }
        public decimal? Duration { get; private set; }
        public int Count { get; private set; }
        public Dictionary<string, string> Path { get; private set; }
        public Dictionary<string, string> Data { get; private set; }
        public string Level { get; private set; }
        public string Environment { get; private set; }
    }
}