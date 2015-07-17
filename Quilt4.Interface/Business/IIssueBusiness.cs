using System;
using System.Collections.Generic;
using Tharga.Quilt4Net.DataTransfer;

namespace Quilt4.Interface
{
    public interface IIssueBusiness
    {
        ILogResponse RegisterIssue(Exception exception, IssueLevel issueLevel);
        RegisterIssueResponse RegisterIssue(RegisterIssueRequest data);
        IEnumerable<IIssue> GetFiveLatestErrorIssusByIssueTypes(IEnumerable<IIssueType> issueTypes);
        IEnumerable<IIssueType> GetIssueTypesForDeveloper(string userEmail);
    }
}