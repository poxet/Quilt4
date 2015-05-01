using System;

namespace Quilt4.Interface
{
    public interface ISessionBusiness
    {
        void RegisterSession(ISession session);
        void EndSession(Guid sessionId);
    }
}