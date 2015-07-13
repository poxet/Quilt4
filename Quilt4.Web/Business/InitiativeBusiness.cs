using System;
using System.Collections.Generic;
using System.Linq;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.Web.Models;
using Tharga.Quilt4Net;

namespace Quilt4.Web.Business
{
    public class InitiativeBusiness : IInitiativeBusiness
    {
        private readonly IRepository _repository;
        private readonly ICounterBusiness _counterBusiness;

        public InitiativeBusiness(IRepository repository, ICounterBusiness counterBusiness)
        {
            _repository = repository;
            _counterBusiness = counterBusiness;
        }

        public void UpdateInitiative(Guid id, string name, string sessionToken, string owner)
        {
            _repository.UpdateInitiative(id, name, sessionToken, owner);
        }

        public void UpdateInitiative(IInitiative initiative)
        {
            var ini = new Initiative(initiative.Id, initiative.Name, initiative.ClientToken, initiative.OwnerDeveloperName, initiative.DeveloperRoles, initiative.ApplicationGroups.Where(x => x.Applications.Any()));
            _repository.UpdateInitiative(ini);
        }

        public void DeleteInitiative(Guid id)
        {
            var applicationIds = _repository.GetApplicationGroups(id).SelectMany(x => x.Applications).Select(y => y.Id);

            foreach (var applicationId in applicationIds)
            {
                _repository.DeleteApplicationVersionForApplication(applicationId);
                _repository.DeleteSessionForApplication(applicationId);
            }

            _repository.DeleteInitiative(id);
        }

        public IEnumerable<IApplicationGroup> GetApplicationGroups(Guid initiativeId)
        {
            var initiatives = _repository.GetApplicationGroups(initiativeId).ToList();
            return initiatives;
        }

        public IEnumerable<IInvitation> GetInvitations(string email)
        {
            return _repository.GetInvitations(email);
        }

        public string GenerateInviteMessage(string initiativeid, string code, string message, Uri url)
        {
            var initiative = _repository.GetInitiative(Guid.Parse(initiativeid));
            var route = url.AbsolutePath.Replace(url.Segments.Last(), "");

            var acceptLink = url.GetLeftPart(UriPartial.Authority) + route + "Accept?id=" + initiativeid + "&inviteCode=" + code;
            var declineLink = url.GetLeftPart(UriPartial.Authority) + route + "Decline?id=" + initiativeid + "&inviteCode=" + code;

            if(!message.Equals(string.Empty)) { return initiative.OwnerDeveloperName + " want to invite you to initiative " + initiative.Name + " at Quilt4. <br/> Message: " + message + "<br/><br/><a href='" + acceptLink + "'>Accept</a><br/><a href='" + declineLink + "'>Decline</a>"; }
            return initiative.OwnerDeveloperName + " want to invite you to initiative " + initiative.Name + " at Quilt4. <br/><br/><a href='" + acceptLink + "'>Accept</a><br/><a href='" + declineLink + "'>Decline</a>";
        }

        private IEnumerable<IInitiativeHead> GetHeadsByDeveloper(string developerName, string[] roleNames)
        {
            var initiatives = _repository.GetInitiativeHeadsByDeveloper(developerName, roleNames).ToList();
            if (!initiatives.Any())
            {
                var defaultInitiative = new Initiative(Guid.NewGuid(), null, GenerateClientToken(), developerName ?? "*", new List<IDeveloperRole>(), new List<ApplicationGroup>());
                initiatives.Insert(0, defaultInitiative);
                _repository.AddInitiative(defaultInitiative);
            }
            return initiatives;
        }

        public IEnumerable<IInitiativeHead> GetInitiativesByDeveloperOwner(string developerName)
        {
            var initiatives = GetHeadsByDeveloper(developerName, new[] { RoleNameConstants.Owner }).ToArray();
            var response = initiatives.OrderBy(x => x.Name);
            return response;
        }

        public IEnumerable<IInitiativeHead> GetInitiativesByDeveloper(string developerName)
        {
            var initiatives = GetHeadsByDeveloper(developerName, new[] { RoleNameConstants.Owner, RoleNameConstants.Administrator }).ToArray();
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

            var initiativesByDeveloper = _repository.GetInitiativeHeadsByDeveloper(developerName, new[] { RoleNameConstants.Owner, RoleNameConstants.Administrator }).ToArray();

            var initiativeId = Guid.Empty;

            //Try the name as identifier
            var initiativeHeads = initiativesByDeveloper.Where(x => (x.Name ?? Constants.DefaultInitiativeName) == initiativeIdentifier).ToArray();
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
            {
                return application;
            }

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

            _repository.DeleteSessionForApplication(applicationId);

            foreach (var version in versions)
                _repository.DeleteApplicationVersion(version.Id);
        }

        public void DeleteApplicationVersion(string applicationVersionFingerprint)
        {
            _repository.DeleteApplicationVersion(applicationVersionFingerprint);
        }

        public void ArchiveApplicationVersion(string versionId)
        {
            _repository.ArchiveApplicationVersion(versionId);
        }

        public IEnumerable<IDictionary<string, string>> GetEnvironmentColors(string userId)//IDeveloper.UserId
        {
            return _repository.GetEnvironmentColors(userId);
        }

        public void UpdateEnvironmentColors(string userId, IEnumerable<IDictionary<string, string>> environmentColors)
        {
            _repository.UpdateEnvironmentColors(userId, environmentColors);
        }

        public void AddEnvironmentColors(string userId, IEnumerable<IDictionary<string, string>> environmentColors)
        {
            _repository.AddEnvironmentColors(userId, environmentColors);
        }

        public IInitiative GetInitiativeByInviteCode(string inviteCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IInitiative> GetInitiatives()
        {
            return _repository.GetInitiatives();
        }

        public IEnumerable<IIssue> GetIssueStatistics(DateTime fromDate, DateTime toDate)
        {
            return _repository.GetIssueStatistics(fromDate, toDate);
        }

        public void DeclineInvitation(string inviteCode)
        {
            var initiatives = _repository.GetInitiatives();
            var initiative = initiatives.Single(x => x.DeveloperRoles.Any(y => y.InviteCode == inviteCode));
            var item = initiative.DeveloperRoles.FirstOrDefault(x => string.Compare(x.InviteCode, inviteCode, StringComparison.InvariantCultureIgnoreCase) == 0);
            if (item == null) throw new NullReferenceException(string.Format("Cannot find invitation with provided code."));
            item.RoleName = RoleNameConstants.Declined;
            item.InviteCode = string.Empty;

            _repository.UpdateInitiative(initiative);
        }

        public void ConfirmInvitation(Guid initiativeId, string developerName)
        {
            var initiative = _repository.GetInitiative(initiativeId);

            var item = initiative.DeveloperRoles.FirstOrDefault(x => string.Compare(x.DeveloperName, developerName, StringComparison.InvariantCultureIgnoreCase) == 0 || string.Compare(x.InviteEMail, developerName, StringComparison.InvariantCultureIgnoreCase) == 0);
            if (item == null) throw new NullReferenceException(string.Format("Cannot find invitation for developer.")).AddData("InitiativeId", initiativeId).AddData("DeveloperName", developerName);
            item.DeveloperName = developerName;
            item.RoleName = RoleNameConstants.Administrator;
            item.InviteResponseTime = DateTime.Now;
            item.InviteCode = string.Empty;

            _repository.UpdateInitiative(initiative);
        }
    }
}