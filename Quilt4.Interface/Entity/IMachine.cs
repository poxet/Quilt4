using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IMachine
    {
        string Id { get; }
        string Name { get; }
        IDictionary<string, string> Data { get; }
    }
}