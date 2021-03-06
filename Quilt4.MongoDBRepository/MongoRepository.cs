﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.MongoDBRepository.Data;
using Quilt4.MongoDBRepository.Entities;

namespace Quilt4.MongoDBRepository
{
    public class MongoRepository : IRepository
    {
        private static readonly object SyncRoot = new object();
        private MongoDatabase _database;
        internal static event EventHandler<RequestUpdateEntityEventArgs> RequestUpdateEntityEvent;
        internal static event EventHandler<RequestDeleteEntityEventArgs> RequestDeleteEntityEvent;

        internal static void InvokeRequestDeleteEntityEvent(RequestDeleteEntityEventArgs e)
        {
            var handler = RequestDeleteEntityEvent;
            if (handler != null) handler(null, e);
        }

        internal static void InvokeRequestUpdateEntityEvent(RequestUpdateEntityEventArgs e)
        {
            var handler = RequestUpdateEntityEvent;
            if (handler != null) handler(null, e);
        }

        public MongoRepository()
        {
            RequestUpdateEntityEvent += MongoRepository_RequestUpdateEntityEvent;
            RequestDeleteEntityEvent += MongoRepository_RequestDeleteEntityEvent;
        }

        public void UpdateInitiative(Guid id, string name, string sessionToken, string owner)
        {
            var initiative = Database.GetCollection("Initiative").FindAllAs<InitiativePersist>().FirstOrDefault(x => x.Id == id);

            initiative.Name = name;
            initiative.ClientToken = sessionToken;
            initiative.OwnerDeveloperName = owner;

            Database.GetCollection("Initiative").Save(initiative, WriteConcern.Acknowledged);
        }

        public IDataBaseInfo GetDatabaseStatus()
        {
            var dbInfo = new DataBaseInfoEntity();

            if (Database.Server.State != MongoServerState.Connected)
            {
                dbInfo.Online = false;
            }
            else
            {
                dbInfo.Online = true;
            }

            dbInfo.Name = Database.Name;
            dbInfo.Server = Database.Server.Settings.Server.Host + ":" + Database.Server.Settings.Server.Port;

            return dbInfo;
        }

        public void LogEmail(string fromEmail, string to, string subject, string body, DateTime dateSent, bool status, string errorMessage)
        {
            var emailLog = new EmailLogPersist();

            emailLog.Id = Guid.NewGuid();
            emailLog.FromEmail = fromEmail;
            emailLog.ToEmail = to;
            emailLog.Subject = subject;
            emailLog.Body = body;
            emailLog.DateSent = dateSent;
            emailLog.Status = status;
            emailLog.ErrorMessage = errorMessage;

            Database.GetCollection("EmailLog").Save(emailLog, WriteConcern.Acknowledged);
        }

        public IEnumerable<IEmail> GetLastHundredEmails()
        {
            var emails = Database.GetCollection("EmailLog").FindAllAs<EmailLogPersist>().OrderByDescending(x => x.DateSent).Take(100);

            return emails;
        }

        public int GetInitiativeCount()
        {
            var initiativeCount = (int)Database.GetCollection("Initiative").Count();

            return initiativeCount;
        }

        public int GetApplicationCount()
        {
            var applicationCount = (int)Database.GetCollection("Initiative").FindAllAs<InitiativePersist>().SelectMany(y => y.ApplicationGroups).Select(z => z.Applications).Count();

            return applicationCount;
        }

        public int GetIssueTypeCount()
        {
            var issueTypeCount = Database.GetCollection("ApplicationVersion").FindAllAs<ApplicationVersionPersist>().Select(y => y.IssueTypes).Count();

            return issueTypeCount;
        }

        public int GetIssueCount()
        {
            var issueCount = Database.GetCollection("ApplicationVersion").FindAllAs<ApplicationVersionPersist>().SelectMany(y => y.IssueTypes).Select(z => z.Issues).Count();

            return issueCount;
        }

