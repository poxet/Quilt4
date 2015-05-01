using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IApplicationVersion
    {
        string Id { get; }
        Guid ApplicationId { get; }
        string Version { get; }
        IEnumerable<IIssueType> IssueTypes { get; }
        string ResponseMessage { get; set; }
        bool IsOfficial { get; set; }
        bool Ignore { get; set; }
        string SupportToolkitNameVersion { get; }
        DateTime? BuildTime { get; }

        void Add(IIssueType issueType);
    }
}