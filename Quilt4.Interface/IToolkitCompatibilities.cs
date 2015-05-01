using System;

namespace Quilt4.Interface
{
    public interface IToolkitCompatibilities
    {
        Version ServerVersion { get; }
        DateTime? RegisterDate { get; }
        string SupportToolkitNameVersion { get; }
        ECompatibility Compatibility { get; }
        DateTime? LastUsed { get; set; }
    }
}