using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class InviteMemberModel
    {
        public string InitiativeId { get; set; }
        public IDeveloperRole Developer { get; set; }
    }
}