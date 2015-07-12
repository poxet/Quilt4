using System;
using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;
using Quilt4.MongoDBRepository.Data;

namespace Quilt4.MongoDBRepository.Entities
{
    internal class ApplicationPersist : ISupportInitialize
    {
        public Guid Id { get; internal set; }
        public string Name { get; internal set; }
        public DateTime FirstRegistered { get; internal set; }
        public string TicketPrefix { get; internal set; }

        [BsonExtraElements]
        private IDictionary<string, object> ExtraElements { get; set; }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (ExtraElements != null)
            {
                if (ExtraElements.ContainsKey("DevColor"))
                    ExtraElements.Remove("DevColor");

                if (ExtraElements.ContainsKey("ProdColor"))
                    ExtraElements.Remove("ProdColor");

                if (ExtraElements.ContainsKey("CiColor"))
                    ExtraElements.Remove("CiColor");

                MongoRepository.InvokeRequestUpdateEntityEvent(new RequestUpdateEntityEventArgs("Application", this));
            }
        }
    }
}