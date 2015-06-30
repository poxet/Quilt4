using System;
using System.Collections.Generic;
using System.Linq;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.Web.BusinessEntities;
using Tharga.Quilt4Net;

namespace Quilt4.Web.Business
{
    public class InitiativeBusiness : IInitiativeBusiness
    {
        private readonly IRepository _repository;
        

        public InitiativeBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<IInitiative> GetAllByDeveloper(string developerName)
        {
            var initiatives = _repository.GetInitiativesByDeveloper(developerName).ToList();
            if (!initiatives.Any())
            {
                var defaultInitiative = new Initiative(Guid.NewGuid(), null, GenerateClientToken(), developerName ?? "*", new List<IDeveloperRole>(), new List<ApplicationGroup>());
                initiatives.Insert(0, defaultInitiative);
                _repository.AddInitiative(defaultInitiative);
            }
            return initiatives;
        }

        public void UpdateInitiative(Guid id, string name, string sessionToken, string owner)
        {
            _repository.UpdateInitiative(id, name, sessionToken, owner);
        }

        public void UpdateInitiative(IInitiative initiative)
        {
            _repository.UpdateInitiative(initiative);
        }

        public void DeleteInitiative(string id)
        {
            var guidId = Guid.Parse(id);
            
            var applicationIds = _repository.GetApplicationGroups(guidId).SelectMany(x => x.Applications).Select(y => y.Id);

            foreach (var applicationId in applicationIds)
            {
                _repository.DeleteApplicationVersionForApplication(applicationId);
                _repository.DeleteSessionForApplication(applicationId);
            }

            _repository.DeleteInitiative(Guid.Parse(id));
            
        }

        public IEnumerable<IApplicationGroup> GetApplicationGroups(Guid initiativeId)
        {
            var initiatives = _repository.GetApplicationGroups(initiativeId).ToList();
            return initiatives;
        }

        private IEnumerable<IInitiativeHead> GetAllHeadsByDeveloper(string developerName)
        {
            var initiatives = _repository.GetInitiativeHeadsByDeveloper(developerName).ToList();
            if (!initiatives.Any())
            {
                var defaultInitiative = new Initiative(Guid.NewGuid(), null, GenerateClientToken(), developerName ?? "*", new List<IDeveloperRole>(), new List<ApplicationGroup>());
                initiatives.Insert(0, defaultInitiative);
                _repository.AddInitiative(defaultInitiative);
            }
            return initiatives;
        }

        public IEnumerable<IInitiativeHead> GetInitiativesByDeveloper(string developerName)
        {
            //var ib = new InitiativeBusiness(_repository);
            var initiatives = GetAllHeadsByDeveloper(developerName).ToArray();
            //var response = initiatives.OrderBy(x => x.Name).Select(initiative => initiative.ToViewModel(new List<ISession>(), new List<IApplicationVersion>()));
            //var response = initiatives.OrderBy(x => x.Name).Select(x => new Initiative(x.Id, x.Name, x.ClientToken, x.OwnerDeveloperName, x.DeveloperRoles, x.ApplicationGroups));
            var response = initiatives.OrderBy(x => x.Name);
            return response;
        }

        private string GenerateClientToken()
        {
            var response = RandomUtility.GetRandomString(32);
            return response;
        }

        public void Create(string developerName, string initiativename)
        {
            developerName = developerName ?? "*";

            var initiative = new Initiative(Guid.NewGuid(), initiativename, GenerateClientToken(), developerName, new List<IDeveloperRole>(), new List<ApplicationGroup>());
            _repository.AddInitiative(initiative);
        }

        public IInitiative GetInitiative(Guid initiativeId)
        {
            return _repository.GetInitiative(initiativeId);
        }

        public IInitiative GetInitiative(string developerName, string initiativeIdentifier)
        {
            //The initiativeIdentifier can be the name, id or client token of the initiative.

            var initiativesByDeveloper = _repository.GetInitiativesByDeveloper(developerName).ToArray();

            var initiativeId = Guid.Empty;

            //Try the name as identifier
            var initiativeHeads = initiativesByDeveloper.Where(x => (x.Name ?? Models.Constants.DefaultInitiativeName) == initiativeIdentifier).ToArray();
            if (initiativeHeads.Count() == 1)
                initiativeId = initiativeHeads.Single().Id;

            //Try the id as identifier
            if (initiativeId == Guid.Empty)
            {
                initiativeHeads = initiativesByDeveloper.Where(x => x.Id.ToString() == initiativeIdentifier).ToArray();
                if (initiativeHeads.Count() == 1)
                    initiativeId = initiativeHeads.Single().Id;
            }

            //Try the clientToken as identifier
            if (initiativeId == Guid.Empty)
            {
                initiativeHeads = initiativesByDeveloper.Where(x => x.ClientToken == initiativeIdentifier).ToArray();
                if (initiativeHeads.Count() == 1)
                    initiativeId = initiativeHeads.Single().Id;
            }

            if (initiativeId == Guid.Empty)
                throw new InvalidOperationException("Cannot find a single initiative with the provided identifier for the developer.").AddData("developerName", developerName).AddData("initiativeIdentifier", initiativeIdentifier);

            var initiative = GetInitiative(initiativeId);
            return initiative;
        }

