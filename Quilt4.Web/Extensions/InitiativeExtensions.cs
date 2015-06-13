using System.Collections.Generic;
using System.Linq;
using Quilt4.Web.Models;

namespace Quilt4.Web.Extensions
{
    public static class InitiativeExtensions
    {
        public static string GetUniqueIdentifier(this Initiative item, IEnumerable<Initiative> items)
        {
            //First use name if possible
            if (items.Count(x => (x.Name ?? Constants.DefaultInitiativeName) == (item.Name ?? Constants.DefaultInitiativeName)) == 1)
                return item.Name;

            return item.Id.ToString();
        }
    }
}