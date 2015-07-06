using System;
using Quilt4.Interface;

namespace Quilt4.Web
{
    public static class SessionExtensions
    {
        public static DateTime ServerEndTimeCalculated(this ISession session)
        {
            if (session.ServerEndTime != null)
                return session.ServerEndTime.Value;

            if (session.ServerLastKnown != null)
                return session.ServerLastKnown.Value.AddMinutes(15);

            return session.ServerStartTime.AddMinutes(15);
        }
    }
}