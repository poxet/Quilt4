using System;

namespace Quilt4.Interface
{
    public interface IEmail
    {
        string FromEmail { get; set; }
        string ToEmail { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        DateTime DateSent { get; set; }
        bool Status { get; set; }
        string ErrorMessage { get; set; }
    }
}