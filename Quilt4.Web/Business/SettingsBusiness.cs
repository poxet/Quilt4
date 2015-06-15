using Quilt4.Interface;

namespace Quilt4.Web.Business
{
    public class SettingsBusiness : ISettingsBusiness
    {
        public T GetConfigSetting<T>(string name)
        {
            return default(T);
        }

        public T GetDatabaseSetting<T>(string name)
        {
            return default(T);
        }
    }
}