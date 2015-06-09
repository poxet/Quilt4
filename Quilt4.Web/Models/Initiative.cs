using System;
using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class Initiative
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ClientToken { get; set; }
        public string OwnerDeveloperName { get; set; }
        public DeveloperRole[] DeveloperRoles { get; set; }
        public string ApplicationCount { get; set; }
        public string Sessions { get; set; }
        public string Issues { get; set; }
        public IEnumerable<Guid> ApplicationsIds { get; set; }
        public IEnumerable<IApplication> Applications { get; set; }

        //public ApplicationGroup[] ApplicationGroups { get; set; }
    }

    public class DeveloperRole
    {
        public string DeveloperName { get; set; }
    }
}