        public ISetting GetSetting(string name)
        {
            var query = Query.EQ("_id", name);
            var item = Database.GetCollection("Setting").FindOneAs<SettingPersist>(query);

            if (item == null)
                return null;

            var result = item.ToEntity();
            return result;
        }

        public IEnumerable<ISetting> GetSettings()
        {
            var items = Database.GetCollection("Setting").FindAllAs<SettingPersist>();
            var result = items.Select(x => x.ToEntity());
            return result;
        }

        public void SetSetting(ISetting setting)
        {
            var settingPersist = new SettingPersist { Id = setting.Name, Value = setting.Value, Type = setting.Type, Encrypted = setting.Encrypted };
            Database.GetCollection("Setting").Save(settingPersist, WriteConcern.Acknowledged);
        }

        public void DeleteSessionForApplication(Guid applicationId)
        {
            var query = Query.EQ("ApplicationId", applicationId);
            Database.GetCollection("Session").Remove(query, WriteConcern.Acknowledged);
        }

        public IEnumerable<ISession> GetSessionsForUser(string userId)
        {
            return Database.GetCollection("Session").FindAllAs<SessionPersist>().Where(x => x.UserFingerprint == userId).Select(x => x.ToEntity()).ToArray();
        }

        public void ArchiveApplicationVersion(string versionId)
        {
            var versionPersist = Database.GetCollection("ApplicationVersion").FindAllAs<ApplicationVersionPersist>().Single(x => x.Id == versionId);
            Database.GetCollection("ApplicationVersionArchive").Insert(versionPersist, WriteConcern.Acknowledged);

            var query = Query.EQ("_id", versionId);
            Database.GetCollection("ApplicationVersion").Remove(query, WriteConcern.Acknowledged);
        }

        public IEnumerable<IApplicationVersion> GetArchivedApplicationVersions(Guid applicationId)
        {
            var query = Query.EQ("ApplicationId", applicationId);
            var applicationVersionPersists = Database.GetCollection("ApplicationVersionArchive").FindAs<ApplicationVersionPersist>(query);
            var response = applicationVersionPersists.Select(x => x.ToEntity());
            return response;
        }

        //TODO: Duplicate code
        private MongoDatabase GetDatabaseFromUrl(MongoUrl url)
        {
            if (url.DatabaseName == null)
            {
                throw new Exception("No database name specified in connection string");
            }

            return new MongoDatabase(new MongoServer(new MongoServerSettings { Server = url.Server }), url.DatabaseName, new MongoDatabaseSettings { });
        }

        private MongoDatabase Database
        {
            get
            {
                if (_database != null)
                    return _database;

                lock (SyncRoot)
                {
                    if (_database == null)
                    {
                        var connectionNameOrUrl = "Mongo";

                        MongoDatabase db;
                        if (connectionNameOrUrl.ToLower().StartsWith("mongodb://"))
                        {
                            db = GetDatabaseFromUrl(new MongoUrl(connectionNameOrUrl));
                        }
                        else
                        {
                            var connStringFromManager = ConfigurationManager.ConnectionStrings[connectionNameOrUrl].ConnectionString;
                            db = GetDatabaseFromUrl(new MongoUrl(connStringFromManager));
                        }

                        _database = db;

                        _database.GetCollection("Setting").DropAllIndexes(); //TODO: If you cannot access the settings table. Enable this line, run the program and disable it again. (This line should be removed when it works on all machines)
                        _database.GetCollection("Setting").CreateIndex(new IndexKeysBuilder().Ascending("_id"), IndexOptions.SetUnique(true));
                        _database.GetCollection("Initiative").CreateIndex(new IndexKeysBuilder().Ascending("ClientToken"), IndexOptions.SetUnique(true));
                        _database.GetCollection("AspNetUsers").CreateIndex(new IndexKeysBuilder().Ascending("UserName"), IndexOptions.SetUnique(true));
                        _database.GetCollection("AspNetUsers").CreateIndex(new IndexKeysBuilder().Ascending("Email"), IndexOptions.SetUnique(true));
                    }
                }

                return _database;
            }
        }

