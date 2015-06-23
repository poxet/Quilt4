using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class ApplicationModel
    {
        public string Initiative { get; set; }
        public string Application { get; set; }
        public IEnumerable<VersionModel> Versions { get; set; }
        public List<IEnumerable<IMachine>> Machines { get; set; }
    }

    public class VersionModel
    {
        public string UniqueIdentifier { get; set; }
        public string Version { get; set; }
    }
}