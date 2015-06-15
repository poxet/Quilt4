﻿using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface ISettingsBusiness
    {
        T GetConfigSetting<T>(string name);
        ISetting GetDatabaseSetting(string name);
        T GetDatabaseSetting<T>(string name, T defaultValue);
        IEnumerable<ISetting> GetAllDatabaseSettings();
        void SetDatabaseSetting(string id, string value, Type type);
    }
}