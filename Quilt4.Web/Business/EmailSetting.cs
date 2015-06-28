using Quilt4.Interface;

namespace Quilt4.Web.Business
{
    public class EmailSetting : IEmailSetting
    {
        public EmailSetting(string supportEmailAddress, string smtpServerAdress, int smtpServerPort, bool sendEMailEnabled, bool eMailConfirmationEnabled, string username, string password)
        {
            SupportEmailAddress = supportEmailAddress;
            SmtpServerAdress = smtpServerAdress;
            SmtpServerPort = smtpServerPort;
            SendEMailEnabled = sendEMailEnabled;
            EMailConfirmationEnabled = eMailConfirmationEnabled;
            Username = username;
            Password = password;
        }

        public string SupportEmailAddress { get; private set; }
        public string SmtpServerAdress { get; private set; }
        public int SmtpServerPort { get; private set; }
        public bool SendEMailEnabled { get; private set; }
        public bool EMailConfirmationEnabled { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
    }
}