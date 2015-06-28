using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IApplicationVersionBusiness
    {
        IFingerprint AssureApplicationFingerprint(string applicationFingerprint, string version, string supportToolkitNameVersion, DateTime? buildTime, string applicationName, string clientToken);
        IApplicationVersion RegisterApplicationVersionUsage(IFingerprint applicationVersionFingerprint, Guid applicationId, string version, string supportToolkitNameVersion, DateTime? buildTime);
        IEnumerable<IApplicationVersion> GetApplicationVersions(Guid applicationId);
    }
}