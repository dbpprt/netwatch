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
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Netwatch.DataAccessLayer.Contracts;
using Netwatch.Model;
using Netwatch.Model.DataTransfer;
using Netwatch.Model.Entities;
using Netwatch.ServiceLayer.Common;
using Netwatch.ServiceLayer.Contracts;

namespace Netwatch.ServiceLayer.Services
{
    public class SnmpStatisticsService : ServiceBase<SnmpStatisticsService>, ISnmpStatisticsService
    {
        [Dependency]
        protected IRepository<SnmpTarget> SnmpTargets { get; set; }

        [Dependency]
        protected IRepository<MonitoredPort> MonitoredPorts { get; set; }

        [Dependency]
        protected IRepository<MonitoredService> MonitoredServices { get; set; }

        [Dependency]
        protected IRepository<CollectedTrafficData> CollectedTrafficDatas { get; set; }

        [Dependency]
        protected IRepository<Grouping> Groupings { get; set; }

        [Dependency]
        protected IRepository<MacPortMapping> MacPortMappings { get; set; }

        public async Task<SnmpTarget> GetSnmpTarget(string ipAddress)
        {
            var snmpTarget = await SnmpTargets.Query()
                .FirstOrDefaultAsync(target => target.IpAddress == ipAddress);

            if (snmpTarget == null)
                throw new ArgumentException("ipAddress");

            return snmpTarget;
        }

        public async Task<List<PortStatistics>> GetHighscore(int countOfEntries, DateTime startTime, DateTime endTime,
            TrafficType type)
        {
            try
            {
                var entriesInTimespan = await CollectedTrafficDatas.Query()
                    .Where(data => data.TrafficType == type)
                    .Where(data => data.DiffedTimeSpan != null)
                    .Include(data => data.MonitoredPort)
                    .Where(data => !data.MonitoredPort.ExcludeFromStatistics)
                    .GroupBy(data => data.MonitoredPort)
                    .Select(data => data.Where(inner => inner.TimeScanned >= startTime))
                    .Select(data => data.Where(inner => inner.TimeScanned <= endTime))
                    .Select(data => data.OrderBy(inner => inner.TimeScanned))
                    .Select(data => new
                    {
                        Sum = data.Sum(inner => inner.Octets),
                        Data = data
                    })
                    .OrderByDescending(data => data.Sum)
                    .Take(countOfEntries)
                    .ToListAsync();

                var desiredSampledSeconds = (endTime - startTime).TotalSeconds;
                var results = new List<PortStatistics>();

                var monitoredPorts = await MonitoredPorts.Query()
                    .Include(port => port.SnmpTarget)
                    .ToListAsync();

                foreach (var entry in entriesInTimespan)
                {
                    var sampledSeconds = entry.Data.Sum(entity => entity.DiffedTimeSpan.Value.TotalSeconds);
                    var correctionFactor = (float) desiredSampledSeconds/sampledSeconds;
                    var trafficInSpan = (long) (correctionFactor*entry.Sum);

                    var result = new PortStatistics
                    {
                        MonitoredPort = monitoredPorts.FirstOrDefault(
                            port =>
                                port.SnmpIpAddress == entry.Data.FirstOrDefault().SnmpIpAddress &&
                                port.PortNumber == entry.Data.FirstOrDefault().PortNumber),
                    };
                    result.SnmpTarget = result.MonitoredPort.SnmpTarget;
                    result.Comment = result.MonitoredPort.Comment;

                    var bytesPerSecond = trafficInSpan/desiredSampledSeconds;

                    var ordinals = new[] {"", "K", "M", "G", "T", "P", "E"};

                    var ordinal = 0;

                    while (bytesPerSecond > 1024)
                    {
                        bytesPerSecond /= 1024;
                        ordinal++;
                    }

                    result.EstimatedSpeed = String.Format("{0} {1}b/s",
                        Math.Round(bytesPerSecond, 2, MidpointRounding.AwayFromZero),
                        ordinals[ordinal]);

                    switch (type)
                    {
                        case TrafficType.Inbound:
                            result.InboundTraffic = trafficInSpan;
                            break;

                        case TrafficType.Outbound:
                            result.OutboundTraffic = trafficInSpan;
                            break;
                    }

                    results.Add(result);
                }

                return results;
            }
            catch
            {
                return new List<PortStatistics>();
            }
        }

