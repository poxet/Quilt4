using System;
using System.Collections.Generic;
using System.Linq;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.Web.BusinessEntities;

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

        public IEnumerable<IApplicationGroup> GetApplicationGroups(Guid initiativeId)
        {
            var initiatives = _repository.GetApplicationGroups(initiativeId).ToList();
            return initiatives;
        }

        public IEnumerable<IInitiative> GetAllHeadsByDeveloper(string developerName)
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

            application = new Application(Guid.NewGuid(), applicationName, DateTime.UtcNow, null);
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