using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class CounterLine : ICounterLine
    {
        public CounterLine(string key, int[] counts)
        {
            Key = key;
            Counts = counts;
        }

        public string Key { get; private set; }
        public int[] Counts { get; private set; }
    }
}