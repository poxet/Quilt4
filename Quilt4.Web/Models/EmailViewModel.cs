using System;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class EmailViewModel : IEmail
    {
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime DateSent { get; set; }
        public bool Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}