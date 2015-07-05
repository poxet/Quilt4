using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class LogResponse : ILogResponse
    {
        public LogResponse(string issueInstanceTicket, string issueTypeTicket, string responseMessage)
        {
            IssueInstanceTicket = issueInstanceTicket;
            IssueTypeTicket = issueTypeTicket;
            ResponseMessage = responseMessage;
        }

        public string IssueInstanceTicket { get; private set; }
        public string IssueTypeTicket { get; private set; }
        public string ResponseMessage { get; private set; }
    }
}