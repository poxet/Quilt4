using System;
using System.Collections.Generic;
using Tharga.Quilt4Net.DataTransfer;

namespace Quilt4.Interface
{
    public interface ISessionBusiness
    {
        void RegisterSession(ISession session);
        void EndSession(Guid sessionId);
        void RegisterSessionEx(RegisterSessionRequest data);
        IEnumerable<ISession> GetSessionsForApplicationVersion(string applicationVersionId);
        IEnumerable<ISession> GetSessionsForApplications(IEnumerable<Guid> applicationIds);
        IEnumerable<ISession> GetSessionsForUser(string userId);
        IEnumerable<ISession> GetSessionsForMachine(string machineId);
    }
}