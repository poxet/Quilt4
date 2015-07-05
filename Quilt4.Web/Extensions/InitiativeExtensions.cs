using System.Collections.Generic;
using System.Linq;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web
{
    public static class InitiativeExtensions
    {
        public static string GetUniqueIdentifier(this IInitiativeHead item, IEnumerable<string> names)
        {
            //First use name if possible
            if (names != null && names.Count(x => (x ?? Constants.DefaultInitiativeName) == (item.Name ?? Constants.DefaultInitiativeName)) == 1)
            {
                return item.Name ?? Constants.DefaultInitiativeName;
            }

            return item.Id.ToString();
        }

        public static string GetUniqueIdentifier(this IssueViewModel item, string name)
        {
            //First use name if possible
            if (name != null)
            {
                return item.Version;
            }

            return item.ApplicationVersionId;
        }
        public static string GetUniqueIdentifier(this IApplicationVersion item, IEnumerable<string> versions)
        {
            //First use name if possible
            if (versions != null && versions.Count(x => (x ?? Constants.DefaultVersionName) == (item.Version ?? Constants.DefaultVersionName)) == 1)
            {
                return item.Version ?? Constants.DefaultVersionName;
            }

            return item.Id.Replace(":", string.Empty);
        }
    }
}