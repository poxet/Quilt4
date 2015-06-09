using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Business
{
    public class EmailBusiness : IEmailBusiness
    {
        private readonly IRepository _repository;

        public EmailBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public void SendEmail(IEnumerable<string> tos, string subject, string body)
        {
            var smtpServerAdress = ConfigurationManager.AppSettings["SmtpServerAddress"];
            var smtpServerPort = ConfigurationManager.AppSettings["SmtpServerPort"];
            var mailFrom = ConfigurationManager.AppSettings["SupportEmailAddress"];

            int portNumber;
            int.TryParse(smtpServerPort, out portNumber);

            var smtpClient = new SmtpClient(smtpServerAdress, portNumber);
            
            smtpClient.Credentials = new NetworkCredential("daniel.bohlin@quilt4net.com", "All4One!");

            foreach (var to in tos)
            {
                var mailMessage = new MailMessage(mailFrom, to, subject, body);

                _repository.LogEmail(mailFrom, to, subject, body, DateTime.Now);

                smtpClient.Send(mailMessage);
            }
        }

        public IEnumerable<IEmail> GetLastHundredEmails()
        {
            var emails = _repository.GetLastHundredEmails();

            return emails.ToEmails();
        }


    }

    public static class EmailExtensions
    {
        public static IEnumerable<EmailViewModel> ToEmails(this IEnumerable<IEmail> emails)
        {
            foreach (var item in emails)
            {
                yield return new EmailViewModel
                {

                    ToEmail = item.ToEmail,
                    FromEmail = item.FromEmail,
                    Subject = item.Subject,
                    Body = item.Body,
                    DateSent = item.DateSent,
                };
            }
        }


    }


}