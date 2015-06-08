using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quilt4.Interface;

namespace Quilt4.MongoDBRepository.Entities
{
    class DataBaseInfoEntity : IDataBaseInfo
    {
        public bool Online { get; set; }
        public string Server { get; set; }
        public string Name { get; set; }
    }
}
