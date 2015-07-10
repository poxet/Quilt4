using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IEmailBusiness
    {
        bool SendEmail(IEnumerable<string> tos, string subject, string body);
        IEnumerable<IEmail> GetLastHundredEmails();
    }
}