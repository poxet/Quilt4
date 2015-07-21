using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class GoogleAuthSetting : IGoogleAuthSetting
    {
        public GoogleAuthSetting(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
