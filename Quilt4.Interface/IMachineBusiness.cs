using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IMachineBusiness
    {
        void RegisterMachine(IFingerprint id, string name, IDictionary<string, string> data);
        IMachine GetMachine(string machineFingerprint);
    }
}