        public async Task<List<PortStatistics>> GetInboundHighscore(int countOfEntries)
        {
            var monitoredPorts = await MonitoredPorts.Query()
                .Include(port => port.SnmpTarget)
                .Where(port => !port.ExcludeFromStatistics)
                .OrderByDescending(port => port.AllInOctets)
                .Take(countOfEntries)
                .ToListAsync();

            var results = new List<PortStatistics>();

            foreach (var monitoredPort in monitoredPorts)
            {
                var result = new PortStatistics
                {
                    InboundTraffic = monitoredPort.AllInOctets,
                    MonitoredPort = monitoredPort,
                    OutboundTraffic = monitoredPort.AllOutOctets,
                    SnmpTarget = monitoredPort.SnmpTarget
                };

                results.Add(result);
            }

            return results;
        }

        public async Task<List<PortStatistics>> GetOutboundHighscore(int countOfEntries)
        {
            var monitoredPorts = await MonitoredPorts.Query()
                .Include(port => port.SnmpTarget)
                .Where(port => !port.ExcludeFromStatistics)
                .OrderByDescending(port => port.AllOutOctets)
                .Take(countOfEntries)
                .ToListAsync();

            var results = new List<PortStatistics>();

            foreach (var monitoredPort in monitoredPorts)
            {
                var result = new PortStatistics
                {
                    InboundTraffic = monitoredPort.AllInOctets,
                    MonitoredPort = monitoredPort,
                    OutboundTraffic = monitoredPort.AllOutOctets,
                    SnmpTarget = monitoredPort.SnmpTarget
                };

                results.Add(result);
            }

            return results;
        }

        public async Task<List<PortStatistics>> GetPortStatisticsForSwitch(string ipAddress)
        {
            var snmpTarget = await SnmpTargets.Query()
                .FirstOrDefaultAsync(target => target.IpAddress == ipAddress);

            if (snmpTarget == null)
                throw new ArgumentException("ipAddress");

            var monitoredPorts = await MonitoredPorts.Query()
                .Where(port => port.SnmpIpAddress == ipAddress)
                .Where(port => !port.ExcludeFromStatistics)
                .ToListAsync();

            var results = new List<PortStatistics>();

            foreach (var monitoredPort in monitoredPorts)
            {
                var result = new PortStatistics
                {
                    InboundTraffic = monitoredPort.AllInOctets,
                    MonitoredPort = monitoredPort,
                    OutboundTraffic = monitoredPort.AllOutOctets,
                    SnmpTarget = snmpTarget
                };

                results.Add(result);
            }

            return results;
        }

        public async Task<List<MacPortMapping>> GetMacDetails(string macAddress)
        {
            return await MacPortMappings.Query()
                .Include(_ => _.MacAddress)
                .Include(_ => _.MonitoredPort)
                .Include(_ => _.MonitoredPort.SnmpTarget)
                .Where(_ => _.Mac == macAddress)
                .ToListAsync();
        }

        public async Task<List<SwitchStatistics>> GetSwitchStatistics()
        {
            var snmpTargets = await SnmpTargets.Query()
                .Include(_ => _.Group)
                .ToListAsync();

            var results = new List<SwitchStatistics>();

            foreach (var snmpTarget in snmpTargets)
            {
                var target = snmpTarget;

                try
                {
                    var inboundTraffic = await MonitoredPorts.Query()
                        .Where(port => port.SnmpIpAddress == target.IpAddress)
                        .Where(port => !port.ExcludeFromStatistics)
                        .SumAsync(port => port.AllInOctets);

                    var outboundTraffic = await MonitoredPorts.Query()
                        .Where(port => port.SnmpIpAddress == target.IpAddress)
                        .Where(port => !port.ExcludeFromStatistics)
                        .SumAsync(port => port.AllOutOctets);

                    results.Add(new SwitchStatistics
                    {
                        InboundTraffic = inboundTraffic,
                        OutboundTraffic = outboundTraffic,
                        SnmpTarget = target,
                        Reachable = target.Reachable,
                        GroupName = target.Group.Name
                    });
                }
                catch (Exception)
                {
                    // TODO: Logging
                }
            }

            return results;
        }
    }
}