using System;
using System.Threading.Tasks;
using InfluxDB.Net;

namespace Quilt4.Web.Agents
{
    public interface IDatabaseConfig
    {
        string Url { get; }
        string Username { get; }
        string Password { get; }
        string Name { get; }
    }

    public class InfluxDbAgent
    {
        private readonly InfluxDb _influxDb;
        private readonly IDatabaseConfig _databaseConfig;

        public InfluxDbAgent(IDatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
            _influxDb = new InfluxDb(_databaseConfig.Url, _databaseConfig.Username, _databaseConfig.Password);
        }

        public async Task<InfluxDbApiResponse> WriteAsync(TimeUnit milliseconds) //, Serie serie)
        {
            //return await _influxDb.WriteAsync(_databaseConfig.Name, milliseconds, serie);
            throw new NotImplementedException();
        }

        public async Task<bool> AuthenticateDatabaseUserAsync()
        {
            var result = await _influxDb.AuthenticateDatabaseUserAsync(_databaseConfig.Name, _databaseConfig.Username, _databaseConfig.Password);
            throw new NotImplementedException();
        }

        public async Task<bool> CanConnect()
        {
            var pong = await _influxDb.PingAsync();
            if (pong.Status == "ok")
                return true;
            return false;
        }

        public async Task<bool> PingAsync()
        {
            var pong = await _influxDb.PingAsync();
            throw new NotImplementedException();
        }

        public async Task<string> VersionAsync()
        {
            return await _influxDb.VersionAsync();
        }
    }
}