using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;

namespace Quilt4.MongoDBRepository.Entities
{
    internal class SettingPersist : ISupportInitialize
    {
        public string Id { get; internal set; }
        public string Value { get; internal set; }
        public string Type { get; internal set; }
        public bool Encrypted { get; internal set; }

        [BsonExtraElements]
        private IDictionary<string, object> ExtraElements { get; set; }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (ExtraElements != null)
            {
                //if (ExtraElements.ContainsKey("OwnerDeveloperName"))
                //{
                //    OwnerDeveloperName = OwnerDeveloperName ?? ExtraElements["OwnerDeveloperName"] as string;
                //    ExtraElements.Remove("OwnerDeveloperName");
                //}

                //if (ExtraElements.ContainsKey("Members"))
                //{
                //    DeveloperRoles = DeveloperRoles ?? ExtraElements["Members"] as IEnumerable<DeveloperRolePersist> ?? new List<ApplicationGroupPersist>() as IEnumerable<DeveloperRolePersist>;
                //    ExtraElements.Remove("Members");
                //}

                //MongoRepository.InvokeRequestUpdateEntityEvent(new RequestUpdateEntityEventArgs("Initiative", this));
            }
        }
    }
}