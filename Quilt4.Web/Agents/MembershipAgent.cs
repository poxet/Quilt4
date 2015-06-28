using System;
using System.Diagnostics;
using System.Web;

namespace Quilt4.Web.Agents
{
    public class MembershipAgent : IMembershipAgent
    {
        //public IMembershipUser GetDeveloper()
        //{
        //    //if (string.IsNullOrEmpty(WebSecurity.CurrentUserName))
        //    //    return new MembershipUser(null, null);

        //    //using (var context = new UsersContext())
        //    //{
        //    //    var userData = context.UserData.Single(x => x.UserName == WebSecurity.CurrentUserName);
        //    //    return new MembershipUser(userData.UserName, userData.Email);
        //    //}
        //    throw new NotImplementedException();
        //}

        //public bool IsEMailConfirmed(string developerName)
        //{
        //    //if (!Settings.UseAuthorization())
        //    //    return true;

        //    //if (!Settings.EMailConfirmationEnabled())
        //    //    return true;

        //    //using (var context = new UsersContext())
        //    //{
        //    //    var userData = context.UserData.Single(x => x.UserName == developerName);
        //    //    return userData.EMailConfirmed;
        //    //}
        //    throw new NotImplementedException();
        //}

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
}