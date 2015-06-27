//using System;
//using Quilt4.Interface;

//namespace Quilt4.Web.Agents
//{
//    public class LogAgent : ILogAgent
//    {
//        public ILogResponse RegisterIssue(Exception exception, IssueLevel issueLevel)
//        {
//            //TODO: Refactor
//            throw new NotImplementedException();
//            //try
//            //{
//            //    var response = Issue.Register(exception, Issue.ExceptionIssueLevel.Warning);
//            //    return new LogResponse
//            //    {
//            //        IssueInstanceTicket = response.IssueInstanceTicket,
//            //        IssueTypeTicket = response.IssueTypeTicket,
//            //        ResponseMessage = response.ResponseMessage,
//            //    };
//            //}
//            //catch (Exception exp)
//            //{
//            //    try
//            //    {
//            //        //TODO: PRIO: Get the innter exception here too!
//            //        EventLog.WriteEntry(Interface.Constants.EventLogName, exp.Message, EventLogEntryType.Error);
//            //    }
//            //    catch (System.Security.SecurityException exp2)
//            //    {
//            //        return new LogResponse { ResponseMessage = exp2.Message + " (" + exp.Message + ")" };
//            //    }
//            //    return new LogResponse { ResponseMessage = exp.Message };
//            //}
//        }
//    }
//}