using System;

namespace Quilt4.Interface
{
    public interface IEmail
    {
        string FromEmail { get; }
        string ToEmail { get; }
        string Subject { get; }
        string Body { get; }
        DateTime DateSent { get; }
        bool Status { get; }
        string ErrorMessage { get; }
    }
}