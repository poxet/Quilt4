using System;
using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;
using Quilt4.Interface;

namespace Quilt4.MongoDBRepository.Entities
{
    internal class ToolkitCompabilityPersist : ISupportInitialize
    {
        public Guid Id { get; internal set; }
        public string ServerVersion { get; internal set; }
        public DateTime RegisterDate { get; internal set; }
        public string SupportToolkitNameVersion { get; internal set; }
        public int Compatibility { get; internal set; }

        [BsonExtraElements]
        private IDictionary<string, object> ExtraElements { get; set; }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (ExtraElements != null)
            {
                Compatibility = (bool)ExtraElements["Compatible"] ? (int)ECompatibility.Compable : (int)ECompatibility.Incompatible;
                ExtraElements.Remove("Compatible");

                MongoRepository.InvokeRequestUpdateEntityEvent(new RequestUpdateEntityEventArgs("ToolkitCompability", this));
            }
        }
    }
}