using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Quilt4.Interface;

namespace Quilt4.Web.Business
{
    public interface IIssueBusiness
    {
        ILogResponse RegisterIssue(Exception exception, IssueLevel warning);
        ISession GetSession(Guid id);
        void UpdateApplicationVersion(IApplicationVersion applicationVersion);
        IApplicationVersion GetApplicationVersion(string applicationVersionFingerprint);
        IInitiative GetInitiativeByApplication(Guid applicationId);
    }

    public interface ISettingsBusiness
    {
        T GetSetting<T>(string name);
    }

    public interface ICompatibilityBusiness
    {
        void RegisterToolkitCompability(Version version, DateTime utcNow, string supportToolkitNameVersion, object o);
    }

    static class HelperExtension
    {
        public static string ToMd5Hash(this string input)
        {
            var inputBytes = Encoding.Default.GetBytes(input);
            var provider = new MD5CryptoServiceProvider();
            var hash = provider.ComputeHash(inputBytes);
            return hash.Aggregate(string.Empty, (current, b) => current + b.ToString("X2"));
        }
    }
}
