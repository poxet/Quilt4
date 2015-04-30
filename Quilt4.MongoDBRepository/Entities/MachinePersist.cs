using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;

namespace Quilt4.MongoDBRepository.Entities
{
    internal class MachinePersist : ISupportInitialize
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public IDictionary<string, string> Data { get; internal set; }

        [BsonExtraElements]
        private IDictionary<string, object> ExtraElements { get; set; }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (ExtraElements != null)
            {
                var osName = (string)ExtraElements["OsName"];
                ExtraElements.Remove("OsName");

                if (Data == null)
                    Data = new Dictionary<string, string>();

                if (!Data.ContainsKey("OsName"))
                    Data.Add("OsName", osName);

                MongoRepository.InvokeRequestUpdateEntityEvent(new RequestUpdateEntityEventArgs("Machine", this));
            }
        }
    }
}