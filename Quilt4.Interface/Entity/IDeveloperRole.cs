using System;

namespace Quilt4.Interface
{
    public interface IDeveloperRole
    {
        //TODO: REFACTOR: Make immutable
        string DeveloperName { get; set; }
        string RoleName { get; set; }
        string InviteCode { get; set; }
        string InviteEMail { get; }
        DateTime InviteTime { get; }
        DateTime InviteResponseTime { get; set; }
    }
}