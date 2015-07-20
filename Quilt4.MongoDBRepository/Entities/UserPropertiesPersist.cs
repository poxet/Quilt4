using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Quilt4.MongoDBRepository.Entities
{
    class UserPropertiesPersist : ISupportInitialize
    {
        public string Id { get; internal set; }
        public IDictionary<string, string> EnvironmentColors { get; internal set; }

        [BsonExtraElements]
        private IDictionary<string, object> ExtraElements { get; set; }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (ExtraElements != null)
            {
                if (ExtraElements.ContainsKey("Username"))
                {
                    ExtraElements.Remove("Username");
                }
            }
        }
    }
}
