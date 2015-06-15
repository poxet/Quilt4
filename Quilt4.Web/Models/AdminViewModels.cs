using System;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class AdminIndexViewModel
    {
        public int InitiativeCount { get; set; }
        public int ApplicationCount { get; set; }
        public int IssueTypeCount { get; set; }
        public int IssueCount { get; set; }
    }

    public class AdminViewModels
    {
        public string DBType { get; set; }
        public bool DBOnline { get; set; }
        public string DBName { get; set; }
        public string DBServer { get; set; }

        public string SupportEmailAdress { get; set; }
        public string SmtpServerAdress { get; set; }
        public string SmtpServerPort { get; set; }
        public string SendEMailEnabled { get; set; }
        public string EMailConfirmationEnabled { get; set; }
    }

    public class SendEmailViewModel
    {
        public string ToEmail { get; set; }
        public bool EmailEnabled { get; set; }

    }

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