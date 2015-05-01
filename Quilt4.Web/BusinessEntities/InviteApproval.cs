using System;
using Quilt4.Interface;

namespace Quilt4.Web.BusinessEntities
{
    public class InviteApproval : IInviteApproval
    {
        public Guid InitiativeId { get; set; }
        public string InitiativeName { get; set; }
        public string InitiativeOwner { get; set; }
        public string Message { get; set; }
        public DateTime InviteTime { get; set; }
        public string InviteCode { get; set; }
    }
}