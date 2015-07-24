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
        string ResponseMessage { get; }        
        [Obsolete("This feature is to be removed")]
        bool IsOfficial { get; }
        bool Ignore { get; }
        string SupportToolkitNameVersion { get; }
        DateTime? BuildTime { get; }
        List<string> Environments { get; set; } 

        //TODO: REFACTOR: Move thees functions to the business layer
        void Add(IIssueType issueType);
    }
}