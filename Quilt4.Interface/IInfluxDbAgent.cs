﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quilt4.Interface
{
    public interface IInfluxDbAgent
    {
        bool IsEnabled { get; }
        Task WriteAsync(IEnumerable<ISerie> series);
        Task<List<ISerie>> QueryAsync(string counterName);
        bool CanConnect();
        IInfluxDbSetting GetSetting();
        string GetDatabaseVersion();
    }
}