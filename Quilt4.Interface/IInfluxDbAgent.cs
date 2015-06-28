namespace Quilt4.Interface
{
    public interface IInfluxDbAgent
    {
        bool IsEnabled { get; }
        void WriteAsync(ISerie serie);
        bool CanConnect();
        IInfluxDbSetting GetSetting();
        string GetDatabaseVersion();
    }
}