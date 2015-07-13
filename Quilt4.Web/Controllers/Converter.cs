using System;
using System.Collections.Generic;
using System.Linq;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Controllers
{
    public static class Converter
    {
        public static ApplicationGroup ToModel(this IApplicationGroup item)
        {
            return new ApplicationGroup(item.Name, item.Applications.Select(x => x.ToModel()));
        }

        public static Application ToModel(this IApplication item)
        {
            return new Application(item.Id,item.Name,item.FirstRegistered,item.TicketPrefix);
        }

        public static InitiativeViewModel ToModel(this IInitiative item, IEnumerable<string> allInitiativeNames)
        {
            var dateCreated = (item.ApplicationGroups.SelectMany(x => x.Applications)).Select(y => y.FirstRegistered).OrderBy(z => z.Date).FirstOrDefault();
            
            var response = new InitiativeViewModel
            {
                Id = item.Id,
                Name = item.Name,
                ClientToken = item.ClientToken,
                OwnerDeveloperName = item.OwnerDeveloperName,
                DeveloperRoles = item.DeveloperRoles.Select(x => x.ToModel()).ToArray(),
                ApplicationCount = item.ApplicationGroups.SelectMany(x => x.Applications).Count().ToString(),
                Sessions = (item.ApplicationGroups.SelectMany(x => x.Applications)).Select(y => y.Id).ToString(),
                FirstUsedDate = dateCreated == new DateTime() ? "N/A" : dateCreated.ToShortDateString() + " " + dateCreated.ToShortTimeString(),
                ApplicationGroups = item.ApplicationGroups.Select(x => x.ToModel()).ToArray(),
                UniqueIdentifier = item.GetUniqueIdentifier(allInitiativeNames),                
            };
            return response;
        }

        public static DeveloperRoleViewModel ToModel(this IDeveloperRole item)
        { 
            return new DeveloperRoleViewModel
            {
                DeveloperName = item.DeveloperName,                
            };
        }
    }
}