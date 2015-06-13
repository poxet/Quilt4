using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quilt4.Interface
{
    public interface IDataBaseInfo
    {
        bool Online { get; set; }
        string Server { get; set; }
        string Name { get; set; }
    }
}
