using System;

namespace Quilt4.Web.Models
{
    public class Initiative
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ClientToken { get; set; }
        public string OwnerDeveloperName { get; set; }
        //public DeveloperRole[] DeveloperRoles { get; set; }
        //public ApplicationGroup[] ApplicationGroups { get; set; }
    }
}