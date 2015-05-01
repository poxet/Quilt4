using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Quilt4.BusinessEntities;
using Quilt4.Interface;

namespace Quilt4.Web.BusinessEntities
{
    public class SessionBusiness : ISessionBusiness
    {
        internal int ThreadTestDelay;
        private readonly IRepository _repository;

        public SessionBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public void RegisterSession(ISession session)
        {
            var mutex = new Mutex(false, session.Id.ToString());

            try
            {
                mutex.WaitOne();

                var existingSession = _repository.GetSession(session.Id);
                Thread.Sleep(ThreadTestDelay);
                if (existingSession == null)
                    _repository.AddSession(session);
                else
                {
                    if (session.ApplicationVersionId != existingSession.ApplicationVersionId)
                    {
                        var ex = new ArgumentException("The session belongs to a different application version.");
                        ex.Data.Add("SessionId", session.Id);
                        ex.Data.Add("ApplicationVersionId", session.ApplicationVersionId);
                        ex.Data.Add("ExistingApplicationVersionId", existingSession.ApplicationVersionId);
                        throw ex;
                    }
                    _repository.UpdateSessionUsage(session.Id, DateTime.UtcNow);
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public void EndSession(Guid sessionId)
        {
            _repository.EndSession(sessionId, DateTime.UtcNow);
        }

        public IEnumerable<ISession> GetSessionsForApplicationVersion(string applicationVersionId)
        {
            return _repository.GetSessionsForApplicationVersion(applicationVersionId).OrderByDescending(x => x.ServerStartTime);
        }

        public IEnumerable<ISession> GetSessionsForApplication(Guid applicationId)
        {
            return _repository.GetSessionsForApplication(applicationId).OrderByDescending(x => x.ServerStartTime);
        }

        public IEnumerable<ISession> GetSessionsForDeveloper(string developerName)
        {
            return _repository.GetSessionsForDeveloper(developerName).OrderByDescending(x => x.ServerStartTime);
        }

        public IEnumerable<ISession> GetSessionsForApplications(IEnumerable<Guid> initiativeId)
        {
            return _repository.GetSessionsForApplications(initiativeId).OrderByDescending(x => x.ServerStartTime);
        }

        public IEnumerable<ISession> GetSessionsForMachine(Fingerprint machineFingerprint)
        {
            return _repository.GetSessionsForMachine(machineFingerprint);
        }
    }
}