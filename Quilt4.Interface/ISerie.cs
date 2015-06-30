using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface ISerie
    {
        string CounterName { get; }
        Dictionary<string,object> Data { get; }
    }
}