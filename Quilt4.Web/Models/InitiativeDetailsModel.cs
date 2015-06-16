using System.Collections.Generic;

namespace Quilt4.Web.Models
{
    public class InitiativeDetailsModel
    {
        public Initiative Initiative { get; set; }
        public List<string> AllInitiativeNames { get; set; }
    }
}