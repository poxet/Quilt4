using Quilt4.Interface;

namespace Quilt4.Web.Business
{
    public class InfluxDbSetting : IInfluxDbSetting
    {
        public InfluxDbSetting(string url, string username, string password, string name)
        {
            Url = url;
            Username = username;
            Password = password;
            Name = name;
        }

        public string Url { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Name { get; private set; }
    }
}