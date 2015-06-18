using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quilt4.Web.Models
{
    public class ApplicationModel
    {
        public string Id { get; set; }
        public string Application { get; set; }
        public IEnumerable<Quilt4.Interface.IApplicationVersion> Versions { get; set; }
    }
}