using System;
using Quilt4.Interface;

namespace Quilt4.Web
{
    public static class IssueTypeExtensions
    {
        public static bool AreEqual(this Tharga.Quilt4Net.DataTransfer.IssueType item, IIssueType issueType)
        {
            if (item == null && issueType == null) return true;
            if (item.ExceptionTypeName != issueType.ExceptionTypeName) return false;
            if (string.Compare(Clean(item.Message), Clean(issueType.Message), StringComparison.InvariantCultureIgnoreCase) != 0) return false;
            if (string.Compare(Clean(item.StackTrace), Clean(issueType.StackTrace), StringComparison.InvariantCultureIgnoreCase) != 0) return false;
            if (item.IssueLevel.ToIssueLevel() != issueType.IssueLevel) return false;
            if (!item.Inner.AreEqual(issueType.Inner)) return false;
            return true;
        }

        public static bool AreEqual(this Tharga.Quilt4Net.DataTransfer.IssueType item, IInnerIssueType issueType)
        {
            if (item == null && issueType == null) return true;
            if (item.ExceptionTypeName != issueType.ExceptionTypeName) return false;
            if (string.Compare(Clean(item.Message), Clean(issueType.Message), StringComparison.InvariantCultureIgnoreCase) != 0) return false;
            if (string.Compare(Clean(item.StackTrace), Clean(issueType.StackTrace), StringComparison.InvariantCultureIgnoreCase) != 0) return false;
            if (item.IssueLevel != issueType.IssueLevel) return false;
            if (!item.Inner.AreEqual(issueType.Inner)) return false;
            return true;
        }

        private static string Clean(string data)
        {
            if (data == null) return string.Empty;
            return data.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
        }
    }
}