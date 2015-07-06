using System.Collections.Generic;

namespace Quilt4.MongoDBRepository.Entities
{
    internal class ApplicationGroupPersist
    {
        public string Name { get; internal set; }
        public IEnumerable<ApplicationPersist> Applications { get; internal set; }
    }
}