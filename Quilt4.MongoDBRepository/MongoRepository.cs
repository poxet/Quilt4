using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Quilt4.Interface;
using Quilt4.MongoDBRepository.Entities;

namespace Quilt4.MongoDBRepository
{
    public class MongoRepository : IRepository
    {
        //private static readonly object SyncRoot = new object();
        //private string _databaseName;
        //private MongoServer _server;
        //private MongoDatabase _database;

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

        public void LogEmail(string fromEmail, string to, string subject, string body, DateTime dateSent)
        {
            var emailLog = new EmailLogPersist();

            emailLog.Id = Guid.NewGuid();
            emailLog.FromEmail = fromEmail;
            emailLog.ToEmail = to;
            emailLog.Subject = subject;
            emailLog.Body = body;
            emailLog.DateSent = dateSent;

            Database.GetCollection("EmailLog").Save(emailLog, WriteConcern.Acknowledged);

        }

        public IEnumerable<IEmail> GetLastHundredEmails()
        {
            var emails = Database.GetCollection("EmailLog").FindAllAs<EmailLogPersist>().OrderBy(x => x.DateSent).Take(100);

            return emails;
        }

        //public string DatabaseName 
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_databaseName))
        //        {
        //            _databaseName = System.Configuration.ConfigurationManager.AppSettings["MongoDbName"];
        //            if (string.IsNullOrEmpty(_databaseName))
        //                _databaseName = "Quilt4Net";
        //        }
        //        return _databaseName;
        //    }
        //    set
        //    {
        //        if (!string.IsNullOrEmpty(_databaseName) && _databaseName != value)
        //            throw new InvalidOperationException("Cannot change database name once it has been set.");
        //        _databaseName = value;
        //    }
        //}

        //public MongoServer Server
        //{
        //    get
        //    {
        //        if (_server != null)
        //            return _server;

        //        lock (SyncRoot)
        //        {
        //            if (_server == null)
        //                _server = new MongoServer(new MongoServerSettings {Server = GetMongoServerAddress()});
        //        }
        //        return _server;
        //    }
        //    set
        //    {
        //        if (_server != null)
        //            throw new InvalidOperationException("Cannot change server once it has been initialized.");
        //        _server = value;
        //    }
        //}

        //TODO: Duplicate code
        private MongoDatabase GetDatabaseFromUrl(MongoUrl url)
        {
            //var client = new MongoClient(url);
            if (url.DatabaseName == null)
            {
                throw new Exception("No database name specified in connection string");
            }
            //return client.GetDatabase(,); // WriteConcern defaulted to Acknowledged
            //return client.GetDatabase()
            return new MongoDatabase(new MongoServer(new MongoServerSettings { Server = url.Server }), url.DatabaseName, new MongoDatabaseSettings { });
        }

        private MongoDatabase Database
        {
            get
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

                return db;

        //        if (_database != null)
        //            return _database;

        //        lock (SyncRoot)
        //        {
        //            if (_database == null)
        //            {
        //                _database = Server.GetDatabase(DatabaseName);
        //                _database.GetCollection("Initiative").CreateIndex(new IndexKeysBuilder().Ascending("ClientToken"), IndexOptions.SetUnique(true));
        //            }
        //        }
        //        return _database;
                throw new InvalidOperationException();
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

        private static MongoServerAddress GetMongoServerAddress()
        {
            var mongoServerAddress = new MongoServerAddress("localhost", 27017);

            var mongoDbServerAddress = System.Configuration.ConfigurationManager.AppSettings["MongoDbServerAddress"];
            if (!string.IsNullOrEmpty(mongoDbServerAddress))
            {
                var parts = mongoDbServerAddress.Split(':');

                var host = mongoServerAddress.Host;
                var port = mongoServerAddress.Port;
                
                if (parts.Length >= 1) 
                    host = parts[0];

                if (parts.Length >= 2)
                {
                    if (!int.TryParse(parts[1], out port))
                        throw new InvalidOperationException("Unable to parse setting MongoDbServerAddress port to integer. (host:port).");
                }

                mongoServerAddress = new MongoServerAddress(host, port);
            }

            return mongoServerAddress;
        }

        public void AddInitiative(IInitiative initiative)
        {
            Database.GetCollection("Initiative").Insert(initiative.ToPersist(), WriteConcern.Acknowledged);
        }

        public void UpdateInitiative(IInitiative initiative)
        {
            Database.GetCollection("Initiative").Save(initiative.ToPersist(), WriteConcern.Acknowledged);
        }

        public IEnumerable<IInitiative> GetInitiativesByDeveloper(string developerName)
        {
            // TODO: Exact same statement in two places
            var initiativePersists = Database.GetCollection("Initiative").FindAllAs<InitiativePersist>().Where(x => x.OwnerDeveloperName == "*" 
                || string.Compare(x.OwnerDeveloperName, developerName, StringComparison.InvariantCultureIgnoreCase) == 0 
                || (x.DeveloperRoles != null && x.DeveloperRoles.Any(xx => string.Compare(xx.DeveloperName, developerName, StringComparison.InvariantCultureIgnoreCase) == 0)));
            var initiatives = initiativePersists.Select(x => x.ToEntity()).ToArray();
            return initiatives;
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

        public IEnumerable<IInitiative> GetInitiativeHeadsByDeveloper(string developerName)
        {
            var initiativePersists = Database.GetCollection("Initiative").FindAllAs<InitiativePersist>().Where(x => x.OwnerDeveloperName == "*"
                || string.Compare(x.OwnerDeveloperName, developerName, StringComparison.InvariantCultureIgnoreCase) == 0
                || (x.DeveloperRoles != null && x.DeveloperRoles.Any(xx => string.Compare(xx.DeveloperName, developerName, StringComparison.InvariantCultureIgnoreCase) == 0)));
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

        public IEnumerable<IApplicationVersion> GetApplicationVersions(Guid applicationId)
        {
            var query = Query.EQ("ApplicationId", applicationId);
            var applicationVersionPersists = Database.GetCollection("ApplicationVersion").FindAs<ApplicationVersionPersist>(query);
            var response = applicationVersionPersists.Select(x => x.ToEntity());
            return response;
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForDeveloper(string developerName)
        {
            var applicationIds = GetSessionsForDeveloper(developerName).GroupBy(x => x.ApplicationId);
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

        public IEnumerable<ISession> GetSessionsForApplications(IEnumerable<Guid> applicationIds)
        {
            var allSessions = Database.GetCollection("Session").FindAllAs<SessionPersist>().Where(x => applicationIds.Contains(x.ApplicationId)).ToArray();
            var response = allSessions.Select(x => x.ToEntity()).ToArray();
            return response; 
        }

        public void UpdateMachine(IMachine machine)
        {
            Database.GetCollection("Machine").Save(machine.ToPersist());
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
            //TODO: Delete sessions for this application version
            //var sessions = Database.GetCollection("Session").FindAllAs<SessionPersist>().Where(x => x.ApplicationVersionId == applicationVersionFingerprint).ToArray();
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

        public IEnumerable<ISession> GetSessionsForDeveloper(string developerName)
        {
            //TODO: Rewrite this to a direct mongodb query
            var initiatives = GetInitiativesByDeveloper(developerName);
            var applications = initiatives.SelectMany(x => x.ApplicationGroups).SelectMany(x => x.Applications);
            var allSessions = Database.GetCollection("Session").FindAllAs<SessionPersist>().ToArray();
            var sessionPersists = allSessions.Where(x => applications.Any(y => y.Id == x.ApplicationId));
            var response = sessionPersists.Select(x => x.ToEntity()).ToArray();
            return response;
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
            return Database.GetCollection("Machine").FindAllAs<MachinePersist>().Where(x => sessions.Any(y => y.MachineFingerprint == x.Id)).Select(x => x.ToEntity());
        }

        public void RegisterToolkitCompability(Version serverVersion, DateTime registerDate, string supportToolkitNameVersion, ECompatibility compatibility)
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
            else if ((ECompatibility)item.Compatibility != compatibility)
            {
                item.Compatibility = (int)ECompatibility.Inconclusive;
                item.RegisterDate = registerDate;
                toolkitCompability.Save(item, WriteConcern.Acknowledged);
            }
        }

        public IEnumerable<IToolkitCompatibilities> GetToolkitCompability(Version version)
        {
            var toolkitCompability = Database.GetCollection("ToolkitCompability");
            var query = Query.EQ("ServerVersion", version.ToString());
            var toolkitCompabilities = toolkitCompability.FindAs<ToolkitCompabilityPersist>(query).Where(x => !string.IsNullOrEmpty(x.SupportToolkitNameVersion)).Select(x => x.ToEntity()).ToList();
            var firstRegisterDate = toolkitCompabilities.Min(x => x.RegisterDate) ?? new DateTime(2014,4,28,16,20,0);

            var supportToolkitNameVersions = Database.GetCollection("ApplicationVersion").FindAllAs<ApplicationVersionPersist>().GroupBy(x => x.SupportToolkitNameVersion);

            //Find applications to exclude
            var query2 = Query.ElemMatch("ApplicationGroups", Query.ElemMatch("Applications", Query.EQ("Name", "ToolkitCompatibilityTester")));
            var initiativePersists = Database.GetCollection("Initiative").FindAs<InitiativePersist>(query2).ToArray();
            var excludeApplications = initiativePersists.SelectMany(x => x.ApplicationGroups).SelectMany(x => x.Applications).Where(x => x.Name == "ToolkitCompatibilityTester").ToArray();

            foreach (var supportToolkitNameVersion in supportToolkitNameVersions)
            {
                var sessions = Database.GetCollection("Session").FindAllAs<SessionPersist>().Where(x =>  excludeApplications.All(y => y.Id != x.ApplicationId) &&  supportToolkitNameVersion.Select(y => y.Id).Any(y => x.ApplicationVersionId == y)).ToArray();
                var lastUsed = sessions.Any() ? sessions.Max(y => y.ServerStartTime) : (DateTime?)null;

                var item = toolkitCompabilities.SingleOrDefault(x => x.SupportToolkitNameVersion == supportToolkitNameVersion.Key);
                if (item == null)
                {
                    var lastUsedWithThisService = lastUsed > firstRegisterDate ? lastUsed : (DateTime?)null;
                    item = new ToolkitCompability(version, null, supportToolkitNameVersion.Key, ECompatibility.Unknown, lastUsedWithThisService);
                    toolkitCompabilities.Add(item);
                }
                else 
                    item.LastUsed = lastUsed;
            }

            return toolkitCompabilities;
        }
    }
}