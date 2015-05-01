using System;

namespace Quilt4.Interface
{
    public interface ISession
    {
        Guid Id { get; }
        string ApplicationVersionId { get; }
        string Environment { get; }
        Guid ApplicationId { get; }
        string MachineFingerprint { get; }
        string UserFingerprint { get; }
        DateTime ClientStartTime { get; }
        DateTime ServerStartTime { get; }
        DateTime? ServerEndTime { get; }
        DateTime? ServerLastKnown { get; }
        string CallerIp { get; }
    }
}