using System;
using System.Linq;
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
        private readonly IInfluxDbSetting _influxDbSetting;

        public InfluxDbAgent(ISettingsBusiness settingsBusiness)
        {
            _settingsBusiness = settingsBusiness;
            _influxDbSetting = GetSetting();
            if (!string.IsNullOrEmpty(_influxDbSetting.Url))
            {
                _influxDb = new InfluxDb(_influxDbSetting.Url, _influxDbSetting.Username, _influxDbSetting.Password);
            }
        }

        public bool IsEnabled { get { return _influxDb != null; } }

        public void WriteAsync(ISerie serie)
        {
            if (_influxDb == null) return;

            var s = new Serie.Builder(serie.CounterName)
                .Columns(serie.Data.Keys.ToArray())
                .Values(serie.Data.Values.ToArray())
                .Build();

            var task = Task.Run(async () => await _influxDb.WriteAsync(_influxDbSetting.DatabaseName, TimeUnit.Milliseconds, s));
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