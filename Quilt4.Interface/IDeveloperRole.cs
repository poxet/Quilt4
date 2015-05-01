using System;

namespace Quilt4.Interface
{
    public interface IDeveloperRole
    {
        string DeveloperName { get; set; }
        string RoleName { get; set; }
        string InviteCode { get; }
        string InviteEMail { get; }
        DateTime InviteTime { get; }
        //string InviteMessage { get; } // TODO: Add this property
    }
}