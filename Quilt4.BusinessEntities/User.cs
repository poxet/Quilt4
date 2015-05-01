using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class User : IUser
    {
        private readonly string _fingerprint;
        private readonly string _userName;

        public User(string fingerprint, string userName)
        {
            _fingerprint = fingerprint;
            _userName = userName;
        }

        public string Id { get { return _fingerprint; } }
        public string UserName { get { return _userName; } }
    }
}