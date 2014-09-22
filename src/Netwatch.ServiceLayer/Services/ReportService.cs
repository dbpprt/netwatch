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
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using SnmpSharpNet;
using TrafficStats.DataAccessLayer.Contracts;
using TrafficStats.Model;
using TrafficStats.Model.Entities;
using TrafficStats.ServiceLayer.Common;
using TrafficStats.ServiceLayer.Contracts;

namespace TrafficStats.ServiceLayer.Services
{
    public class ReportService : ServiceBase<ReportService>, IReportService
    {
        [Dependency]
        protected ISnmpStatisticsService SnmpStatisticsService { get; set; }

        [Dependency]
        protected IRepository<MonitoredPort> MonitoredPorts { get; set; }
 
        [Dependency]
        protected IRepository<PortReport> PortReports { get; set; } 

        [Dependency]
        protected IRepository<CollectedTrafficData> CollectedTrafficDatas { get; set; }

        [Dependency]
        protected IRepository<MacPortMapping> MacPortMappings { get; set; } 

        public async Task<List<PortReport>> GetOverallDayStatistic(DateTime day)
        {
            var reports = await PortReports.Query()
                .Where(report => report.Year == day.Year)
                .Where(report => report.Month == day.Month)
                .Where(report => report.Day == day.Day)
                .GroupBy(report => report.Hour)
                .Select(report => new
                {
                    Hour = report.Key,
                    Inbound = report.Sum(inner => inner.InboundTraffic),
                    Outbound = report.Sum(inner => inner.OutboundTraffic)
                })
                .ToListAsync();

            var hourRange = Enumerable.Range(0, 24);
            var result = new List<PortReport>();

            foreach (var hour in hourRange)
            {
                var existing = reports.FirstOrDefault(report => report.Hour == hour);

                if (existing != null)
                    result.Add(new PortReport
                    {
                        Day = day.Day,
                        Year = day.Year,
                        Hour = existing.Hour,
                        Month = day.Month,
                        InboundTraffic = existing.Inbound,
                        OutboundTraffic = existing.Outbound
                    });
                else
                    result.Add(new PortReport
                    {
                        Day = day.Day,
                        Year = day.Year,
                        Hour = hour,
                        Month = day.Month
                    });
            }

            return result;
        }

        public Task<List<MacPortMapping>> GetMacAddressesForPort(string snmpIpAddress, int portNumber)
        {
            var mappings = MacPortMappings.Query()
                .Where(mapping => mapping.PortNumber == portNumber && mapping.SnmpIpAddress == snmpIpAddress)
                .OrderBy(mapping => mapping.LastSeen)
                .Include(mapping => mapping.MacAddress)
                .Include(mapping => mapping.MacAddress.User)
                .ToListAsync();

            return mappings;
        }

        public async Task<List<PortReport>> GetDayStatisticForPort(string snmpIpAddress, int portNumber, DateTime day)
        {
            var reports = await PortReports.Query()
                .Where(report => report.SnmpIpAddress == snmpIpAddress)
                .Where(report => report.PortNumber == portNumber)
                .Where(report => report.Year == day.Year)
                .Where(report => report.Month == day.Month)
                .Where(report => report.Day == day.Day)
                .ToListAsync();

            var hourRange = Enumerable.Range(0, 24);
            var result = new List<PortReport>();

            foreach (var hour in hourRange)
            {
                var existing = reports.FirstOrDefault(report => report.Hour == hour);

                if (existing != null)
                    result.Add(existing);
                else
                    result.Add(new PortReport
                    {
                        Day = day.Day,
                        Year = day.Year,
                        PortNumber = portNumber,
                        SnmpIpAddress = snmpIpAddress,
                        Hour = hour,
                        Month = day.Month
                    });
            }

            return result;
        }

        public async Task<List<DateTime>>  GetPendingReportTimes()
        {
            var times = await CollectedTrafficDatas.Query()
                .Select(data => new
                {
                    Hour = data.TimeScanned.Hour,
                    Day = data.TimeScanned.Day,
                    Month = data.TimeScanned.Month,
                    Year = data.TimeScanned.Year
                })
                .Distinct()
                .ToListAsync();

            return times
                .Select(time => new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0))
                .Where(time => DateTime.Now.Subtract(time) > new TimeSpan(2, 0, 0))
                .ToList();
        }

        public async Task WritePortReports(int year, int month, int day, int hour)
        {
            var startTime = new DateTime(year, month, day, hour, 0, 0);
            var endTime = startTime.Add(new TimeSpan(1, 0, 0));

            var inboundHighscore = await SnmpStatisticsService
                .GetHighscore(int.MaxValue, startTime, endTime, TrafficType.Inbound);
            var outboundHighscore = await SnmpStatisticsService
                .GetHighscore(int.MaxValue, startTime, endTime, TrafficType.Outbound);

            if (inboundHighscore == null || outboundHighscore == null)
                throw new Exception("high score failure :(");

            var reports = new List<PortReport>();
            var monitoredPorts = await MonitoredPorts.Query().ToListAsync();

            foreach (var monitoredPort in monitoredPorts)
            {
                var inboundStatistics = inboundHighscore
                    .FirstOrDefault(
                        stats =>
                            stats.MonitoredPort.SnmpIpAddress == monitoredPort.SnmpIpAddress &&
                            stats.MonitoredPort.PortNumber == monitoredPort.PortNumber);
                var outboundStatistics = outboundHighscore
                    .FirstOrDefault(
                        stats =>
                            stats.MonitoredPort.SnmpIpAddress == monitoredPort.SnmpIpAddress &&
                            stats.MonitoredPort.PortNumber == monitoredPort.PortNumber);

                if (inboundStatistics == null || outboundStatistics == null)
                    continue;

                var currentReport = new PortReport
                {
                    Day = day,
                    Hour = hour,
                    Month = month,
                    Year = year,
                    InboundTraffic = inboundStatistics.InboundTraffic,
                    OutboundTraffic = outboundStatistics.OutboundTraffic,
                    PortNumber = monitoredPort.PortNumber,
                    SnmpIpAddress = monitoredPort.SnmpIpAddress
                };

                reports.Add(currentReport);
            }

            PortReports.InsertRange(reports);
            await Context.SaveAsync();

            var itemsToDelete = await CollectedTrafficDatas.Query()
                .Where(data => data.TimeScanned >= startTime)
                .Where(data => data.TimeScanned <= endTime)
                .ToListAsync();

            itemsToDelete.ForEach(item => CollectedTrafficDatas.Delete(item));
            await Context.SaveAsync();
        }
    }
}
