using System.ComponentModel.DataAnnotations;
using Quilt4.Interface;

namespace Quilt4.Web.Areas.Admin.Models
{
    public class InviteModel
    {
        [Display(Name = "Email")]
        public string InviteEmail { get; set; }

        public IInitiative Initiative { get; set; }

        [Display(Name = "Additional message")]
        public string Message { get; set; }

        public bool IsAllowedToAdministrate { get; set; }
        public string UniqueInitiativeIdentifier { get; set; }
    }
}