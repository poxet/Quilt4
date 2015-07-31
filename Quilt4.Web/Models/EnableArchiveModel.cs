using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class EnableArchiveModel
    {
        public string InitiativeId { get; set; }
        public string Application { get; set; }
        public IEnumerable<IApplicationVersion> VersionsToArchive { get; set; }
    }
}