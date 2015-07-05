namespace Quilt4.Interface
{
    public interface ILogResponse
    {
        string IssueInstanceTicket { get; }
        string IssueTypeTicket { get; }
        string ResponseMessage { get; }
    }
}