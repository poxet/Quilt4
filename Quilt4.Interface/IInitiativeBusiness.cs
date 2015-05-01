namespace Quilt4.Interface
{
    public interface IInitiativeBusiness
    {
        IApplication RegisterApplication(IClientToken clientToken, string applicationName, string applicationVersionFingerprint);
    }
}