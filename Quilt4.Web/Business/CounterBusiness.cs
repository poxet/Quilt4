using System;
using System.Collections.Generic;
using Quilt4.Interface;
using Quilt4.Web.BusinessEntities;

namespace Quilt4.Web.Business
{
    public class CounterBusiness : ICounterBusiness
    {
        private readonly IInfluxDbAgent _influxDbAgent;

        public CounterBusiness(IInfluxDbAgent influxDbAgent)
        {
            _influxDbAgent = influxDbAgent;
        }

        private void Register(string counterName, int count, Dictionary<string, object> data)
        {
            _influxDbAgent.WriteAsync(new Serie(counterName, data));
        }

        public void RegisterInitiative(IInitiativeHead initiative)
        {
            var data = new Dictionary<string, object>
            {
                { "InitiativeId", initiative.Id },
                { "InitiativeName", initiative.Name },
                { "OwnerDeveloperName", initiative.OwnerDeveloperName },
            };
            Register("Initiative", 1, data);
        }

        public void RegisterApplication(IInitiativeHead initiative, IApplication application)
        {
            var data = new Dictionary<string, object>
            {
                { "InitiativeId", initiative.Id },
                { "InitiativeName", initiative.Name },
                { "OwnerDeveloperName", initiative.OwnerDeveloperName },
                { "ApplicationId", application.Id.ToString() },
                { "ApplicationName", application.Name },
            };
            Register("Application", 1, data);
        }

        public void RegisterApplicationVersion(IInitiativeHead initiative, IApplication application, IApplicationVersion applicationVersion)
        {
            var data = new Dictionary<string, object>
            {
                { "InitiativeId", initiative.Id },
                { "InitiativeName", initiative.Name },
                { "OwnerDeveloperName", initiative.OwnerDeveloperName },
                { "ApplicationId", application.Id.ToString() },
                { "ApplicationName", application.Name },
                { "ApplicationVersionId", applicationVersion.Id },
                { "ApplicationVersionBuildTime", applicationVersion.BuildTime },
                { "ApplicationVersionVersion", applicationVersion.Version },
                { "ApplicationVersionSupportToolkit", applicationVersion.SupportToolkitNameVersion },
            };
            Register("ApplicationVersion", 1, data);
        }

        public void RegisterIssueType(IInitiativeHead initiative, IApplication application, IApplicationVersion applicationVersion, IIssueType issueType)
        {
            var data = new Dictionary<string, object>
            {
                { "InitiativeId", initiative.Id },
                { "InitiativeName", initiative.Name },
                { "OwnerDeveloperName", initiative.OwnerDeveloperName },
                { "ApplicationId", application.Id.ToString() },
                { "ApplicationName", application.Name },
                { "ApplicationVersionId", applicationVersion.Id },
                { "ApplicationVersionBuildTime", applicationVersion.BuildTime },
                { "ApplicationVersionVersion", applicationVersion.Version },
                { "ApplicationVersionSupportToolkit", applicationVersion.SupportToolkitNameVersion },
                { "IssueTypeName", issueType.ExceptionTypeName },
                { "IssueTypeLevel", issueType.IssueLevel },
                { "IssueTypeMessage", issueType.Message },
                { "IssueTypeTicket", issueType.Ticket },
            };
            Register("IssueType", 1, data);
        }

        public void RegisterIssue(IInitiativeHead initiative, IApplication application, IApplicationVersion applicationVersion, IIssueType issueType, IIssue issue)
        {
            var data = new Dictionary<string, object>
            {
                { "InitiativeId", initiative.Id },
                { "InitiativeName", initiative.Name },
                { "OwnerDeveloperName", initiative.OwnerDeveloperName },
                { "ApplicationId", application.Id.ToString() },
                { "ApplicationName", application.Name },
                { "ApplicationVersionId", applicationVersion.Id },
                { "ApplicationVersionBuildTime", applicationVersion.BuildTime },
                { "ApplicationVersionVersion", applicationVersion.Version },
                { "ApplicationVersionSupportToolkit", applicationVersion.SupportToolkitNameVersion },
                { "IssueTypeName", issueType.ExceptionTypeName },
                { "IssueTypeLevel", issueType.IssueLevel },
                { "IssueTypeMessage", issueType.Message },
                { "IssueTypeTicket", issueType.Ticket },
                { "IssueId", issue.Id },
                { "IssueThreadGuid", issue.IssueThreadGuid },
                { "IssueTicket", issue.Ticket },
                { "IssueUserHandle", issue.UserHandle },
                { "IssueVisibleToUser", issue.VisibleToUser },
            };

            foreach (var machineData in issue.Data)
            {
                data.Add(machineData.Key, machineData.Value);
            }

            Register("Issue", 1, data);
        }

        public void RegisterSession(IInitiativeHead initiative, IApplication application, IApplicationVersion applicationVersion, ISession session)
        {
            var data = new Dictionary<string, object>
            {
                { "InitiativeId", initiative.Id },
                { "InitiativeName", initiative.Name },
                { "OwnerDeveloperName", initiative.OwnerDeveloperName },
                { "ApplicationId", application.Id.ToString() },
                { "ApplicationName", application.Name },
                { "ApplicationVersionId", applicationVersion.Id },
                { "ApplicationVersionBuildTime", applicationVersion.BuildTime },
                { "ApplicationVersionVersion", applicationVersion.Version },
                { "ApplicationVersionSupportToolkit", applicationVersion.SupportToolkitNameVersion },
                { "SessionId", session.Id },
                { "CallerIp", session.CallerIp },
                { "Environment", session.Environment },
                { "MachineFingerprint", session.MachineFingerprint },
                { "UserFingerprint", session.UserFingerprint },
            };
            Register("Session", 1, data);
        }

        public void RegisterMachine(IMachine machine)
        {
            var data = new Dictionary<string, object>
            {
                { "MachineId", machine.Id },
                { "MachineName", machine.Name },
            };
            
            foreach (var machineData in machine.Data)
            {
                data.Add(machineData.Key, machineData.Value);
            }

            Register("Machine", 1, data);
        }

        public void RegisterUser(IUser user)
        {
            var data = new Dictionary<string, object>
            {
                { "UserId", user.Id },
                { "UserName", user.UserName },
            };

            Register("User", 1, data);
        }

        public void RegisterDeveloper(IDeveloper developer)
        {
            var data = new Dictionary<string, object>
            {
                { "DeveloperName", developer.UserId },
                { "DeveloperId", developer.UserName },
            };

            Register("Developer", 1, data);
        }
    }
}