using System;
using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;

namespace Quilt4.MongoDBRepository.Entities
{
    internal class IssuePersist : ISupportInitialize
    {
        public Guid Id { get; internal set; }
        public DateTime ClientTime { get; internal set; }
        public DateTime ServerTime { get; internal set; }
        public bool? VisibleToUser { get; internal set; }
        public IDictionary<string, string> Data { get; internal set; }
        public Guid? IssueThreadGuid { get; internal set; }
        public string UserHandle { get; internal set; }
        public string UserInput { get; internal set; }
        public int Ticket { get; internal set; }
        public Guid SessionId { get; internal set; }

        [BsonExtraElements]
        private IDictionary<string, object> ExtraElements { get; set; }

        public void BeginInit()
        {            
        }

        public void EndInit()
        {
            if (ExtraElements != null)
            {
            }
        }
    }
}