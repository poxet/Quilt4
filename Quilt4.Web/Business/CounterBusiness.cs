using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
        private readonly IEventLogAgent _eventLogAgent;

        public CounterBusiness(IInfluxDbAgent influxDbAgent, IRepository repository, IEventLogAgent eventLogAgent)
        {
            _influxDbAgent = influxDbAgent;
            _repository = repository;
            _eventLogAgent = eventLogAgent;
        }

        private void Register(string counterName, Dictionary<string, object>[] datas)
        {
            var task = Task.Run(async () => await _influxDbAgent.WriteAsync(datas.Select(data => new Serie(counterName, data))));
            task.Wait();
        }

        private void Clear(string counterName)
        {
            Task.Run(async () => await _influxDbAgent.ClearAsync(counterName));
        }

        private List<ISerie> Query(string counterName)
        {
            var task = Task<List<ISerie>>.Run(async () => await _influxDbAgent.QueryAsync(counterName));
            return task.Result;
        }

        private ISerie QueryLast(string counterName)
        {
            if (!_influxDbAgent.IsEnabled) throw new InvalidOperationException("InfluxDb agent is not enabled.");
            var task = Task<ISerie>.Run(async () => await _influxDbAgent.QueryLastAsync(counterName));
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

        public void ClearSessionCounters()
        {
            if (!_influxDbAgent.IsEnabled)
                throw new InvalidOperationException("InflixDb agent is not enabled.");

            Clear("Session");
        }

        private DateTime GetLastSessionCounterTime()
        {
            var lastItem = QueryLast("Session");
            if (lastItem == null)
                return DateTime.MinValue;

            var lastEpoch = (long)lastItem.Data["time"];
            var lastTime = lastEpoch.ToDateTime();
            return lastTime;
        }

        public void UpdateSessionCounters()
        {
            Task.Factory.StartNew(UpdateSessionCountersEx);
        }

        private void UpdateSessionCountersEx()
        {
            if (!_influxDbAgent.IsEnabled) 
                return;

            var mutex = new Mutex(false, "UpdateSessionCounters");
            try
            {
                mutex.WaitOne();

                //TODO: Lock this update with mutex
                var last = GetLastSessionCounterTime();
                var sessions = _repository.GetSessions().Where(x => x.ServerStartTime >= last || x.ServerEndTimeCalculated() >= last).ToArray();

                var startTimes = sessions.Select(x => x.ServerStartTime);
                var endTimes = sessions.Select(x => x.ServerEndTimeCalculated()).OrderBy(x => x);
                var timePoints = startTimes.Union(endTimes).Where(x => (x - last).TotalSeconds >= 1 && x <= DateTime.UtcNow).OrderBy(x => x).ToArray();

                var datas = new List<Dictionary<string, object>>();
                foreach (var timePoint in timePoints)
                {
                    var inSpanSessions = sessions.Where(x => x.ServerStartTime <= timePoint && (x.ServerEndTime ?? x.ServerStartTime.AddMinutes(15)) >= timePoint).ToArray();
                    var time = timePoint.ToInfluxTime();
                    foreach (var avSessions in inSpanSessions.GroupBy(x => new { x.ApplicationId, x.ApplicationVersionId, x.Environment, x.MachineFingerprint, x.CallerIp, x.UserFingerprint }))
                    {
                        var count = avSessions.Count(x => x.ServerStartTime <= timePoint && (x.ServerEndTime ?? x.ServerStartTime.AddMinutes(15)) > timePoint);
                        var data = new Dictionary<string, object>
                        {
                            { "time", time },
                            { "ApplicationId", avSessions.Key.ApplicationId },
                            { "ApplicationVersionId", avSessions.Key.ApplicationVersionId },
                            { "Environment", avSessions.Key.Environment },
                            { "MachineFingerprint", avSessions.Key.MachineFingerprint },
                            { "CallerIp", avSessions.Key.CallerIp },
                            { "UserFingerprint", avSessions.Key.UserFingerprint },
                            { "Total", count },
                        };

                        datas.Add(data);
                    }
                    Register("Session", datas.ToArray());
                    datas.Clear();
                }

            }
            catch (Exception exception)
            {
                _eventLogAgent.WriteToEventLog(exception, EventLogEntryType.Warning);
            }
            finally
            {
                mutex.ReleaseMutex();
            }
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