using System;
using Quilt4.Interface;

namespace Quilt4.MongoDBRepository.Entities
{
    internal class EmailLogPersist : IEmail
    {
        public string ToEmail { get; internal set; }
        public string FromEmail { get; internal set; }
        public string Subject { get; internal set; }
        public string Body { get; internal set; }
        public DateTime DateSent { get; internal set; }
        public Guid Id { get; internal set; }
        public bool Status { get; internal set; }
        public string ErrorMessage { get; internal set; }
    }
}