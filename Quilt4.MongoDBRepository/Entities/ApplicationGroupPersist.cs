using System;
using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.MongoDBRepository.Entities
{
    public class ToolkitCompability : IToolkitCompatibilities
    {
        public ToolkitCompability(Version serverVersion, DateTime? registryDate, string supportToolkitNameVersion, ECompatibility compatibility, DateTime? lastUsed)
        {
            LastUsed = lastUsed;
            Compatibility = compatibility;
            SupportToolkitNameVersion = supportToolkitNameVersion;
            RegisterDate = registryDate;
            ServerVersion = serverVersion;
        }

        public Version ServerVersion { get; private set; }
        public DateTime? RegisterDate { get; private set; }
        public string SupportToolkitNameVersion { get; private set; }
        public ECompatibility Compatibility { get; private set; }
        public DateTime? LastUsed { get; set; }
    }

    internal class ApplicationGroupPersist
    {
        public string Name { get; internal set; }
        public IEnumerable<ApplicationPersist> Applications { get; internal set; }
    }
}