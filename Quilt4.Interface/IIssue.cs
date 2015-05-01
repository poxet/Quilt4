using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IIssue
    {
        Guid Id { get; }
        DateTime ClientTime { get; }
        DateTime ServerTime { get; }
        bool? VisibleToUser { get; }
        IDictionary<string, string> Data { get; }
        Guid? IssueThreadGuid { get; }
        string UserHandle { get; }
        string UserInput { get; }
        int Ticket { get; }
        Guid SessionId { get; }
    }
}