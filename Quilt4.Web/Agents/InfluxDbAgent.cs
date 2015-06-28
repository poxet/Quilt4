using System;
using System.Threading.Tasks;
using InfluxDB.Net;
using InfluxDB.Net.Models;
using Quilt4.Interface;

namespace Quilt4.Web.Agents
{
    public class InfluxDbAgent : IInfluxDbAgent
    {
        private readonly ISettingsBusiness _settingsBusiness;
        private readonly InfluxDb _influxDb;

        public InfluxDbAgent(ISettingsBusiness settingsBusiness)
        {
            _settingsBusiness = settingsBusiness;
            var influxDbSetting = GetSetting();
            if (!string.IsNullOrEmpty(influxDbSetting.Url))
            {
                _influxDb = new InfluxDb(influxDbSetting.Url, influxDbSetting.Username, influxDbSetting.Password);
            }
        }

        public bool IsEnabled { get { return _influxDb != null; } }

        public void WriteAsync(ISerie serie)
        {
            if (_influxDb == null) return;

            //var b = new Serie.Builder("");
            //var s = b.Columns("").Values("").Build();

            throw new NotImplementedException();
            //var task = Task.Run(async () => await _influxDb.WriteAsync(_influxDbSetting.Name, TimeUnit.Milliseconds, s));
        }

        public bool CanConnect()
        {
            var task = Task.Run(async () => await _influxDb.PingAsync());
            return task.Result.Status == "ok";
        }

        public IInfluxDbSetting GetSetting()
        {
            var result = _settingsBusiness.GetInfluxDBSetting();
            return result;
        }

        public string GetDatabaseVersion()
        {
            try
            {
                var task = Task.Run(async () => await _influxDb.VersionAsync());
                return task.Result;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        //public async Task<bool> AuthenticateDatabaseUserAsync()
        //{
        //    var result = await _influxDb.AuthenticateDatabaseUserAsync(_influxDbSetting.Name, _influxDbSetting.Username, _influxDbSetting.Password);
        //    throw new NotImplementedException();
        //}

        //public async Task<bool> CanConnect()
        //{
        //    var pong = await _influxDb.PingAsync();
        //    if (pong.Status == "ok")
        //        return true;
        //    return false;
        //}

        //public async Task<bool> PingAsync()
        //{
        //    var pong = await _influxDb.PingAsync();
        //    throw new NotImplementedException();
        //}

        //public async Task<string> VersionAsync()
        //{
        //    return await _influxDb.VersionAsync();
        //}
    }
}