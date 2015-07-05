namespace Quilt4.Interface
{
    public interface IUserBusiness
    {
        void RegisterUser(IFingerprint id, string userName);
        IUser GetUser(string userFingerprint);
    }
}