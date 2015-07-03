using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quilt4.Interface;
using Quilt4.Web.BusinessEntities;

namespace Quilt4.Web.Business
{
    //TODO: Batch updates on existing data
    public class CounterBusiness : ICounterBusiness
    {
        private readonly IInfluxDbAgent _influxDbAgent;
        private readonly IRepository _repository;

        public CounterBusiness(IInfluxDbAgent influxDbAgent, IRepository repository)
        {
            _influxDbAgent = influxDbAgent;
            _repository = repository;
        }

        private void Register(string counterName, Dictionary<string, object>[] datas)
        {
            var task = Task.Run(async () => await _influxDbAgent.WriteAsync(datas.Select(data => new Serie(counterName, data))));
            task.Wait();
        }

        private List<ISerie> Query(string counterName)
        {
            var task = Task<List<ISerie>>.Run(async () => await _influxDbAgent.QueryAsync(counterName));
            return task.Result;
        }

        //public void RegisterInitiative(IInitiativeHead initiative)
        //{
        //    throw new NotImplementedException();
        //    var data = new Dictionary<string, object>
        //    {
        //        { "InitiativeId", initiative.Id },
        //        { "InitiativeName", initiative.Name },
        //        { "OwnerDeveloperName", initiative.OwnerDeveloperName },
        //    };
        //    Register("Initiative", new[] { data });
        //}

        //public void RegisterApplication(IInitiativeHead initiative, IApplication application)
        //{
        //    throw new NotImplementedException();
        //    var data = new Dictionary<string, object>
        //    {
        //        { "InitiativeId", initiative.Id },
        //        { "InitiativeName", initiative.Name },
        //        { "OwnerDeveloperName", initiative.OwnerDeveloperName },
        //        { "ApplicationId", application.Id.ToString() },
        //        { "ApplicationName", application.Name },
        //    };
        //    Register("Application", new[] { data });
        //}

        public void UpdateApplicationVersionCounters()
        {
            var initiatives = _repository.GetInitiatives().ToArray();
            var applicationVersions = _repository.GetApplicationVersions();

            var datas = new List<Dictionary<string, object>>();
            var avSessionses = applicationVersions.GroupBy(x => new { x.ApplicationId, InitiativeId = GetInitiativeId(initiatives, x) });
            foreach (var avSessions in avSessionses)
            {
                var data = new Dictionary<string, object>
                {
                    { "InitiativeId", avSessions.Key.InitiativeId },
                    { "ApplicationId", avSessions.Key.ApplicationId },
                    { "Total", avSessions.Count() },
                };

                datas.Add(data);
            }
            Register("ApplicationVersion", datas.ToArray());
        }

        private static Guid GetInitiativeId(IInitiative[] initiatives, IApplicationVersion x)
        {
            var initiative = initiatives.SingleOrDefault(xx => xx.ApplicationGroups.SelectMany(y => y.Applications).Any(z => z.Id == x.ApplicationId));
            return initiative != null ? initiative.Id : Guid.Empty;
        }

        //public void RegisterIssueType(IInitiativeHead initiative, IApplication application, IApplicationVersion applicationVersion, IIssueType issueType)
        //{
        //    throw new NotImplementedException();
        //    var data = new Dictionary<string, object>
        //    {
        //        { "InitiativeId", initiative.Id },
        //        { "InitiativeName", initiative.Name },
        //        { "OwnerDeveloperName", initiative.OwnerDeveloperName },
        //        { "ApplicationId", application.Id.ToString() },
        //        { "ApplicationName", application.Name },
        //        { "ApplicationVersionId", applicationVersion.Id },
        //        { "ApplicationVersionBuildTime", applicationVersion.BuildTime },
        //        { "ApplicationVersionVersion", applicationVersion.Version },
        //        { "ApplicationVersionSupportToolkit", applicationVersion.SupportToolkitNameVersion },
        //        { "IssueTypeName", issueType.ExceptionTypeName },
        //        { "IssueTypeLevel", issueType.IssueLevel },
        //        { "IssueTypeMessage", issueType.Message },
        //        { "IssueTypeTicket", issueType.Ticket },
        //    };
        //    Register("IssueType", new[] { data });
        //}

