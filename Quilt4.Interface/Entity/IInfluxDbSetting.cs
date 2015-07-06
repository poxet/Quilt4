namespace Quilt4.Interface
{
    public interface IInfluxDbSetting
    {
        string Url { get; }
        string Username { get; }
        string Password { get; }
        string DatabaseName { get; }
    }
}