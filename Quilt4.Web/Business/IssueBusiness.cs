﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.Web.Agents;
using Quilt4.Web.BusinessEntities;
using Quilt4.Web.Controllers.WebAPI;
using Tharga.Quilt4Net;
using Tharga.Quilt4Net.DataTransfer;

namespace Quilt4.Web.Business
{
    public class IssueBusiness : IIssueBusiness
    {
        private readonly IMembershipAgent _membershipAgent;
        private readonly IApplicationVersionBusiness _applicationVersionBusiness;
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IMachineBusiness _machineBusiness;
        private readonly ISettingsBusiness _settingsBusiness;

        public IssueBusiness(IMembershipAgent membershipAgent, IApplicationVersionBusiness applicationVersionBusiness, IInitiativeBusiness initiativeBusiness, ISessionBusiness sessionBusiness, IUserBusiness userBusiness, IMachineBusiness machineBusiness, ISettingsBusiness settingsBusiness)
        {
            _membershipAgent = membershipAgent;
            _applicationVersionBusiness = applicationVersionBusiness;
            _initiativeBusiness = initiativeBusiness;
            _sessionBusiness = sessionBusiness;
            _userBusiness = userBusiness;
            _machineBusiness = machineBusiness;
            _settingsBusiness = settingsBusiness;
        }

        public ILogResponse RegisterIssue(Exception exception, IssueLevel issueLevel)
        {
            //TODO: Refactor: _compositeRoot.LogAgent.RegisterIssue(exception, IssueLevel.Warning);
            throw new NotImplementedException();
        }

        private ISession GetSession(Guid id)
        {
            throw new NotImplementedException();
        }

        private void UpdateApplicationVersion(IApplicationVersion applicationVersion)
        {
            throw new NotImplementedException();
        }

        private IApplicationVersion GetApplicationVersion(string applicationVersionFingerprint)
        {
            throw new NotImplementedException();
        }

