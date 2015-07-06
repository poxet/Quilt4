using System;
using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;

namespace Quilt4.MongoDBRepository.Entities
{
    internal class ApplicationVersionPersist : ISupportInitialize
    {
        public string Id { get; internal set; } //ApplicationVersionId
        public Guid ApplicationId { get; internal set; } //ApplicationId
        public string Version { get; internal set; }
        public IEnumerable<IssueTypePersist> IssueTypes { get; internal set; }
        public string ResponseMessage { get; internal set; }
        public bool IsOfficial { get; internal set; }
        public bool Ignore { get; internal set; }
        public string SupportToolkitNameVersion { get; internal set; }
        public DateTime? BuildTime { get; internal set; }

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