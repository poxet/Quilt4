using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface ISettingsBusiness
    {
        ISetting GetSetting(string name);
        IEnumerable<ISetting> GetAllSettings();
        void SetSetting(string name, string value, Type type, bool encrypt);
        IEmailSetting GetEmailSetting();
        string GetIssueTypeTicketPrefix();
        string GetIssueTicketPrefix();
        string GetQuilt4ClientToken();
        string GetQuilt4TargetLocation(string defaultLocation);
        IInfluxDbSetting GetInfluxDBSetting();
        void SetEventLogReadDate(DateTime dateTime);
        DateTime GetEventLogReadDate();
    }
}