using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Quilt4.Interface;

namespace Quilt4.Web.Business
{
    public class IssueBusiness : IIssueBusiness
    {
        public ILogResponse RegisterIssue(Exception exception, IssueLevel warning)
        {
            throw new NotImplementedException();
        }

        public ISession GetSession(Guid id)
        {
            throw new NotImplementedException();
        }

        public void UpdateApplicationVersion(IApplicationVersion applicationVersion)
        {
            throw new NotImplementedException();
        }

        public IApplicationVersion GetApplicationVersion(string applicationVersionFingerprint)
        {
            throw new NotImplementedException();
        }

        public IInitiative GetInitiativeByApplication(Guid applicationId)
        {
            throw new NotImplementedException();
        }
    }

    public class SettingsBusiness : ISettingsBusiness
    {
        public T GetSetting<T>(string name)
        {
            return default(T);
        }
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
