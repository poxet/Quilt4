using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quilt4.Interface;

namespace Quilt4.MongoDBRepository.Entities
{
    public class EmailLogPersist : IEmail
    {
        public string ToEmail { get; set; }
        public string FromEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime DateSent { get; set; }
        public Guid Id { get; set; }
        public bool Status { get; set; }
    }
}
