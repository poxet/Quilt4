using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quilt4.Web.Areas.Admin.Models
{
    public class InviteModel
    {
        public string InviteEmail { get; set; }
        public string InitiativeName { get; set; }
        public IEnumerable<Quilt4.Interface.IDeveloperRole> Members { get; set; }

    }
}