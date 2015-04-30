using System;
using System.Diagnostics;
using System.Web;
using System.Web.Security;
using Quilt4.Interface;

namespace Quilt4.Web.Agents
{
    public class MembershipAgent : IMembershipAgent
    {
        public IMembershipUser GetDeveloper()
        {
            //if (string.IsNullOrEmpty(WebSecurity.CurrentUserName))
            //    return new MembershipUser(null, null);

            //using (var context = new UsersContext())
            //{
            //    var userData = context.UserData.Single(x => x.UserName == WebSecurity.CurrentUserName);
            //    return new MembershipUser(userData.UserName, userData.Email);
            //}
            throw new NotImplementedException();
        }

        public bool IsEMailConfirmed(string developerName)
        {
            //if (!Settings.UseAuthorization())
            //    return true;

            //if (!Settings.EMailConfirmationEnabled())
            //    return true;

            //using (var context = new UsersContext())
            //{
            //    var userData = context.UserData.Single(x => x.UserName == developerName);
            //    return userData.EMailConfirmed;
            //}
            throw new NotImplementedException();
        }

        public string GetUserHostAddress()
        {
            try
            {
                var callerIp = HttpContext.Current.Request.UserHostAddress;
                return callerIp;
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.Message);
                return "Unknown";
            }
        }
    }

    public interface IMembershipAgent
    {
        IMembershipUser GetDeveloper();
        bool IsEMailConfirmed(string developerName);
        string GetUserHostAddress();
    }

    public interface ILogAgent
    {
        ILogResponse RegisterIssue(Exception exception, IssueLevel warning);
    }

    public class LogAgent : ILogAgent
    {
        public ILogResponse RegisterIssue(Exception exception, IssueLevel warning)
        {
            //TODO: Refactor
            throw new NotImplementedException();
            //try
            //{
            //    var response = Issue.Register(exception, Issue.ExceptionIssueLevel.Warning);
            //    return new LogResponse
            //    {
            //        IssueInstanceTicket = response.IssueInstanceTicket,
            //        IssueTypeTicket = response.IssueTypeTicket,
            //        ResponseMessage = response.ResponseMessage,
            //    };
            //}
            //catch (Exception exp)
            //{
            //    try
            //    {
            //        //TODO: PRIO: Get the innter exception here too!
            //        EventLog.WriteEntry(Interface.Constants.EventLogName, exp.Message, EventLogEntryType.Error);
            //    }
            //    catch (System.Security.SecurityException exp2)
            //    {
            //        return new LogResponse { ResponseMessage = exp2.Message + " (" + exp.Message + ")" };
            //    }
            //    return new LogResponse { ResponseMessage = exp.Message };
            //}
        }
    }
}