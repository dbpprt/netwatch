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
using TrafficStats.Model;
using TrafficStats.ServiceLayer.Contracts;
using TrafficStats.Web.ViewModels.Dashboard;

namespace TrafficStats.Web.Controllers
{
    public class DashboardController : BaseController
    {
        [Dependency]
        protected IDeviceMonitorService DeviceMonitorService { get; set; }

        [Dependency]
        protected IReportService ReportService { get; set; }

        [Dependency]
        protected ISnmpStatisticsService SnmpStatisticsService { get; set; }

        public async Task<ActionResult> Index()
        {
            var viewModel = new IndexViewModel
            {
                MonitoredServices = await DeviceMonitorService.GetMonitoredServices(),
                OverallTodayStats = await ReportService.GetOverallDayStatistic(DateTime.Now),
                OverallYesterdayStats = await ReportService.GetOverallDayStatistic(DateTime.Now.Subtract(new TimeSpan(24, 0, 0)))
            };

            var inbound =
                await
                    SnmpStatisticsService.GetHighscore(50, DateTime.Now.Subtract(new TimeSpan(0, 5, 0)), DateTime.Now,
                        TrafficType.Inbound);
            var outbound =
                await
                    SnmpStatisticsService.GetHighscore(5, DateTime.Now.Subtract(new TimeSpan(0, 5, 0)), DateTime.Now,
                        TrafficType.Outbound);

            return View(viewModel);
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}
