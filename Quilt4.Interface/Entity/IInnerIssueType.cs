namespace Quilt4.Interface
{
    public interface IInnerIssueType
    {
        string ExceptionTypeName { get; }
        string Message { get; }
        string StackTrace { get; }
        string IssueLevel { get; }
        IInnerIssueType Inner { get; }
    }
}