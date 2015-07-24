using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Castle.Core.Internal;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Constants = Quilt4.Web.Models.Constants;

namespace Quilt4.Web.Business
{
    public class ApplicationVersionBusiness : IApplicationVersionBusiness
    {
        private readonly IRepository _repository;
        private readonly ICounterBusiness _counterBusiness;
        private readonly IInitiativeBusiness _initiativeBusiness;

        public ApplicationVersionBusiness(IRepository repository, ICounterBusiness counterBusiness, IInitiativeBusiness initiativeBusiness)
        {
            _repository = repository;
            _counterBusiness = counterBusiness;
            _initiativeBusiness = initiativeBusiness;
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersions(Guid applicationId)
        {
            var response = _repository.GetApplicationVersions(applicationId);
            response = UpdateApplicationVersionsEnvironments(response);
            return response;
        }

        public IFingerprint AssureApplicationFingerprint(string applicationFingerprint, string version, string supportToolkitNameVersion, DateTime? buildTime, string applicationName, string clientToken)
        {
            if (applicationFingerprint == null)
                applicationFingerprint = string.Format("AI2:{0}", string.Format("{0}{1}{2}{3}{4}", applicationName, version, supportToolkitNameVersion, clientToken, buildTime).ToMd5Hash());

            //Make sure that provided data is valid for the provided fingerprint
            var applicationVersion = _repository.GetApplicationVersion(applicationFingerprint);

            if (applicationVersion != null)
            {
                var application = _repository.GetInitiativeByApplication(applicationVersion.ApplicationId).ApplicationGroups.SelectMany(x => x.Applications).First(x => x.Id == applicationVersion.ApplicationId);

                if (application.Name != applicationName) throw new InvalidOperationException("Provided application name does not match the application name stored for the application fingerprint. If the application name has changed a new fingerprint needs to be provided, or provide null and the server will generate a fingerprint for you.");
                if (applicationVersion.Version != version) throw new InvalidOperationException("Provided version does not match the version stored for the application fingerprint. If the version has changed a new fingerprint needs to be provided, or provide null and the server will generate a fingerprint for you.");
                if (applicationVersion.SupportToolkitNameVersion != supportToolkitNameVersion) throw new InvalidOperationException("Provided supportToolkitNameVersion does not match the supportToolkitNameVersion stored for the application fingerprint. If the supportToolkitNameVersion has changed a new fingerprint needs to be provided, or provide null and the server will generate a fingerprint for you.");
                if (applicationVersion.BuildTime != null || buildTime != null)
                {
                    if (applicationVersion.BuildTime == null || buildTime == null || applicationVersion.BuildTime.Value.ToUniversalTime() != buildTime.Value.ToUniversalTime())
                    {
                        throw new InvalidOperationException("Provided buildTime does not match the buildTime stored for the application fingerprint. If the buildTime has changed a new fingerprint needs to be provided, or provide null and the server will generate a fingerprint for you.");
                    }
                }
            }

            return (Fingerprint)applicationFingerprint;
        }

        public IApplicationVersion RegisterApplicationVersionUsage(IFingerprint applicationVersionFingerprint, Guid applicationId, string version, string supportToolkitNameVersion, DateTime? buildTime, string environment)
        {
            Version tmp;
            if (string.IsNullOrEmpty(version)) throw new ArgumentException("No version provided for application.");
            if (!Version.TryParse(version, out tmp)) throw new ArgumentException("Invalid version format.");

            var applicationVersion = RegisterExistingApplicationVersionUsage((Fingerprint)applicationVersionFingerprint, applicationId, environment);
            if (applicationVersion != null)
                return applicationVersion;

            applicationVersion = new ApplicationVersion((Fingerprint)applicationVersionFingerprint, applicationId, version, new List<IIssueType>(), null, false, false, supportToolkitNameVersion, buildTime, new List<string>(){ environment });
            _repository.AddApplicationVersion(applicationVersion);

            //TODO:
            //_counterBusiness.UpdateApplicationVersionCounters();

            return applicationVersion;
        }

        public IApplicationVersion GetApplicationVersionByIssue(Guid issueId)
        {
            var applicationVersion = _repository.GetApplicationVersionByIssue(issueId);
            applicationVersion = UpdateApplicationVersionsEnvironments(new List<IApplicationVersion>() { applicationVersion }).First();

            return applicationVersion;
        }

        private IApplicationVersion RegisterExistingApplicationVersionUsage(Fingerprint applicationVersionFingerprint, Guid applicationId, string environment)
        {
            var response = _repository.GetApplicationVersion(applicationVersionFingerprint);

            if (response != null)
            {
                if (!response.Environments.Contains(environment))
                {
                    response.Environments.Add(environment);
                    _repository.UpdateApplicationVersion(response);
                }

                if (response.ApplicationId != applicationId)
                    response = _repository.UpdateApplicationVersionId(applicationVersionFingerprint, applicationId);
            }
            return response;
        }

        public IApplicationVersion GetApplicationVersion(string initiativeId, string applicationId, string applicationVersionUniqueIdentifier)
        {
            var applicationVersions = _repository.GetApplicationVersionsForApplications(new List<Guid>(){Guid.Parse(applicationId)});
            applicationVersions = UpdateApplicationVersionsEnvironments(applicationVersions).ToArray();

            var applicationVersionId = string.Empty;

            //Try the name as identifier
            var verionNames = applicationVersions.Where(x => (x.Version ?? Constants.DefaultVersionName) == applicationVersionUniqueIdentifier).ToArray();
            if (verionNames.Count() == 1)
                applicationVersionId = verionNames.Single().Id;

            //Try the id as identifier
            if (string.IsNullOrEmpty(applicationVersionId))
            {
                var versionIds = applicationVersions.Where(x => x.Id.ToString().Replace(":", string.Empty) == applicationVersionUniqueIdentifier).ToArray();
                if (versionIds.Count() == 1)
                    applicationVersionId = versionIds.Single().Id;
            }

            if (string.IsNullOrEmpty(applicationVersionId))
                throw new NullReferenceException("No application version found for the specified uid.");

            var applicationVersion = _repository.GetApplicationVersion(applicationVersionId);
            applicationVersion = UpdateApplicationVersionsEnvironments(new List<IApplicationVersion>() { applicationVersion }).First();

            return applicationVersion;
        }

        public IEnumerable<IApplicationVersion> GetArchivedApplicationVersions(Guid applicationId)
        {
            return _repository.GetArchivedApplicationVersions(applicationId);
        }

        public IApplicationVersion GetApplicationVersion(Fingerprint applicationVersionFingerprint)
        {
            var applicationVersion = _repository.GetApplicationVersion(applicationVersionFingerprint);
            applicationVersion = UpdateApplicationVersionsEnvironments(new List<IApplicationVersion>() { applicationVersion }).First();

            return applicationVersion;
        }

        public void SaveApplicationVersion(IApplicationVersion applicationVersion)
        {
            _repository.UpdateApplicationVersion(applicationVersion);
        }

        public void Remove(Fingerprint applicationVersionFingerprint)
        {
            _repository.DeleteApplicationVersion(applicationVersionFingerprint);
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForDeveloper(string developerEmail)
        {
            var applicationVersions = _repository.GetApplicationVersionsForDeveloper(developerEmail);
            applicationVersions = UpdateApplicationVersionsEnvironments(applicationVersions);

            return applicationVersions;
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForApplications(IEnumerable<Guid> initiativeId)
        {
            var applicationVersions = _repository.GetApplicationVersionsForApplications(initiativeId);
            applicationVersions = UpdateApplicationVersionsEnvironments(applicationVersions);

            return applicationVersions;
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForMachine(string machineId)
        {
            var applicationVersions = _repository.GetApplicationVersionsForMachine(machineId);
            applicationVersions = UpdateApplicationVersionsEnvironments(applicationVersions);
            return applicationVersions;
        }

        private IEnumerable<IApplicationVersion> UpdateApplicationVersionsEnvironments(IEnumerable<IApplicationVersion> applicationVersions)
        {
            var versions = new List<IApplicationVersion>();
            foreach (var applicationVersion in applicationVersions)
            {
                if (applicationVersion == null) continue;
                if (applicationVersion.Environments.IsNullOrEmpty())
                {
                    var sessions = _repository.GetSessionsForApplicationVersion(applicationVersion.Id);
                    var environments = sessions.Select(x => x.Environment ?? "").Distinct().ToList();
                    applicationVersion.Environments = environments;
                    _repository.UpdateApplicationVersion(applicationVersion);
                    versions.Add(applicationVersion);
                }
                else
                {
                    versions.Add(applicationVersion);
                }
            }

            return versions;
        }
    }
}