        private IInitiative GetInitiativeByApplication(Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public RegisterIssueResponse RegisterIssue(RegisterIssueRequest request)
        {
            if (request == null) throw new ArgumentNullException("request", "No request object provided.");
            if (request.Session == null) throw new ArgumentException("No session object in request was provided. Need object '{ \"Session\":{...} }' in root.");
            if (request.Session.SessionGuid == Guid.Empty) throw new ArgumentException("No valid session guid provided.");
            if (string.IsNullOrEmpty(request.Session.ClientToken)) throw new ArgumentException("No ClientToken provided.");
            if (request.IssueType == null) throw new ArgumentException("No IssueType object in request was provided. Need object '{ \"IssueType\":{...} }' in root.");
            if (string.IsNullOrEmpty(request.IssueType.Message)) throw new ArgumentException("No message in issue type provided.");
            if (string.IsNullOrEmpty(request.IssueType.IssueLevel)) throw new ArgumentException("No issue level in issue type provided.");

            var callerIp = _membershipAgent.GetUserHostAddress();

            ISession session = null;
            if (request.Session.Application == null)
            {
                session = GetSession(request.Session.SessionGuid);
                if (session == null)
                {
                    throw new ArgumentException("Cannot find session with provided session id.").AddData("SessionGuid", request.Session.SessionGuid);
                }
            }

            var ad = GetApplicationData(request, session);

            var fingerprint = _applicationVersionBusiness.AssureApplicationFingerprint(ad.Fingerprint, ad.Version, ad.SupportToolkitNameVersion, ad.BuildTime, ad.Name, request.Session.ClientToken);

            IApplication application;
            try
            {
                application = _initiativeBusiness.RegisterApplication((ClientToken)request.Session.ClientToken, ad.Name, ad.Fingerprint);
            }
            catch (FingerprintException)
            {
                throw new ArgumentException("No value for application fingerprint provided. A globally unique identifier should be provided, perhaps a machine sid or a hash of unique data that does not change.");
            }

            var applicationVersion = _applicationVersionBusiness.RegisterApplicationVersion(fingerprint, application.Id, ad.Version, ad.SupportToolkitNameVersion, ad.BuildTime);

            if (applicationVersion.Ignore)
            {
                return new RegisterIssueResponse { IssueTypeTicket = null, IssueInstanceTicket = null, ResponseMessage = applicationVersion.ResponseMessage, IsOfficial = applicationVersion.IsOfficial, };
            }

            if (session == null)
            {
                if (request.Session.User.Fingerprint == null) throw new ArgumentException("No user fingerprint provided.");
                session = request.Session.ToSession(applicationVersion.Id, application.Id, DateTime.UtcNow, null, null, callerIp);
            }

            _sessionBusiness.RegisterSession(session);

            var ud = GetUserData(request, session);
            _userBusiness.RegisterUser((Fingerprint)session.UserFingerprint, ud.UserName);

            var md = GetMachineData(request, session);
            _machineBusiness.RegisterMachine((Fingerprint)md.Fingerprint, md.Name, md.Data);

            int issueTypeTicket;
            int issueTicket;
            string issueTypeResponseMessage;

            var mutex = new Mutex(false, application.Id.ToString());
            try
            {
                mutex.WaitOne();

                var applicationVersions = _applicationVersionBusiness.GetApplicationVersions(application.Id).ToArray();

                var issueType = applicationVersion.IssueTypes.FirstOrDefault(x => request.IssueType.AreEqual(x));
                if (issueType == null)
                {
                    var issueTypes = applicationVersions.SelectMany(x => x.IssueTypes).ToArray();
                    var lastIssueTypeTicket = issueTypes.Any() ? issueTypes.Max(x => x.Ticket) : 0;
                    issueTypeTicket = lastIssueTypeTicket + 1;
                    var inner = ToInnerIssueType(request.IssueType.Inner);

                    issueType = new Quilt4.BusinessEntities.IssueType(request.IssueType.ExceptionTypeName, request.IssueType.Message, request.IssueType.StackTrace ?? string.Empty, request.IssueType.IssueLevel.ToIssueLevel(), inner, new List<IIssue>(), issueTypeTicket, null);
                    applicationVersion.Add(issueType);
                }
                else
                {
                    issueTypeTicket = issueType.Ticket;
                }

                issueTypeResponseMessage = issueType.ResponseMessage;

                var issues = applicationVersions.SelectMany(x => x.IssueTypes).SelectMany(x => x.Issues).ToArray();
                var lastIssueTicket = issues.Any() ? issues.Max(x => x.Ticket) : 0;
                issueTicket = lastIssueTicket + 1;

                var issue = new Quilt4.BusinessEntities.Issue(request.Id, request.ClientTime, DateTime.UtcNow, request.VisibleToUser, request.Data, request.IssueThreadGuid, request.UserHandle, request.UserInput, issueTicket, session.Id);
                issueType.Add(issue);

                UpdateApplicationVersion(applicationVersion);
            }
            finally
            {
                mutex.ReleaseMutex();
            }

            var response = new RegisterIssueResponse { //IssueTypeTicket = application.TicketPrefix + Settings.IssueTypeTicketPrefix + issueTypeTicket,
                IssueTypeTicket = application.TicketPrefix + _settingsBusiness.GetSetting<string>("IssueTypeTicketPrefix") + issueTypeTicket,
                //IssueInstanceTicket = application.TicketPrefix + Settings.IssueTicketPrefix + issueTicket,
                IssueInstanceTicket = application.TicketPrefix + _settingsBusiness.GetSetting<string>("IssueTicketPrefix") + issueTicket, ResponseMessage = applicationVersion.ResponseMessage ?? issueTypeResponseMessage, IsOfficial = applicationVersion.IsOfficial, };
            return response;
        }

        private ApplicationData GetApplicationData(RegisterIssueRequest request, ISession session)
        {
            ApplicationData ad;
            if (request.Session.Application == null)
            {
                //var av = _compositeRoot.Repository.GetApplicationVersion(session.ApplicationVersionId);
                var av = GetApplicationVersion(session.ApplicationVersionId);
                //var a = _compositeRoot.Repository.GetInitiativeByApplication(av.ApplicationId).ApplicationGroups.SelectMany(x => x.Applications).First(x => x.Id == av.ApplicationId);
                var a = GetInitiativeByApplication(av.ApplicationId).ApplicationGroups.SelectMany(x => x.Applications).First(x => x.Id == av.ApplicationId);

                ad = new ApplicationData { Version = av.Version, Fingerprint = av.Id, BuildTime = av.BuildTime, SupportToolkitNameVersion = av.SupportToolkitNameVersion, Name = a.Name };
            }
            else
            {
                ad = request.Session.Application;
            }

            return ad;
        }

        private MachineData GetMachineData(RegisterIssueRequest request, ISession session)
        {
            MachineData md;
            if (request.Session.Machine == null)
            {
                var machine = _machineBusiness.GetMachine(session.MachineFingerprint);
                md = new MachineData { Fingerprint = machine.Id, Name = machine.Name, Data = machine.Data, };
            }
            else
            {
                md = request.Session.Machine;
            }
            return md;
        }

        private UserData GetUserData(RegisterIssueRequest request, ISession session)
        {
            UserData ud;
            if (request.Session.User == null)
            {
                var u = _userBusiness.GetUser(session.UserFingerprint);
                ud = new UserData { Fingerprint = u.Id, UserName = u.UserName };
            }
            else
            {
                ud = request.Session.User;
            }
            return ud;
        }

        private IInnerIssueType ToInnerIssueType(Tharga.Quilt4Net.DataTransfer.IssueType issueType)
        {
            if (issueType == null)
                return null;

            var inner = ToInnerIssueType(issueType.Inner);

            var innerIssueType = new InnerIssueType(issueType.ExceptionTypeName, issueType.Message, issueType.StackTrace ?? string.Empty, issueType.IssueLevel, inner);
            return innerIssueType;
        }
    }
}