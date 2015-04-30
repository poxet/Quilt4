using System;
using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;

namespace Quilt4.MongoDBRepository.Entities
{
    internal class SessionPersist : ISupportInitialize
    {
        public Guid Id { get; internal set; }
        public string ApplicationVersionId { get; internal set; }
        public string Environment { get; internal set; }
        public Guid ApplicationId { get; internal set; }
        public string MachineFingerprint { get; internal set; }
        public string UserFingerprint { get; internal set; }
        public DateTime ClientStartTime { get; internal set; }
        public DateTime ServerStartTime { get; internal set; }
        public DateTime? ServerEndTime { get; internal set; }
        public DateTime? ServerLastKnown { get; internal set; }
        public string CallerIp { get; internal set; }

        [BsonExtraElements]
        private IDictionary<string, object> ExtraElements { get; set; }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (ExtraElements != null)
            {
                //if (ExtraElements.ContainsKey("MachineId"))
                //{
                //    MachineFingerprint = MachineFingerprint ?? ExtraElements["MachineId"] as string;
                //    ExtraElements.Remove("MachineId");
                //}

                //if (ExtraElements.ContainsKey("UserId"))
                //{
                //    UserFingerprint = UserFingerprint ?? ExtraElements["UserId"] as string;
                //    ExtraElements.Remove("UserId");
                //}

                ////TODO: How do I perform an update of the converted object here.
                //Guid applicationGuid;
                //if (!Guid.TryParse(ApplicationId, out applicationGuid))
                //{
                //    MongoRepository.InvokeRequestDeleteEntityEvent(new RequestDeleteEntityEventArgs("Session", Id));
                //}
                //else
                //{
                //    MongoRepository.InvokeRequestUpdateEntityEvent(new RequestUpdateEntityEventArgs("Session", this));
                //}
            }
        }
    }
}