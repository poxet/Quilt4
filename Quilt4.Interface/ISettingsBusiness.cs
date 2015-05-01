namespace Quilt4.Interface
{
    public interface ISettingsBusiness
    {
        T GetSetting<T>(string name);
    }
}