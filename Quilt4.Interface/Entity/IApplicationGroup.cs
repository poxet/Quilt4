using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IApplicationGroup
    {
        string Name { get; }
        IEnumerable<IApplication> Applications { get; }

        //TODO: REFACTOR: Move thees functions to the business layer
        void Add(IApplication application);
        void Remove(IApplication application);
    }
}