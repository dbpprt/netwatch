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


using System.Threading.Tasks;
using System.Web.Mvc;
using TrafficStats.ServiceLayer.Contracts;
using TrafficStats.Web.ViewModels.MonitoredServices;

namespace TrafficStats.Web.Controllers
{
    public class MonitoredServicesController : BaseController
    {
        private readonly IDeviceMonitorService _deviceMonitorService;
        private readonly IGroupingService _groupingService;

        public MonitoredServicesController(
            IDeviceMonitorService deviceMonitorService,
            IGroupingService groupingService
            )
        {
            _deviceMonitorService = deviceMonitorService;
            _groupingService = groupingService;
        }

        public async Task<ActionResult> Index()
        {
            var devices = await _deviceMonitorService.GetMonitoredServices();
            return View(new IndexViewModel
            {
                MonitoredServices = devices,
                Groupings = await _groupingService.GetGroupings()
            });
        }

    }
}
