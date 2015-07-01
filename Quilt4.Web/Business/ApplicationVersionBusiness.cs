using System;
using System.Collections.Generic;
using System.Linq;
using Quilt4.BusinessEntities;
using Quilt4.Interface;

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

        public IApplicationVersion RegisterApplicationVersionUsage(IFingerprint applicationVersionFingerprint, Guid applicationId, string version, string supportToolkitNameVersion, DateTime? buildTime)
        {
            Version tmp;
            if (string.IsNullOrEmpty(version)) throw new ArgumentException("No version provided for application.");
            if (!Version.TryParse(version, out tmp)) throw new ArgumentException("Invalid version format.");

            var applicationVersion = RegisterExistingApplicationVersionUsage((Fingerprint)applicationVersionFingerprint, applicationId);
            if (applicationVersion != null)
                return applicationVersion;

            applicationVersion = new ApplicationVersion((Fingerprint)applicationVersionFingerprint, applicationId, version, new List<IIssueType>(), null, false, false, supportToolkitNameVersion, buildTime);
            _repository.AddApplicationVersion(applicationVersion);

            _counterBusiness.RegisterApplicationVersion(applicationVersion);

            return applicationVersion;
        }

        private IApplicationVersion RegisterExistingApplicationVersionUsage(Fingerprint applicationVersionFingerprint, Guid applicationId)
        {
            var response = _repository.GetApplicationVersion(applicationVersionFingerprint);
            if (response != null)
            {
                if (response.ApplicationId != applicationId)
                    response = _repository.UpdateApplicationVersionId(applicationVersionFingerprint, applicationId);
            }
            return response;
        }

        public IApplicationVersion GetApplicationVersion(Fingerprint applicationVersionFingerprint)
        {
            return _repository.GetApplicationVersion(applicationVersionFingerprint);
        }

        public void SaveApplicationVersion(IApplicationVersion applicationVersion)
        {
            _repository.UpdateApplicationVersion(applicationVersion);
        }

        public void Remove(Fingerprint applicationVersionFingerprint)
        {
            _repository.DeleteApplicationVersion(applicationVersionFingerprint);
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForDeveloper(string developerName)
        {
            return _repository.GetApplicationVersionsForDeveloper(developerName);
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForApplications(IEnumerable<Guid> initiativeId)
        {
            return _repository.GetApplicationVersionsForApplications(initiativeId);
        }

        public IEnumerable<IApplicationVersion> GetApplicationVersionsForMachine(string machineId)
        {
            return _repository.GetApplicationVersionsForMachine(machineId);
        }
    }
}