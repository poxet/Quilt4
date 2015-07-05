using System;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class ToolkitCompability : IToolkitCompatibilities
    {
        public ToolkitCompability(Version serverVersion, DateTime? registryDate, string supportToolkitNameVersion, Compatibility compatibility, DateTime? lastUsed)
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
        public Compatibility Compatibility { get; private set; }
        public DateTime? LastUsed { get; private set; }
    }
}