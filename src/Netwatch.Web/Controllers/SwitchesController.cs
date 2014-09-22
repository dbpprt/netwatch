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
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using TrafficStats.ServiceLayer.Contracts;
using TrafficStats.Web.Common;
using TrafficStats.Web.ViewModels.Switches;

namespace TrafficStats.Web.Controllers
{
    public class SwitchesController : BaseController
    {
        [Dependency]
        protected IDeviceMonitorService DeviceMonitorService { get; set; }

        [Dependency]
        protected ISnmpScannerService SnmpScannerService { get; set; }

        [Dependency]
        protected ISnmpStatisticsService SnmpStatisticsService { get; set; }

        [Dependency]
        protected IGroupingService GroupingService { get; set; }

        public async Task<ActionResult> Index()
        {
            var viewModel = new IndexViewModel
            {
                Switches = await SnmpStatisticsService.GetSwitchStatistics(),
                Groupings = await GroupingService.GetGroupings()
            };

            return View(viewModel);
        }

        public async Task<ActionResult> Ports(string ipAddress)
        {
            var viewModel = new PortsViewModel
            {
                Switch = await SnmpStatisticsService.GetSnmpTarget(ipAddress),
                Ports = await SnmpStatisticsService.GetPortStatisticsForSwitch(ipAddress)
            };

            if (!Client.IsAdmin())
            {
                viewModel.Ports = viewModel.Ports.Select(stats => stats.ScramblePersonalData()).ToList();    
            }
            
            return View(viewModel);
        }

    }
}
