using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quilt4.Interface;

namespace Quilt4.Web.BusinessEntities
{
    public class DataBaseInfo : IDataBaseInfo
    {
        public bool Online { get; set; }
        public string Server { get; set; }
        public string Name { get; set; }
    }
}