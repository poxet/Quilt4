using System.Collections.Generic;
using System.Linq;
using Quilt4.BusinessEntities;
using Quilt4.Interface;

namespace Quilt4.Web.Business
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IRepository _repository;

        public UserBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<IUser> GetUsersByApplicationVersion(string applicationVersionId)
        {
            var users = _repository.GetUsersByApplicationVersion(applicationVersionId).OrderBy(x => x.UserName);
            return users;
        }

        public void RegisterUser(IFingerprint id, string userName)
        {
            var user = _repository.GetUser((Fingerprint)id);
            if (user == null)
                _repository.AddUser(new User((Fingerprint)id, userName));
        }

        public IUser GetUser(string userFingerprint)
        {
            return _repository.GetUser(userFingerprint);
        }
    }
}