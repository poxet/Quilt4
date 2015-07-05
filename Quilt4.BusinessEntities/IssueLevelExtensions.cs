using System;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public static class IssueLevelExtensions
    {
        public static IssueLevel ToIssueLevel(this string issueLevel)
        {
            IssueLevel il;
            if (!Enum.TryParse(issueLevel.Replace("Message", string.Empty).Replace("Exception", string.Empty), true, out il))
            {
                //throw new ArgumentException(string.Format("Invalid value for IssueLevel. Use one of the following; Information, Warning or Error.")).AddData("IssueLevel", issueLevel);
                throw new ArgumentException(string.Format("Invalid value for IssueLevel. Use one of the following; Information, Warning or Error."));
            }
            return il;
        }
    }
}