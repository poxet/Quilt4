using System;
using Quilt4.BusinessEntities;

namespace Quilt4.Web.Controllers.WebAPI
{
    public static class Converters
    {
        public static Session ToSession(this Tharga.Quilt4Net.DataTransfer.Session item, Fingerprint applicationVersionId, Guid applicationId, DateTime serverStartTime, DateTime? serverEndTime, DateTime? serverLastKnown, string callerIp)
        {
            var machineId = (Fingerprint)item.Machine.Fingerprint;
            var userId = (Fingerprint)item.User.Fingerprint;
            return new Session(item.SessionGuid, applicationVersionId, item.Environment, applicationId, machineId, userId, item.ClientStartTime, serverStartTime, serverEndTime, serverLastKnown, callerIp);
        }
    }
}