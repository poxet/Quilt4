using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Business
{
    public class EmailBusiness : IEmailBusiness
    {
        private readonly IRepository _repository;
        private readonly ISettingsBusiness _settingsBusiness;

        public EmailBusiness(IRepository repository, ISettingsBusiness settingsBusiness)
        {
            _repository = repository;
            _settingsBusiness = settingsBusiness;
        }

        public void SendEmail(IEnumerable<string> tos, string subject, string body)
        {
            var emailSetting = _settingsBusiness.GetEmailSetting();

            if (!emailSetting.SendEMailEnabled)
                return;

            var smtpClient = new SmtpClient(emailSetting.SmtpServerAdress, emailSetting.SmtpServerPort);
            if (!string.IsNullOrEmpty(emailSetting.Username))
                smtpClient.Credentials = new NetworkCredential(emailSetting.Username, emailSetting.Password);

            var errorMessage = string.Empty;

            foreach (var to in tos)
            {
                var status = false;

                try
                {
                    var mailMessage = new MailMessage(emailSetting.SupportEmailAddress, to, subject, body) { IsBodyHtml = true };
                    smtpClient.Send(mailMessage);
                    status = true;
                }
                catch (Exception exception)
                {
                    errorMessage = exception.Message;
                    throw;
                }
                finally
                {
                    _repository.LogEmail(emailSetting.SupportEmailAddress, to, subject, body, DateTime.Now, status, errorMessage);
                }
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
                    Status = item.Status,
                    ErrorMessage = item.ErrorMessage,
                };
            }
        }
    }
}