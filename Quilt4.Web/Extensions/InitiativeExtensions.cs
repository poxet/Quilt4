using System.Collections.Generic;
using System.Linq;
using Quilt4.Web.Models;

namespace Quilt4.Web.Extensions
{
    public static class InitiativeExtensions
    {
        public static string GetUniqueIdentifier(this Initiative item, IEnumerable<string> names)
        {
            //First use name if possible
            if (names.Count(x => (x ?? Constants.DefaultInitiativeName) == (item.Name ?? Constants.DefaultInitiativeName)) == 1)
                return item.Name;

            return item.Id.ToString();
        }
    }
}