using System;
using System.Collections.Generic;
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

        public async Task WriteAsync(IEnumerable<ISerie> series)
        {
            if (_influxDb == null) return;

            var ss = series.Select(x => new Serie.Builder(x.CounterName)
                .Columns(x.Data.Keys.ToArray())
                .Values(x.Data.Values.ToArray())
                .Build());

            await _influxDb.WriteAsync(_influxDbSetting.DatabaseName, TimeUnit.Seconds, ss.ToArray());
        }

        public async Task ClearAsync(string counterName)
        {
            //TODO:
            //await _influxDb.QueryAsync(_influxDbSetting.DatabaseName, "DELETE FROM \"" + counterName + "\"", TimeUnit.Seconds);
            //await _influxDb.DeleteSeriesAsync(_influxDbSetting.DatabaseName, counterName);
        }

        public async Task<ISerie> QueryLastAsync(string counterName)
        {
            if (_influxDb == null) throw new NullReferenceException("No influxDb agent instance.");
                
            var response = await _influxDb.QueryAsync(_influxDbSetting.DatabaseName, "SELECT * FROM \"" + counterName + "\" LIMIT 1", TimeUnit.Seconds);
            foreach (var serie in response)
            {
                var data = new Dictionary<string, object>();
                for (var i = 0; i < serie.Columns.Length; i++)
                {
                    data.Add(serie.Columns[i], serie.Points[0][i]);
                }
                return new BusinessEntities.Serie(counterName, data);
            }
            return null;
        }

        public async Task<List<ISerie>> QueryAsync(string counterName)
        {
            var list = new List<ISerie>();
            var response = await _influxDb.QueryAsync(_influxDbSetting.DatabaseName, "select * from " + counterName + "", TimeUnit.Seconds);
            foreach (var serie in response)
            {
                var data = new Dictionary<string, object>();
                for (var i = 0; i < serie.Columns.Length; i++)
                {
                    data.Add(serie.Columns[i], serie.Points[i]);
                }
                list.Add(new BusinessEntities.Serie(counterName, data));
            }
            return list;
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