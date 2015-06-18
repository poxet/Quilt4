using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class CounterCollection : ICounterCollection
    {
        public CounterCollection(string[] names, ICounterLine[] lines)
        {
            Names = names;
            Lines = lines;
        }

        public string[] Names { get; private set; }
        public ICounterLine[] Lines { get; private set; }
    }
}