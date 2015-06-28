using System.Collections.Generic;

namespace Quilt4.Web.Models
{
    public class ApplicationModel
    {
        public string Initiative { get; set; }
        public string Application { get; set; }
        public IEnumerable<VersionModel> Versions { get; set; }
    }
}