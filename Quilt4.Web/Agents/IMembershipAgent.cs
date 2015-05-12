using Quilt4.Interface;

namespace Quilt4.Web.Agents
{
    public interface IMembershipAgent
    {
        //IMembershipUser GetDeveloper(string currentUserName);
        bool IsEMailConfirmed(string developerName);
        string GetUserHostAddress();
    }
}