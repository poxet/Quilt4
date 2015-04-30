using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;

namespace Quilt4.MongoDBRepository.Entities
{
    internal class IssueTypePersist : ISupportInitialize
    {
        public string ExceptionTypeName { get; internal set; }
        public string Message { get; internal set; }
        public string StackTrace { get; internal set; }
        public string IssueLevel { get; internal set; }
        public InnerIssueTypePersist Inner { get; internal set; }
        public IEnumerable<IssuePersist> Issues { get; internal set; }
        public int Ticket { get; internal set; }
        public string ResponseMessage { get; internal set; }

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