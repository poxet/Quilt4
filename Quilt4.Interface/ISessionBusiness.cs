using System;
using Tharga.Quilt4Net.DataTransfer;

namespace Quilt4.Interface
{
    public interface ISessionBusiness
    {
        void RegisterSession(ISession session);
        void EndSession(Guid sessionId);
        void RegisterSessionEx(RegisterSessionRequest data);
    }
}