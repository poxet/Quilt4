using System;
using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.Web.Business
{
    public class SettingsBusiness : ISettingsBusiness
    {
        private readonly IRepository _repository;

        public SettingsBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public T GetConfigSetting<T>(string name)
        {
            return default(T);
        }

        public ISetting GetDatabaseSetting(string name)
        {
            var result = _repository.GetSetting(name);
            return result;
        }

        public T GetDatabaseSetting<T>(string name, T defaultValue)
        {
            var result = _repository.GetSetting(name, defaultValue);
            return result;
        }

        public IEnumerable<ISetting> GetAllDatabaseSettings()
        {
            var result = _repository.GetSettings();
            return result;
        }

        public void SetDatabaseSetting(string id, string value, Type type)
        {
            _repository.SetSetting(id, value, type);
        }
    }
}