using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IApplicationGroup
    {
        string Name { get; }
        IEnumerable<IApplication> Applications { get; }

        void Add(IApplication application);
        void Remove(IApplication application);
    }
}