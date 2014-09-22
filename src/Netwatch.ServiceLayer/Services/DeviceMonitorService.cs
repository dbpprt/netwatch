#region Copyright (C) 2014 Netwatch
// Copyright (C) 2014 Netwatch
// https://github.com/flumbee/netwatch

// This file is part of Netwatch

// Applified.NET is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.

// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
#endregion


using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using TrafficStats.DataAccessLayer.Contracts;
using TrafficStats.Model.Entities;
using TrafficStats.ServiceLayer.Common;
using TrafficStats.ServiceLayer.Contracts;

namespace TrafficStats.ServiceLayer.Services
{
    public class DeviceMonitorService : ServiceBase<DeviceMonitorService>, IDeviceMonitorService
    {
        [Dependency]
        protected IRepository<MonitoredService> MonitoredServices { get; set; }

        public async Task ExecuteDeviceMonitoring()
        {
            var targets = await MonitoredServices.Query().ToListAsync();

            foreach (var target in targets)
            {
                target.LastTimeScanned = DateTime.Now;

                using (var ping = new Ping())
                {
                    var pingResult = ping.Send(target.Address);

                    if (pingResult != null && pingResult.Status == IPStatus.Success)
                    {
                        if (!target.IsUpAndRunning)
                        {
                            target.IsUpAndRunning = true;
                            target.LastStatusChange = DateTime.Now;
                        }
                    }
                    else
                    {
                        if (target.IsUpAndRunning)
                        {
                            target.IsUpAndRunning = false;
                            target.LastStatusChange = DateTime.Now;
                        }
                    }
                }

                MonitoredServices.Update(target);
            }

            await Context.SaveAsync();
        }

        public async Task<MonitoredService> AddMonitoredService(MonitoredService service)
        {
            MonitoredServices.Insert(service);
            await Context.SaveAsync();
            return service;
        }

        public async Task RemoveMonitoredService(Guid id)
        {
            var target = await MonitoredServices.Query()
                .FirstOrDefaultAsync(service => service.Id == id);

            if (target != null)
            {
                MonitoredServices.Delete(target);
                await Context.SaveAsync();
            }
        }

        public async Task<MonitoredService> UpdateMonitoredService(MonitoredService service)
        {
            MonitoredServices.Update(service);
            await Context.SaveAsync();
            return service;
        }

        public async Task<List<MonitoredService>> GetMonitoredServices()
        {
            return await MonitoredServices.Query()
                .OrderBy(_ => _.SortKey)
                .ThenBy(_ => _.Name)
                .Include(_ => _.Group)
                .ToListAsync();
        }

        public async Task<MonitoredService> GetMonitoredService(Guid id)
        {
            var target = await MonitoredServices.Query()
                .Include(_ => _.Group)
                .FirstOrDefaultAsync(service => service.Id == id);

            return target;
        }
    }
}
