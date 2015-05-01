using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class InnerIssueType : IInnerIssueType
    {
        private readonly string _exceptionTypeName;
        private readonly string _message;
        private readonly string _stackTrace;
        private readonly string _issueLevel;
        private readonly IInnerIssueType _innerIssueType;

        public InnerIssueType(string exceptionTypeName, string message, string stackTrace, string issueLevel, IInnerIssueType innerIssueType)
        {
            _exceptionTypeName = exceptionTypeName;
            _message = message;
            _stackTrace = stackTrace;
            _issueLevel = issueLevel;
            _innerIssueType = innerIssueType;
        }

        public string ExceptionTypeName { get { return _exceptionTypeName; } }
        public string Message { get { return _message; } }
        public string StackTrace { get { return _stackTrace; } }
        public string IssueLevel { get { return _issueLevel; } }
        public IInnerIssueType Inner { get { return _innerIssueType; } }
    }
}