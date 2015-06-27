namespace Quilt4.Interface
{
    public interface IEmailSetting
    {
        string SupportEmailAddress { get; }
        string SmtpServerAdress { get; }
        int SmtpServerPort { get; }
        bool SendEMailEnabled { get; }
        bool EMailConfirmationEnabled { get; }
        string Username { get; }
        string Password { get; }
    }
}