        //public void RegisterIssue(IInitiativeHead initiative, IApplication application, IApplicationVersion applicationVersion, IIssueType issueType, IIssue issue)
        //{
        //    throw new NotImplementedException();
        //    var data = new Dictionary<string, object>
        //    {
        //        { "InitiativeId", initiative.Id },
        //        { "InitiativeName", initiative.Name },
        //        { "OwnerDeveloperName", initiative.OwnerDeveloperName },
        //        { "ApplicationId", application.Id.ToString() },
        //        { "ApplicationName", application.Name },
        //        { "ApplicationVersionId", applicationVersion.Id },
        //        { "ApplicationVersionBuildTime", applicationVersion.BuildTime },
        //        { "ApplicationVersionVersion", applicationVersion.Version },
        //        { "ApplicationVersionSupportToolkit", applicationVersion.SupportToolkitNameVersion },
        //        { "IssueTypeName", issueType.ExceptionTypeName },
        //        { "IssueTypeLevel", issueType.IssueLevel },
        //        { "IssueTypeMessage", issueType.Message },
        //        { "IssueTypeTicket", issueType.Ticket },
        //        { "IssueId", issue.Id },
        //        { "IssueThreadGuid", issue.IssueThreadGuid },
        //        { "IssueTicket", issue.Ticket },
        //        { "IssueUserHandle", issue.UserHandle },
        //        { "IssueVisibleToUser", issue.VisibleToUser },
        //    };

        //    foreach (var machineData in issue.Data)
        //    {
        //        data.Add(machineData.Key, machineData.Value);
        //    }

        //    Register("Issue", new[] { data });
        //}

        public void UpdateSessionCounters()
        {
            var sessions = _repository.GetActiveSessions(15 * 60);

            var datas = new List<Dictionary<string, object>>();
            foreach (var avSessions in sessions.GroupBy(x => new { x.ApplicationId, x.ApplicationVersionId, x.Environment, x.MachineFingerprint, x.CallerIp, x.UserFingerprint }))
            {
                var data = new Dictionary<string, object>
                {
                    { "ApplicationId", avSessions.Key.ApplicationId },
                    { "ApplicationVersionId", avSessions.Key.ApplicationVersionId },
                    { "Environment", avSessions.Key.Environment },
                    { "MachineFingerprint", avSessions.Key.MachineFingerprint },
                    { "CallerIp", avSessions.Key.CallerIp },
                    { "UserFingerprint", avSessions.Key.UserFingerprint },
                    { "Total", avSessions.Count() },
                };

                datas.Add(data);
            }

            //TODO: Points that previously had a count, but now are zero, should be explicitly be set as zero.
            //? How do I find theese points
            //i Perhaps query inflyx to get the last input from there. Check if that combination exists in datas, and add with zero value, if it does not.
            var existing = Query("Session");

            Register("Session", datas.ToArray());
        }

        //public void RegisterMachine(IMachine machine)
        //{
        //    throw new NotImplementedException();
        //    var data = new Dictionary<string, object>
        //    {
        //        { "MachineId", machine.Id },
        //        { "MachineName", machine.Name },
        //    };
            
        //    foreach (var machineData in machine.Data)
        //    {
        //        data.Add(machineData.Key, machineData.Value);
        //    }

        //    Register("Machine", new[] { data });
        //}

        //public void RegisterUser(IUser user)
        //{
        //    throw new NotImplementedException();
        //    var data = new Dictionary<string, object>
        //    {
        //        { "UserId", user.Id },
        //        { "UserName", user.UserName },
        //    };

        //    Register("User", new[] { data });
        //}

        //public void RegisterDeveloper(IDeveloper developer)
        //{
        //    throw new NotImplementedException();
        //    var data = new Dictionary<string, object>
        //    {
        //        { "DeveloperName", developer.UserId },
        //        { "DeveloperId", developer.UserName },
        //    };

        //    Register("Developer", new[] { data });
        //}
    }
}