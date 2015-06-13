using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IEmailBusiness
    {
        void SendEmail(IEnumerable<string> tos, string subject, string body);

        IEnumerable<IEmail> GetLastHundredEmails();
    }
}