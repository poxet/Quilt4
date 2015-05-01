using System;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class Session : ISession
    {
        private readonly Guid _id;
        private readonly Fingerprint _applicationVersionId;
        private readonly string _environment;
        private readonly Guid _applicationId;
        private readonly Fingerprint _machineId;
        private readonly Fingerprint _userId;
        private readonly DateTime _clientStartTime;
        private readonly DateTime _serverStartTime;
        private readonly DateTime? _serverEndTime;
        private readonly DateTime? _serverLastKnown;
        private readonly string _callerIp;

        public Session(Guid id, Fingerprint applicationVersionId, string environment, Guid applicationId, Fingerprint machineId, Fingerprint userId, DateTime clientStartTime, DateTime serverStartTime, DateTime? serverEndTime, DateTime? serverLastKnown, string callerIp)
        {
            _id = id;
            _applicationVersionId = applicationVersionId;
            _environment = environment;
            _applicationId = applicationId;
            _machineId = machineId;
            _userId = userId;
            _clientStartTime = clientStartTime;
            _serverStartTime = serverStartTime;
            _serverEndTime = serverEndTime;
            _serverLastKnown = serverLastKnown;
            _callerIp = callerIp;
        }

        public Guid Id { get { return _id; } }
        public string ApplicationVersionId { get { return _applicationVersionId; } }
        public string Environment { get { return _environment; } }
        public Guid ApplicationId { get { return _applicationId; } }
        public string MachineFingerprint { get { return _machineId; } }
        public string UserFingerprint { get { return _userId; } }
        public DateTime ClientStartTime { get { return _clientStartTime; } }
        public DateTime ServerStartTime { get { return _serverStartTime; } }
        public DateTime? ServerEndTime { get { return _serverEndTime; } }
        public DateTime? ServerLastKnown { get { return _serverLastKnown; } }
        public string CallerIp { get { return _callerIp; } }
    }
}