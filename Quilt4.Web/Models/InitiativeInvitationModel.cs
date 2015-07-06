using System;

namespace Quilt4.Web.Models
{
    public class InitiativeInvitationModel
    {
        public Guid InitiativeId { get; set; }
        public string InitiativeName { get; set; }
        public string InviteCode { get; set; }
    }
}