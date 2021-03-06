﻿using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IIssueType
    {
        string ExceptionTypeName { get; }
        string Message { get; }
        string StackTrace { get; }
        IssueLevel IssueLevel { get; }
        IInnerIssueType Inner { get; }
        IEnumerable<IIssue> Issues { get; }
        int Ticket { get; }
        string ResponseMessage { get; set; }

        //TODO: REFACTOR: Move thees functions to the business layer
        void Add(IIssue issue);
    }
}