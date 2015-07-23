using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.Web.Controllers.WebAPI;
using Tharga.Quilt4Net.DataTransfer;

namespace Quilt4.Web.Business
{
    public class SessionBusiness : ISessionBusiness
    {
        //internal int ThreadTestDelay;
        private readonly IRepository _repository;
        private readonly IMembershipAgent _membershipAgent;
        private readonly IApplicationVersionBusiness _applicationVersionBusiness;
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IMachineBusiness _machineBusiness;
        private readonly ICounterBusiness _coutnerBusiness;

        public SessionBusiness(IRepository repository, IMembershipAgent membershipAgent, IApplicationVersionBusiness applicationVersionBusiness, IInitiativeBusiness initiativeBusiness, IUserBusiness userBusiness, IMachineBusiness machineBusiness, ICounterBusiness coutnerBusiness)
        {
            _repository = repository;
            _membershipAgent = membershipAgent;
            _applicationVersionBusiness = applicationVersionBusiness;
            _initiativeBusiness = initiativeBusiness;
            _userBusiness = userBusiness;
            _machineBusiness = machineBusiness;
            _coutnerBusiness = coutnerBusiness;
        }

        public void RegisterSession(ISession session)
        {
            var mutex = new Mutex(false, session.Id.ToString());

            try
            {
                mutex.WaitOne();

                var existingSession = _repository.GetSession(session.Id);
                //Thread.Sleep(ThreadTestDelay);
                if (existingSession == null)
                {
                    _repository.AddSession(session);
                    _coutnerBusiness.UpdateSessionCounters();
                }
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
            _coutnerBusiness.UpdateSessionCounters();
        }

        public void RegisterSession(RegisterSessionRequest request)
        {
            if (request == null) throw new ArgumentNullException("request", "No request object provided.");
            if (request.Session == null) throw new ArgumentException("No session object in request was provided. Need object '{ \"Session\":{...} }' in root.");
            if (request.Session.SessionGuid == Guid.Empty) throw new ArgumentException("No valid session guid provided.");
            if (string.IsNullOrEmpty(request.Session.ClientToken)) throw new ArgumentException("No ClientToken provided.");
            if (request.Session.Application == null) throw new ArgumentException("No application object in request was provided. Need object '{ \"Application\":{...} }' in session.");
            if (string.IsNullOrEmpty(request.Session.Application.Name)) throw new ArgumentException("No name provided for application.");
            if (request.Session.User == null) throw new ArgumentException("No user object in request was provided. Need object '{ \"User\":{...} }' in session.");
            if (request.Session.Machine == null) throw new ArgumentException("No machine object in request was provided. Need object '{ \"Machine\":{...} }' in session.");

            var callerIp = _membershipAgent.GetUserHostAddress();

            var fingerprint = _applicationVersionBusiness.AssureApplicationFingerprint(request.Session.Application.Fingerprint, request.Session.Application.Version, request.Session.Application.SupportToolkitNameVersion, request.Session.Application.BuildTime, request.Session.Application.Name, request.Session.ClientToken);

            IApplication application;
            try
            {
                application = _initiativeBusiness.RegisterApplication((ClientToken)request.Session.ClientToken, request.Session.Application.Name, request.Session.Application.Fingerprint);
            }
            catch (FingerprintException)
            {
                throw new ArgumentException("No value for application fingerprint provided. A globally unique identifier should be provided, perhaps a machine sid or a hash of unique data that does not change.");
            }

            var applicationVersion = _applicationVersionBusiness.RegisterApplicationVersionUsage(fingerprint, application.Id, request.Session.Application.Version, request.Session.Application.SupportToolkitNameVersion, request.Session.Application.BuildTime);

            if (applicationVersion.Ignore)
            {
                return;
            }

            try
            {
                _userBusiness.RegisterUser((Fingerprint)request.Session.User.Fingerprint, request.Session.User.UserName);
            }
            catch (FingerprintException)
            {
                throw new ArgumentException("No value for user fingerprint provided. A globally unique identifier should be provided, perhaps a username and domain or a hash of unique data that does not change.");
            }

            try
            {
                _machineBusiness.RegisterMachine((Fingerprint)request.Session.Machine.Fingerprint, request.Session.Machine.Name, request.Session.Machine.Data);
            }
            catch (FingerprintException)
            {
                throw new ArgumentException("No value for machine fingerprint provided. A globally unique identifier should be provided, perhaps a DeviceId, SID or a hash of unique data that does not change.");
            }

            RegisterSession(request.Session.ToSession(applicationVersion.Id, application.Id, DateTime.UtcNow, null, null, callerIp));
        }

        public IEnumerable<ISession> GetSessionsForApplicationVersion(string applicationVersionId)
        {
            return _repository.GetSessionsForApplicationVersion(applicationVersionId).OrderByDescending(x => x.ServerStartTime);
        }

        public IEnumerable<ISession> GetSessionsForApplication(Guid applicationId)
        {
            return _repository.GetSessionsForApplication(applicationId).OrderByDescending(x => x.ServerStartTime);
        }

        public IEnumerable<ISession> GetSessionsForDeveloper(string developerEmail)
        {
            return _repository.GetSessionsForDeveloper(developerEmail).OrderByDescending(x => x.ServerStartTime);
        }

        public IEnumerable<ISession> GetSessionsForApplications(IEnumerable<Guid> initiativeId)
        {
            return _repository.GetSessionsForApplications(initiativeId).OrderByDescending(x => x.ServerStartTime);
        }

        public IEnumerable<ISession> GetArchivedSessionsForApplications(IEnumerable<Guid> applicationsIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISession> GetSessionsForUser(string userId)
        {
            return _repository.GetSessionsForUser(userId);
        }

        public IEnumerable<ISession> GetSessionsForMachine(string machineId)
        {
            return _repository.GetSessionsForMachine(machineId);
        }

        public IEnumerable<ISession> GetSessions()
        {
            return _repository.GetSessions();
        }

        public ISession GetSession(Guid sessionId)
        {
            return _repository.GetSession(sessionId);
        }

        public IEnumerable<ISession> GetSessionsForMachine(Fingerprint machineFingerprint)
        {
            return _repository.GetSessionsForMachine(machineFingerprint);
        }
    }
}