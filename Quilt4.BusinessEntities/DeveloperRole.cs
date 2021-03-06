using System;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class DeveloperRole : IDeveloperRole
    {
        private readonly string _inviteEMail;
        private readonly DateTime _inviteTime;
        private string _inviteCode;
        private string _developerName;
        private string _roleName;

        public DeveloperRole(string developerName, string roleName, string inviteCode, string inviteEMail, DateTime inviteTime, DateTime inviteResponseTime)
        {
            _developerName = developerName;
            _roleName = roleName;
            _inviteCode = inviteCode;
            _inviteEMail = inviteEMail;
            _inviteTime = inviteTime;
            InviteResponseTime = inviteResponseTime;
        }

        public string DeveloperName { get { return !string.IsNullOrEmpty(_developerName) ? _developerName : null; } set { _developerName = value; } }
        public string RoleName { get { return _roleName; } set { _roleName = value; } }
        public string InviteCode { get { return _inviteCode; } set { _inviteCode = value; }}
        public string InviteEMail { get { return _inviteEMail; } }
        public DateTime InviteTime { get { return _inviteTime; } }
        public DateTime InviteResponseTime { get; set; }
    }
}