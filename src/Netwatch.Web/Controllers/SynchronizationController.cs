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
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using TrafficStats.ServiceLayer.Contracts;

namespace TrafficStats.Web.Controllers
{
    public class SynchronizationController : Controller
    {
        [Dependency]
        protected IDeviceMonitorService DeviceMonitorService { get; set; }

        [Dependency]
        protected ISnmpScannerService SnmpScannerService { get; set; }

        [Dependency]
        protected IReportService ReportService { get; set; }

        public async Task<ActionResult> Reports()
        {
            var pendingTimes = await ReportService.GetPendingReportTimes();

            foreach (var pendingTime in pendingTimes)
            {
                await ReportService.WritePortReports(pendingTime.Year, pendingTime.Month, pendingTime.Day, pendingTime.Hour);
            }
            
            return Json(new { Status = "Done" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> MacAddresses()
        {
            await SnmpScannerService.ExecuteMacScan();
            

            return Json(new {Status = "Done"}, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Monitoring()
        {
            await DeviceMonitorService.ExecuteDeviceMonitoring();

            return Json(new { Status = "Done" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Traffic()
        {
            await SnmpScannerService.ExecuteTrafficScan();

            return Json(new { Status = "Done" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> All()
        {
            await SnmpScannerService.ExecuteTrafficScan();
            await DeviceMonitorService.ExecuteDeviceMonitoring();
            await SnmpScannerService.ExecuteMacScan();

            return Json(new { Status = "Done" }, JsonRequestBehavior.AllowGet);
        }
	}
}