        void MongoRepository_RequestDeleteEntityEvent(object sender, RequestDeleteEntityEventArgs e)
        {
            var query = Query.EQ("_id", e.Id);
            Database.GetCollection(e.Collection).Remove(query, WriteConcern.Acknowledged);
        }

        private void MongoRepository_RequestUpdateEntityEvent(object sender, RequestUpdateEntityEventArgs e)
        {
            Database.GetCollection(e.Collection).Save(e.Item, WriteConcern.Acknowledged);
        }

        public void AddInitiative(IInitiative initiative)
        {
            Database.GetCollection("Initiative").Insert(initiative.ToPersist(), WriteConcern.Acknowledged);
        }

        public void UpdateInitiative(IInitiative initiative)
        {
            Database.GetCollection("Initiative").Save(initiative.ToPersist(), WriteConcern.Acknowledged);
        }

        public IEnumerable<IApplicationGroup> GetApplicationGroups(Guid initiativeId)
        {
            var initiativePersists = Database.GetCollection("Initiative").FindAllAs<InitiativePersist>().Where(x => x.Id == initiativeId).Select(y => y.ApplicationGroups);
            var applicationGroups = new List<IApplicationGroup>();

            foreach (var initiativePersist in initiativePersists)
            {
                applicationGroups.AddRange(initiativePersist.Select(x => x.ToEntity()));
            }

            return applicationGroups;
        }

        public IEnumerable<IInvitation> GetInvitations(string email)
        {
            var initiativePersists = Database.GetCollection("Initiative").FindAllAs<InitiativePersist>().Where(x => (x.DeveloperRoles != null && x.DeveloperRoles.Any(xx => string.Compare(xx.InviteEMail, email, StringComparison.InvariantCultureIgnoreCase) == 0 && string.Compare(xx.RoleName, RoleNameConstants.Invited, StringComparison.InvariantCultureIgnoreCase) == 0)));
            var invitations = initiativePersists.Select(x => new Invitation(x.Id,x.Name,x.DeveloperRoles.Single(y => string.Compare(y.InviteEMail, email, StringComparison.InvariantCultureIgnoreCase) == 0).InviteCode)).ToArray();
            return invitations;
        }

        public IEnumerable<IInitiativeHead> GetInitiativeHeadsByDeveloper(string developerEmail, string[] roleNames)
        {
            var initiativePersists = Database.GetCollection("Initiative").FindAllAs<InitiativePersist>().Where(x => 
                x.OwnerDeveloperName == "*"
                || (roleNames.Contains(RoleNameConstants.Owner) && string.Compare(x.OwnerDeveloperName, developerEmail, StringComparison.InvariantCultureIgnoreCase) == 0)
                || (x.DeveloperRoles != null 
                    && x.DeveloperRoles.Any(xx => string.Compare(xx.DeveloperName, developerEmail, StringComparison.InvariantCultureIgnoreCase) == 0 
                        && string.Compare(xx.InviteEMail, developerEmail, StringComparison.InvariantCultureIgnoreCase) == 0 
                        && roleNames.Any(z => string.Compare(z, xx.RoleName, StringComparison.InvariantCultureIgnoreCase) == 0))
                    )
                );
            var initiatives = initiativePersists.Select(x => x.ToEntityHead()).ToArray();
            return initiatives;
        }

        public IEnumerable<IInitiative> GetInitiatives()
        {
            var initiativePersists = Database.GetCollection("Initiative").FindAllAs<InitiativePersist>();
            var initiatives = initiativePersists.Select(x => x.ToEntity()).ToArray();
            return initiatives;
        }

        public IInitiative GetInitiative(Guid initiativeId)
        {
            var query = Query.EQ("_id", initiativeId);
            var response = Database.GetCollection("Initiative").FindOneAs<InitiativePersist>(query);
            return response.ToEntity();
        }

