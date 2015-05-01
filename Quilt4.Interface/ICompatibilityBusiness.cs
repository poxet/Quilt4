using System;

namespace Quilt4.Interface
{
    public interface ICompatibilityBusiness
    {
        void RegisterToolkitCompability(Version version, DateTime utcNow, string supportToolkitNameVersion, object o);
    }
}