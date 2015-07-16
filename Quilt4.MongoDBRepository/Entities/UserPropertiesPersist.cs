using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quilt4.MongoDBRepository.Entities
{
    class UserPropertiesPersist
    {
        public string Id { get; internal set; }
        public IDictionary<string, string> EnvironmentColors { get; internal set; }
    }
}