        public IInitiative GetInitiativeByClientToken(string clientToken)
        {
            var initiativePersists = Database.GetCollection("Initiative").FindAllAs<InitiativePersist>().Where(x => string.Compare(x.ClientToken, clientToken, StringComparison.InvariantCultureIgnoreCase) == 0);
            var item = initiativePersists.SingleOrDefault();
            var initiatives = item.ToEntity();
            return initiatives;
        }

        public IInitiative GetInitiativeByApplication(Guid applicationId)
        {
            var query = Query.ElemMatch("ApplicationGroups", Query.ElemMatch("Applications", Query.EQ("_id", applicationId)));
            var response = Database.GetCollection("Initiative").FindOneAs<InitiativePersist>(query);
            var initiative = response.ToEntity();
            return initiative;
        }

        public void DeleteInitiative(Guid initiativeId)
        {
            var query = Query.EQ("_id", initiativeId);
            Database.GetCollection("Initiative").Remove(query, WriteConcern.Acknowledged);
        }

        public void UpdateApplicationVersion(IApplicationVersion applicationVersion)
        {
            Database.GetCollection("ApplicationVersion").Save(applicationVersion.ToPersist(), WriteConcern.Acknowledged);
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersions()
        {
            var applicationVersionPersists = Database.GetCollection("ApplicationVersion").FindAllAs<ApplicationVersionPersist>();
            var response = applicationVersionPersists.Select(x => x.ToEntity());
            return response;
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersions(Guid applicationId)
        {
            var query = Query.EQ("ApplicationId", applicationId);
            var applicationVersionPersists = Database.GetCollection("ApplicationVersion").FindAs<ApplicationVersionPersist>(query);
            var response = applicationVersionPersists.Select(x => x.ToEntity());
            return response;
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForDeveloper(string developerEmail)
        {
            var applicationIds = GetSessionsForDeveloper(developerEmail).GroupBy(x => x.ApplicationId);
            var applicationVersion = Database.GetCollection("ApplicationVersion").FindAllAs<ApplicationVersionPersist>().Where(x => applicationIds.Any(y => y.Key == x.ApplicationId));
            var result = applicationVersion.Select(x => x.ToEntity());
            return result;
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForInitiative(Guid initiativeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForApplications(IEnumerable<Guid> applicationIds)
        {
            var applicationVersion = Database.GetCollection("ApplicationVersion").FindAllAs<ApplicationVersionPersist>().Where(x => applicationIds.Contains(x.ApplicationId));
            var result = applicationVersion.Select(x => x.ToEntity());
            return result;
        }

        public IEnumerable<ISession> GetSessions()
        {
            var sessions = Database.GetCollection("Session").FindAllAs<SessionPersist>().Select(x => x.ToEntity()).ToArray();
            return sessions;
        }

        public IDictionary<string, string> GetEnvironmentColors(string userId)
        {
            var environmentColors = Database.GetCollection("UserProperties").FindAllAs<UserPropertiesPersist>().SingleOrDefault(x => x.Id == userId);
            return environmentColors == null ? new Dictionary<string, string>() : environmentColors.EnvironmentColors;
        }

        public void UpdateEnvironmentColors(string userId, IDictionary<string, string> environmentColors)
        {
            var environmentPersist = Database.GetCollection("UserProperties").FindAllAs<UserPropertiesPersist>().Single(x => x.Id == userId);
            environmentPersist.EnvironmentColors = environmentColors;
            Database.GetCollection("UserProperties").Save(environmentPersist, WriteConcern.Acknowledged);
        }

        public void AddEnvironmentColors(string userId, IDictionary<string, string> environmentColors)
        {
            UserPropertiesPersist environmentPersist;
            if (Database.GetCollection("UserProperties").FindAllAs<UserPropertiesPersist>().Any(x => x.Id == userId))
            {
                environmentPersist = Database.GetCollection("UserProperties").FindAllAs<UserPropertiesPersist>().Single(x => x.Id == userId);
                environmentPersist.EnvironmentColors = environmentColors;
                Database.GetCollection("UserProperties").Save(environmentPersist, WriteConcern.Acknowledged);
            }
            else
            {
                environmentPersist = new UserPropertiesPersist() { Id = userId, EnvironmentColors = environmentColors };
                Database.GetCollection("UserProperties").Insert(environmentPersist, WriteConcern.Acknowledged);
            }
        }

        public void ArchiveSessionsForApplicationVersion(string versionId)
        {
            var sessions = GetSessionsForApplicationVersion(versionId);
            //Insert into SessionArchive
            //Remove from Session

            foreach (var session in sessions)
            {
                Database.GetCollection("SessionArchive").Insert(session.ToPersist(), WriteConcern.Acknowledged);
                DeleteSession(session.Id);
            }
            
        }

        public void DeleteSession(Guid sessionId)
        {
            var query = Query.EQ("Id", sessionId);
            Database.GetCollection("Session").Remove(query, WriteConcern.Acknowledged);
        }

        public IApplicationVersion GetApplicationVersionByIssue(Guid issueId)
        {
            var applicationVersion = Database.GetCollection("ApplicationVersion").FindAllAs<ApplicationVersionPersist>().Single(x => x.IssueTypes.Any(y => y.Issues.Any(z => z.Id == issueId))).ToEntity();
            return applicationVersion;
        }

        public IApplication GetApplicationByVersion(string versionId)
        {
            var sessions = GetSessionsForApplicationVersion(versionId);
            
            //var application = sessions.Select(x => Database.GetCollection("Initiative").FindAllAs<ApplicationPersist>().Single(y => y.Id == x.ApplicationId)).First().ToEntity();
            IApplication application;
            foreach (var session in sessions)
            {
                if (Database.GetCollection("Initiative").FindAllAs<InitiativePersist>().SelectMany(x => x.ApplicationGroups).SelectMany(x => x.Applications).Any(x => x.Id == session.ApplicationId))
                {
                    application = Database.GetCollection("Initiative").FindAllAs<InitiativePersist>().SelectMany(x => x.ApplicationGroups).SelectMany(x => x.Applications).Single(x => x.Id == session.ApplicationId).ToEntity();
                    return application;
                }
            }
            return null;
        }

        public IEnumerable<ISession> GetArchivedSessionsForApplications(IEnumerable<Guid> applicationsIds)
        {
            var allSessions = Database.GetCollection("SessionArchive").FindAllAs<SessionPersist>().Where(x => applicationsIds.Contains(x.ApplicationId)).ToArray();
            var response = allSessions.Select(x => x.ToEntity()).ToArray();
            return response;
        }

        public IApplication GetApplication(Guid applicationId)
        {
            var allApplications = GetInitiatives().SelectMany(x => x.ApplicationGroups).SelectMany(x => x.Applications).ToArray();
            IApplication response = null;
            if (allApplications.Any(x => x.Id == applicationId))
            {
                response = allApplications.Single(x => x.Id == applicationId);
            }
            return response;
        }

        public IEnumerable<ISession> GetSessionsForApplications(IEnumerable<Guid> applicationIds)
        {
            var allSessions = Database.GetCollection("Session").FindAllAs<SessionPersist>().Where(x => applicationIds.Contains(x.ApplicationId)).ToArray();
            var response = allSessions.Select(x => x.ToEntity()).ToArray();
            return response;
        }

        public void UpdateMachine(IMachine machine)
        {
            Database.GetCollection("Machine").Save(machine.ToPersist(), WriteConcern.Acknowledged);
        }

        public bool CanConnect()
        {
            try
            {
                var oneSession = Database.GetCollection("Session").Count();
                return oneSession > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<ISession> GetSessionStatistics(DateTime from, DateTime to)
        {
            var query = Query.And(Query.GT("ServerStartTime", from), Query.LT("ServerStartTime", to));
            var response = Database.GetCollection("Session").FindAs<SessionPersist>(query).Select(x => x.ToEntity()).ToArray();
            return response;
        }

        public IEnumerable<IIssue> GetIssueStatistics(DateTime from, DateTime to)
        {
            var avp = Database.GetCollection("ApplicationVersion").FindAllAs<ApplicationVersionPersist>();
            var response = avp.SelectMany(x => x.IssueTypes).SelectMany(x => x.Issues).Where(x => x.ServerTime > from && x.ServerTime < to).Select(x => x.ToEntity()).ToArray();
            return response;
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForMachine(string machineId)
        {
            var applicationVersionIds = GetSessionsForMachine(machineId).GroupBy(x => x.ApplicationVersionId);
            var applicationVersion = Database.GetCollection("ApplicationVersion").FindAllAs<ApplicationVersionPersist>().Where(x => applicationVersionIds.Any(y => y.Key == x.Id));
            var result = applicationVersion.Select(x => x.ToEntity()).ToArray();
            return result;
        }

        public IApplicationVersion UpdateApplicationVersionId(string applicationVersionFingerprint, Guid applicationId)
        {
            var query = Query.EQ("_id", applicationVersionFingerprint);
            var update = Update.Set("ApplicationId", applicationId);
            Database.GetCollection("ApplicationVersion").Update(query, update, WriteConcern.Acknowledged);
            var result = Database.GetCollection("ApplicationVersion").FindOneAs<ApplicationVersionPersist>(query);
            return result.ToEntity();
        }

        public void AddSession(ISession session)
        {
            Database.GetCollection("Session").Insert(session.ToPersist(), WriteConcern.Acknowledged);
        }

        public ISession GetSession(Guid id)
        {
            var query = Query.EQ("_id", id);
            var session = Database.GetCollection("Session").FindOneAs<SessionPersist>(query);
            return session.ToEntity();
        }

        public void UpdateSessionUsage(Guid sessionId, DateTime serverLastKnown)
        {
            var query = Query.EQ("_id", sessionId);
            var update = Update.Set("ServerLastKnown", serverLastKnown);
            Database.GetCollection("Session").Update(query, update, WriteConcern.Acknowledged);
        }

        public IApplicationVersion GetApplicationVersion(string applicationVersionFingerprint)
        {
            var query = Query.EQ("_id", applicationVersionFingerprint);
            var session = Database.GetCollection("ApplicationVersion").FindOneAs<ApplicationVersionPersist>(query);
            return session.ToEntity();
        }

        public void AddApplicationVersion(IApplicationVersion applicationVersion)
        {
            Database.GetCollection("ApplicationVersion").Insert(applicationVersion.ToPersist());
        }

        public void DeleteApplicationVersion(string applicationVersionFingerprint)
        {
            var query = Query.EQ("_id", applicationVersionFingerprint);
            Database.GetCollection("ApplicationVersion").Remove(query, WriteConcern.Acknowledged);
            Database.GetCollection("ApplicationVersionArchive").Remove(query, WriteConcern.Acknowledged);
        }

        public void DeleteApplicationVersionForApplication(Guid applicationId)
        {
            var query = Query.EQ("ApplicationId", applicationId);
            Database.GetCollection("ApplicationVersion").Remove(query, WriteConcern.Acknowledged);
        }

        public IEnumerable<ISession> GetSessionsForApplicationVersion(string applicationVersionId)
        {
            var response = Database.GetCollection("Session").FindAllAs<SessionPersist>().Where(x => x.ApplicationVersionId == applicationVersionId).Select(x => x.ToEntity()).ToArray();
            return response;
        }

        public IEnumerable<ISession> GetSessionsForApplication(Guid applicationId)
        {
            var query = Query.EQ("ApplicationId", applicationId);
            var response = Database.GetCollection("Session").FindAs<SessionPersist>(query).Select(x => x.ToEntity()).ToArray();
            return response;
        }

        public IEnumerable<ISession> GetSessionsForDeveloper(string developerEmail)
        {
            var i = GetInitiatives().Where(x => x.OwnerDeveloperName == developerEmail || x.DeveloperRoles.Any(y => y.DeveloperName == developerEmail)).ToArray();
            var a = i.SelectMany(x => x.ApplicationGroups).SelectMany(x => x.Applications).ToArray();
            var response = Database.GetCollection("Session").FindAllAs<SessionPersist>().Where(x => a.Any(y => y.Id == x.ApplicationId)).Select(x => x.ToEntity()).ToArray();
            return response;

            //throw new NotImplementedException();
            ////TODO: Rewrite this to a direct mongodb query
            //var initiatives = GetInitiativesByDeveloper(developerName);
            //var applications = initiatives.SelectMany(x => x.ApplicationGroups).SelectMany(x => x.Applications);
            //var allSessions = Database.GetCollection("Session").FindAllAs<SessionPersist>().ToArray();
            //var sessionPersists = allSessions.Where(x => applications.Any(y => y.Id == x.ApplicationId));
            //var response = sessionPersists.Select(x => x.ToEntity()).ToArray();
            //return response;
        }

        public IEnumerable<ISession> GetSessionsForApplications(Guid initiativeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISession> GetSessionsForMachine(string machineFingerprint)
        {
            var sessions = Database.GetCollection("Session").FindAllAs<SessionPersist>().Where(x => x.MachineFingerprint == machineFingerprint);
            var response = sessions.Select(x => x.ToEntity()).ToArray();
            return response;
        }

        //public IEnumerable<ISession> GetActiveSessions(int timeoutSeconds)
        //{
        //    var sessions = Database.GetCollection("Session").FindAllAs<SessionPersist>().Where(x => x.ServerEndTime == null && (DateTime.UtcNow - (x.ServerLastKnown ?? x.ServerStartTime)).TotalSeconds < timeoutSeconds);
        //    var response = sessions.Select(x => x.ToEntity()).ToArray();
        //    return response;
        //}

        public void EndSession(Guid sessionId, DateTime serverEndTime)
        {
            var query = Query.EQ("_id", sessionId);
            var update = Update.Set("ServerEndTime", serverEndTime);
            Database.GetCollection("Session").Update(query, update, WriteConcern.Acknowledged);
        }

        public IUser GetUser(string fingerprint)
        {
            var query = Query.EQ("_id", fingerprint);
            var response = Database.GetCollection("User").FindOneAs<UserPersist>(query).ToEntity();
            return response;
        }

        public void AddUser(IUser user)
        {
            Database.GetCollection("User").Insert(user.ToPersist());
        }

        public IEnumerable<IUser> GetUsersByApplicationVersion(string applicationFingerprint)
        {
            var sessions = Database.GetCollection("Session").FindAllAs<SessionPersist>().Where(x => x.ApplicationVersionId == applicationFingerprint).ToArray();
            var userPersists = Database.GetCollection("User").FindAllAs<UserPersist>().Where(x => sessions.Any(y => y.UserFingerprint == x.Id)).ToArray();
            var result = userPersists.Select(x => x.ToEntity()).ToArray();
            return result;
        }

        public IMachine GetMachine(string id)
        {
            var query = Query.EQ("_id", id);
            return Database.GetCollection("Machine").FindOneAs<MachinePersist>(query).ToEntity();
        }

        public void AddMachine(IMachine machine)
        {
            Database.GetCollection("Machine").Insert(machine.ToPersist());
        }

        public IEnumerable<IMachine> GetMachinesByApplicationVersion(string applicationFingerprint)
        {
            var sessions = Database.GetCollection("Session").FindAllAs<SessionPersist>().Where(x => x.ApplicationVersionId == applicationFingerprint).ToArray();
            return Database.GetCollection("Machine").FindAllAs<MachinePersist>().Where(x => sessions.Any(y => y.MachineFingerprint == x.Id)).Select(x => x.ToEntity()).ToArray();
        }

        public IEnumerable<IMachine> GetMachinesByApplicationVersions(IEnumerable<string> applicationFingerprints)
        {
            var sessions = Database.GetCollection("Session").FindAllAs<SessionPersist>().Where(x => applicationFingerprints.Any(y => y == x.ApplicationVersionId)).ToArray();
            return Database.GetCollection("Machine").FindAllAs<MachinePersist>().Where(x => sessions.Any(y => y.MachineFingerprint == x.Id)).Select(x => x.ToEntity()).ToArray();
        }

        public void RegisterToolkitCompability(Version serverVersion, DateTime registerDate, string supportToolkitNameVersion, Compatibility compatibility)
        {
            var serverVersionString = serverVersion.ToString();

            var toolkitCompability = Database.GetCollection("ToolkitCompability");
            var query = Query.And(Query.EQ("ServerVersion", serverVersionString), Query.EQ("SupportToolkitNameVersion", supportToolkitNameVersion));
            var item = toolkitCompability.FindOneAs<ToolkitCompabilityPersist>(query);
            if (item == null)
            {
                var toolkitCompabilityPersist = new ToolkitCompabilityPersist { Id = Guid.NewGuid(), ServerVersion = serverVersionString, RegisterDate = registerDate, SupportToolkitNameVersion = supportToolkitNameVersion, Compatibility = (int)compatibility };
                toolkitCompability.Insert(toolkitCompabilityPersist, WriteConcern.Acknowledged);
            }
            else if ((Compatibility)item.Compatibility != compatibility)
            {
                item.Compatibility = (int)Compatibility.Inconclusive;
                item.RegisterDate = registerDate;
                toolkitCompability.Save(item, WriteConcern.Acknowledged);
            }
        }

        public IEnumerable<IToolkitCompatibilities> GetToolkitCompability(Version version)
        {
            var toolkitCompability = Database.GetCollection("ToolkitCompability");
            var query = Query.EQ("ServerVersion", version.ToString());
            var toolkitCompabilities = toolkitCompability.FindAs<ToolkitCompabilityPersist>(query).Where(x => !string.IsNullOrEmpty(x.SupportToolkitNameVersion)).Select(x => x.ToEntity()).ToList();
            var firstRegisterDate = toolkitCompabilities.Min(x => x.RegisterDate) ?? new DateTime(2014, 4, 28, 16, 20, 0);

            var supportToolkitNameVersions = Database.GetCollection("ApplicationVersion").FindAllAs<ApplicationVersionPersist>().GroupBy(x => x.SupportToolkitNameVersion);

            //Find applications to exclude
            var query2 = Query.ElemMatch("ApplicationGroups", Query.ElemMatch("Applications", Query.EQ("Name", "ToolkitCompatibilityTester")));
            var initiativePersists = Database.GetCollection("Initiative").FindAs<InitiativePersist>(query2).ToArray();
            var excludeApplications = initiativePersists.SelectMany(x => x.ApplicationGroups).SelectMany(x => x.Applications).Where(x => x.Name == "ToolkitCompatibilityTester").ToArray();

            foreach (var supportToolkitNameVersion in supportToolkitNameVersions)
            {
                var sessions = Database.GetCollection("Session").FindAllAs<SessionPersist>().Where(x => excludeApplications.All(y => y.Id != x.ApplicationId) && supportToolkitNameVersion.Select(y => y.Id).Any(y => x.ApplicationVersionId == y)).ToArray();
                var lastUsed = sessions.Any() ? sessions.Max(y => y.ServerStartTime) : (DateTime?)null;

                var item = toolkitCompabilities.SingleOrDefault(x => x.SupportToolkitNameVersion == supportToolkitNameVersion.Key);
                if (item == null)
                {
                    var lastUsedWithThisService = lastUsed > firstRegisterDate ? lastUsed : (DateTime?)null;
                    item = new ToolkitCompability(version, null, supportToolkitNameVersion.Key, Compatibility.Unknown, lastUsedWithThisService);
                    toolkitCompabilities.Add(item);
                }
                else
                {
                    //TODO: Send information on when this toolkit was last userd to the repository
                    //item.LastUsed = lastUsed;
                }
            }

            return toolkitCompabilities;
        }
    }
}