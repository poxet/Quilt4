﻿using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IApplicationVersionBusiness
    {
        IFingerprint AssureApplicationFingerprint(string applicationFingerprint, string version, string supportToolkitNameVersion, DateTime? buildTime, string applicationName, string clientToken);
        IApplicationVersion RegisterApplicationVersionUsage(IFingerprint applicationVersionFingerprint, Guid applicationId, string version, string supportToolkitNameVersion, DateTime? buildTime, string environment);
        IEnumerable<IApplicationVersion> GetApplicationVersions(Guid applicationId);
        IApplicationVersion GetApplicationVersion(string initiativeId, string applicationId, string applicationVersionUniqueIdentifier);
        IEnumerable<IApplicationVersion> GetArchivedApplicationVersions(Guid applicationId);
        IApplicationVersion GetApplicationVersionByIssue(Guid issueId);
    }
}