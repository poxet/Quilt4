using System;

namespace Quilt4.MongoDBRepository.Entities
{
    internal class DeveloperRolePersist
    {
        public string DeveloperName { get; internal set; }
        public string RoleName { get; internal set; }
        public string InviteCode { get; internal set; }
        public string InviteEMail { get; internal set; }
        public DateTime InviteTime { get; internal set; }
        public DateTime InviteResponseTime { get; internal set; }
    }
}