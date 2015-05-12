using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IInitiativeBusiness
    {
        void Create(string developerName, string initiativename);
        IEnumerable<IInitiative> GetInitiativesByDeveloperHead(string developerName);
        IApplication RegisterApplication(IClientToken clientToken, string applicationName, string applicationVersionFingerprint);
        IEnumerable<IInitiative> GetInitiatives();
    }
}