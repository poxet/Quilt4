using System;

namespace Quilt4.Interface
{
    public interface IInvitation
    {
        Guid InitiativeId { get; }
        string InitiativeName { get; }
        string InviteCode { get; }
    }
}