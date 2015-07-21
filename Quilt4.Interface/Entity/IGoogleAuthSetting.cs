using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quilt4.Interface
{
    public interface IGoogleAuthSetting
    {
        string ClientId { get; set; }
        string ClientSecret { get; set; }
    }
}
