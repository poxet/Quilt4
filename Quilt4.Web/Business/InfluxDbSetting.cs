using Quilt4.Interface;

namespace Quilt4.Web.Business
{
    public class InfluxDbSetting : IInfluxDbSetting
    {
        public InfluxDbSetting(string url, string username, string password, string databaseName)
        {
            Url = url;
            Username = username;
            Password = password;
            DatabaseName = databaseName;
        }

        public string Url { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string DatabaseName { get; private set; }
    }
}