        public int GetInitiativeCount()
        {

            return _repository.GetInitiativeCount();
        }

        public int GetApplicationCount()
        {
            return _repository.GetApplicationCount();
        }

        public int GetIssueTypeCount()
        {
            return _repository.GetIssueTypeCount();
        }

        public int GetIssueCount()
        {
            return _repository.GetIssueCount();
        }

        public IInitiative GetInitiativeByApplication(Guid applicationId)
        {
            return _repository.GetInitiativeByApplication(applicationId);
        }

        public IApplication GetApplication(Guid applicationId)
        {
            var initiative = _repository.GetInitiativeByApplication(applicationId);
            var applicationGroup = initiative.ApplicationGroups.Single(x => x.Applications.Any(y => y.Id == applicationId));
            var app = applicationGroup.Applications.Single(x => x.Id == applicationId);
            return app;
        }

        public IApplication RegisterApplication(IClientToken clientToken, string applicationName, string applicationVersionFingerprint)
        {
            if (clientToken == null) throw new ArgumentNullException("clientToken");
            if (applicationName == null) throw new ArgumentNullException("applicationName");

            var initiative = _repository.GetInitiativeByClientToken((ClientToken)clientToken);

            if (initiative == null) throw new InvalidOperationException("Invalid client token.");

            var application = initiative.ApplicationGroups.SelectMany(x => x.Applications).SingleOrDefault(x => x.Name == applicationName);
            if (application != null)
                return application;

            var applicationGroup = GetDefaultApplicationGroup(initiative);

            application = new Application(Guid.NewGuid(), applicationName, DateTime.UtcNow, null, null, null, null);
            applicationGroup.Add(application);

            _repository.UpdateInitiative(initiative);

            return application;
        }

        private static IApplicationGroup GetDefaultApplicationGroup(IInitiative initiative)
        {
            var applicationGroup = initiative.ApplicationGroups.SingleOrDefault(x => x.Name == null);
            if (applicationGroup == null)
            {
                applicationGroup = new ApplicationGroup(null, new List<IApplication>());
                initiative.AddApplicationGroup(applicationGroup);
            }
            return applicationGroup;
        }

        public void SaveInitiative(IInitiative initiative)
        {
            if (initiative.Name == string.Empty)
                initiative.Name = null;

            _repository.UpdateInitiative(initiative);
        }

        public void DeleteInitiative(Guid initiativeId)
        {
            _repository.DeleteInitiative(initiativeId);
        }

        public void DeleteApplication(Guid applicationId)
        {
            var initiative = GetInitiativeByApplication(applicationId);
            var applicationGroup = initiative.ApplicationGroups.Single(x => x.Applications.Any(y => y.Id == applicationId));
            var app = applicationGroup.Applications.Single(x => x.Id == applicationId);
            applicationGroup.Remove(app);

            DeleteApplicationVersions(applicationId);

            SaveInitiative(initiative);
        }

        private void DeleteApplicationVersions(Guid applicationId)
        {
            var versions = _repository.GetApplicationVersions(applicationId);
            foreach (var version in versions)
                _repository.DeleteApplicationVersion(version.Id);
        }

        public IEnumerable<IInitiative> GetInitiatives()
        {
            return _repository.GetInitiatives();
        }

        public IEnumerable<IIssue> GetIssueStatistics(DateTime fromDate, DateTime toDate)
        {
            return _repository.GetIssueStatistics(fromDate, toDate);
        }

        public IEnumerable<IInviteApproval> GetPendingApprovals(string developerEMail)
        {
            var initiatives = _repository.GetInitiatives().Where(x => x.DeveloperRoles.Any(y => y.InviteEMail == developerEMail && y.RoleName == "Invited")).ToArray();
            return initiatives.Select(x =>
            {
                var developerRole = x.DeveloperRoles.Single(y => y.InviteEMail == developerEMail && y.RoleName == "Invited");
                return new InviteApproval
                {
                    InitiativeId = x.Id,
                    InitiativeName = x.Name,
                    Message = developerRole.RoleName,
                    InviteTime = developerRole.InviteTime,
                    InitiativeOwner = x.OwnerDeveloperName,
                    InviteCode = developerRole.InviteCode
                };
            });
        }
    }
}