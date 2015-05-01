using System.Collections.Generic;
using System.Linq;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class IssueType : IIssueType
    {
        private readonly string _exceptionTypeName;
        private readonly string _message;
        private readonly string _stackTrace;
        private readonly IssueLevel _issueLevel;
        private readonly IInnerIssueType _inner;
        private readonly List<IIssue> _issues;
        private readonly int _ticket;

        public IssueType(string exceptionTypeName, string message, string stackTrace, IssueLevel issueLevel, IInnerIssueType inner, IEnumerable<IIssue> issues, int ticket, string responseMessage)
        {
            _exceptionTypeName = exceptionTypeName;
            _message = message;
            _stackTrace = stackTrace;
            _issueLevel = issueLevel;
            _inner = inner;
            _issues = issues.ToList();
            _ticket = ticket;
            ResponseMessage = responseMessage;
        }

        public string ExceptionTypeName { get { return _exceptionTypeName; } }
        public string Message { get { return _message; } }
        public string StackTrace { get { return _stackTrace; } }
        public IssueLevel IssueLevel { get { return _issueLevel; } }
        public IInnerIssueType Inner { get { return _inner; } }
        public IEnumerable<IIssue> Issues { get { return _issues; } }
        public int Ticket { get { return _ticket; } }
        public string ResponseMessage { get; set; }

        public void Add(IIssue issue)
        {
            _issues.Add(issue);
        }
    }
}