using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quilt4.Web.Models
{
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
}