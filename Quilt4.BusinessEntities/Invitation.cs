using System;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class Invitation : IInvitation
    {
        public Invitation(Guid initiativeId, string initiativeName, string inviteCode)
        {
            InitiativeId = initiativeId;
            InitiativeName = initiativeName;
            InviteCode = inviteCode;
        }

        public Guid InitiativeId { get; private set; }
        public string InitiativeName { get; private set; }
        public string InviteCode { get; private set; }
    }
}