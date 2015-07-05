namespace Quilt4.Interface
{
    public interface IMembershipAgent
    {
        //IMembershipUser GetDeveloper(string currentUserName);
        bool IsEMailConfirmed(string developerName);
        string GetUserHostAddress();
    }
}