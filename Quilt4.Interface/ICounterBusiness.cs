using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface ICounterBusiness
    {
        void Register(string counterName, int count, Dictionary<string, string> data);
    }
}