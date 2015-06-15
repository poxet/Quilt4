namespace Quilt4.Interface
{
    public interface ISettingsBusiness
    {
        T GetConfigSetting<T>(string name);
        T GetDatabaseSetting<T>(string name);
    }
}