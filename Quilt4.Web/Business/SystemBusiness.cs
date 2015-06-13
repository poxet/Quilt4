using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quilt4.Interface;
using Quilt4.Web.BusinessEntities;

namespace Quilt4.Web.Business
{
    public class SystemBusiness
    {
        private IRepository _repository;

        public SystemBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public DataBaseInfo GetDataBaseStatus()
        {
            var dbInfo = _repository.GetDatabaseStatus();

            var newDbInfo = new DataBaseInfo();

            newDbInfo.Online = dbInfo.Online;
            newDbInfo.Name = dbInfo.Name;
            newDbInfo.Server = dbInfo.Server;

            return newDbInfo;
        }
    }
}