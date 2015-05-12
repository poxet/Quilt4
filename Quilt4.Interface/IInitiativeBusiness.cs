namespace Quilt4.Interface
{
    public interface IInitiativeBusiness
    {
        void Create(string developerName, string initiativename);
        IApplication RegisterApplication(IClientToken clientToken, string applicationName, string applicationVersionFingerprint);
    }
}