using System;
using System.Collections.Generic;
using System.Linq;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class ApplicationVersion : IApplicationVersion
    {
        private readonly Fingerprint _id;
        private readonly Guid _applicationId;
        private readonly string _version;
        private readonly List<IIssueType> _issueTypes;
        private readonly DateTime? _buildTime;
        private readonly string _supportToolkitNameVersion;
        private string _responseMesssage;
        private bool _isOfficial;
        private bool _ignore;
        private List<string> _environments;

        public ApplicationVersion(Fingerprint id, Guid applicationId, string version, IEnumerable<IIssueType> issueTypes, string responseMesssage, bool isOfficial, bool ignore, string supportToolkitNameVersion, DateTime? buildTime, List<string> environments )
        {
            _id = id;
            _applicationId = applicationId;
            _version = version;
            _issueTypes = issueTypes.ToList();
            _responseMesssage = responseMesssage;
            _isOfficial = isOfficial;
            _ignore = ignore;
            _supportToolkitNameVersion = supportToolkitNameVersion;
            _buildTime = buildTime;
            _environments = environments;
        }

        public string Id { get { return _id; } }
        public Guid ApplicationId { get { return _applicationId; } }
        public string Version { get { return _version; } }
        public IEnumerable<IIssueType> IssueTypes { get { return _issueTypes; } }
        public string ResponseMessage { get { return _responseMesssage; } set { _responseMesssage = value == string.Empty ? null : value; } }
        public bool IsOfficial { get { return _isOfficial; } set { _isOfficial = value; } }

        public bool Ignore
        {
            get { return _ignore; }
            set { _ignore = value; }
        }

        public string SupportToolkitNameVersion { get { return _supportToolkitNameVersion; } }
        public DateTime? BuildTime { get { return _buildTime; } }
        public List<string> Environments { get { return _environments;} set { _environments = value; }}

        public void Add(IIssueType issueType)
        {
            _issueTypes.Add(issueType);
        }
    }
}