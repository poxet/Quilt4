using System;
using System.Collections.Generic;
using Quilt4.BusinessEntities;
using Quilt4.Interface;

namespace Quilt4.Web.Business
{
    public class SettingsBusiness : ISettingsBusiness
    {
        private readonly IRepository _repository;

        public SettingsBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public ISetting GetSetting(string name)
        {
            var result = _repository.GetSetting(name);
            if (result.Encrypted)
            {
                result = new Setting(result.Name, Decrypt(result.Value), result.Type, result.Encrypted);
            }

            return result;
        }

        private T GetSettingValue<T>(string name, T defaultValue, bool encrypted = false)
        {
            var result = _repository.GetSetting(name);

            if (result == null)
            {
                SetSetting(name, defaultValue.ToString(), typeof(T), encrypted);
                return defaultValue;
            }

            var value = result.Value;
            if (encrypted)
            {
                value = Decrypt(value);
            }

            var response = (T)Convert.ChangeType(value, typeof(T));
            return response;
        }

        public IEnumerable<ISetting> GetAllSettings()
        {
            var result = _repository.GetSettings();
            return result;
        }

        public void SetSetting(string name, string value, Type type, bool encrypt)
        {
            if (encrypt)
            {
                value = Encrypt(value);
            }

            _repository.SetSetting(new Setting(name, value, type.ToString(), encrypt));
        }

        public IEmailSetting GetEmailSetting()
        {
            return new EmailSetting(GetSettingValue("SupportEmailAddress", "info@quilt4net.com"), GetSettingValue("SmtpServerAddress", "smtp.quilt4net.com"), GetSettingValue("SmtpServerPort", 587), GetSettingValue("SendEMailEnabled", false), GetSettingValue("EMailConfirmationEnabled", false), GetSettingValue("SmtpUserName", string.Empty), GetSettingValue("SmtpPassword", string.Empty, true));
        }

        public string GetIssueTypeTicketPrefix()
        {
            return GetSettingValue("IssueTypeTicketPrefix", "A");
        }

        public string GetIssueTicketPrefix()
        {
            return GetSettingValue("IssueTicketPrefix", "B");
        }

        public string GetQuilt4ClientToken()
        {
            return GetSettingValue("Quilt4ClientToken", string.Empty);
        }

        public string GetQuilt4TargetLocation(string defaultLocation)
        {
            return GetSettingValue("Quilt4TargetLocation", defaultLocation);
        }

                public string GetQuilt4TargetLocation(string defaultLocation)
        {
            return GetSettingValue("Quilt4TargetLocation", defaultLocation);
        }

        public void SetEventLogReadDate(DateTime dateTime)
        {
            SetSetting("EventLogReadDate", dateTime.ToShortDateString() + " " + dateTime.ToLongTimeString(), typeof(DateTime), false);
        }

        public DateTime GetEventLogReadDate()
        {
            return GetSettingValue("EventLogReadDate", DateTime.MinValue);
        }

        public void SetEventLogReadDate(DateTime dateTime)
        {
            SetSetting("EventLogReadDate", dateTime.ToShortDateString() + " " + dateTime.ToLongTimeString(), typeof(DateTime), false);
        }

        public DateTime GetEventLogReadDate()
        {
            return GetSettingValue("EventLogReadDate", DateTime.MinValue);
        }

        private static string Encrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var crypto = new Crypto(GetSalt());
            var result = crypto.EncryptStringAes(value, GetSharedSecret());
            return result;
        }

        private static string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var crypto = new Crypto(GetSalt());
            try
            {
                var result = crypto.DecryptStringAes(value, GetSharedSecret());
                return result;
            }
            catch (FormatException)
            {
                return string.Empty;
            }
        }

        private static string GetSalt()
        {
            return System.Configuration.ConfigurationManager.AppSettings["Salt"];
        }

        private static string GetSharedSecret()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SharedSecret"] ?? "Reapadda";
        }
    }
}