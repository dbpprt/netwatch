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
using TrafficStats.ServiceLayer.Services;
using TrafficStats.Web.ViewModels.PortDetails;

namespace TrafficStats.Web.Controllers
{
    public class PortDetailsController : BaseController
    {
        [Dependency]
        protected IReportService ReportService { get; set; }

        [Dependency]
        protected ISnmpScannerService SnmpScannerService { get; set; }

        public async Task<ActionResult> Index(string snmpIpAddress, int portNumber)
        {

            var viewModel = new IndexViewModel
            {
                TodayReports = await ReportService.GetDayStatisticForPort(snmpIpAddress, portNumber, DateTime.Now),
                YesterdayReports =
                    await
                        ReportService.GetDayStatisticForPort(snmpIpAddress, portNumber,
                            DateTime.Now.Subtract(new TimeSpan(24, 0, 0))),
                MacAddresses = await ReportService.GetMacAddressesForPort(snmpIpAddress, portNumber)
            };


            return View(viewModel);
        }

	}
}
