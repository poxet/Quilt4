using System;
using System.Collections.Generic;
using System.Linq;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Tharga.Quilt4Net;

namespace Quilt4.Web.Business
{
    public class ApplicationVersionBusiness : IApplicationVersionBusiness
    {
        private readonly IRepository _repository;

        public ApplicationVersionBusiness(IRepository repository)
        {
            _repository = repository;
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

        public IApplicationVersion RegisterApplicationVersion(IFingerprint applicationVersionFingerprint, Guid applicationId, string version, string supportToolkitNameVersion, DateTime? buildTime)
        {
            Version tmp;
            if (string.IsNullOrEmpty(version)) throw new ArgumentException(String.Format("No version provided for application."));
            if (!Version.TryParse(version, out tmp)) throw new ArgumentException(String.Format("Invalid version format."));

            var applicationVersion = RegisterApplicationVersionEx((Fingerprint)applicationVersionFingerprint, applicationId);
            if (applicationVersion != null)
                return applicationVersion;

            try
            {
                applicationVersion = new ApplicationVersion((Fingerprint)applicationVersionFingerprint, applicationId, version, new List<IIssueType>(), null, false, false, supportToolkitNameVersion, buildTime);
                _repository.AddApplicationVersion(applicationVersion);
            }
            catch (System.Data.SqlClient.SqlException)
            {
                applicationVersion = RegisterApplicationVersionEx((Fingerprint)applicationVersionFingerprint, applicationId);
                if (applicationVersion == null)
                    throw;
            }
            return applicationVersion;
        }

        private IApplicationVersion RegisterApplicationVersionEx(Fingerprint applicationVersionFingerprint, Guid applicationId)
        {
            var response = _repository.GetApplicationVersion(applicationVersionFingerprint);
            if (response != null)
            {
                if (response.ApplicationId != applicationId)
                    response = _repository.UpdateApplicationVersionId(applicationVersionFingerprint, applicationId);
            }
            return response;
        }


        public IApplicationVersion GetApplicationVersion(string initiativeId, string applicationId, string applicationVersionUniqueIdentifier)
        {
            //var initiative = _repository.GetInitiative(Guid.Parse(initiativeId));
            //var application = initiative.ApplicationGroups.SelectMany(x => x.Applications).Single(x => x.Id == Guid.Parse(applicationId));

            //var applicationIds = applications.Select(x => x.Id);
            //var applicationIds = new List<Guid>();
            //foreach (var application in applications)
            //{
            //    applicationIds.Add(application.Id);
            //}

            var applicationVersions = _repository.GetApplicationVersionsForApplications(new List<Guid>(){Guid.Parse(applicationId)}).ToArray();
            

            var applicationVersionId = "";

            //Try the name as identifier
            var verionNames = applicationVersions.Where(x => (x.Version ?? Models.Constants.DefaultVersionName) == applicationVersionUniqueIdentifier).ToArray();
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

            return applicationVersion;
        }

        public IEnumerable<IApplicationVersion> GetArchivedApplicationVersions(Guid applicationId)
        {
            return _repository.GetArchivedApplicationVersions(applicationId);
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

        //public IEnumerable<IApplicationVersion> GetApplicationVersionsForApplications(IEnumerable<Guid> initiative)
        //{
        //    return _repository.GetApplicationVersionsForApplications(initiative);
        //}
    }
}