using Quilt4.BusinessEntities;
using Quilt4.Interface;

namespace Quilt4.Web.Business
{
    public class SystemBusiness : ISystemBusiness
    {
        private readonly IRepository _repository;

        public SystemBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public IDataBaseInfo GetDataBaseStatus()
        {
            var dbInfo = _repository.GetDatabaseStatus();

            var newDbInfo = new DataBaseInfo
            {
                Online = dbInfo.Online,
                Name = dbInfo.Name,
                Server = dbInfo.Server
            };

            return newDbInfo;
        }
    }
}