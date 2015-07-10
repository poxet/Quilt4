using System;
using System.Collections.Generic;
using Tharga.Quilt4Net.DataTransfer;

namespace Quilt4.Interface
{
    public interface ISessionBusiness
    {
        void RegisterSession(ISession session);
        void RegisterSession(RegisterSessionRequest data);
        void EndSession(Guid sessionId);
        IEnumerable<ISession> GetSessionsForApplicationVersion(string applicationVersionId);
        IEnumerable<ISession> GetSessionsForApplications(IEnumerable<Guid> applicationIds);
        IEnumerable<ISession> GetSessionsForUser(string userId);
        IEnumerable<ISession> GetSessionsForMachine(string machineId);
        IEnumerable<ISession> GetSessions();
        ISession GetSession(Guid sessionId);
    }
}