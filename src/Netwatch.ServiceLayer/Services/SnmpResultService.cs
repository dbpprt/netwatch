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
    public class SnmpResultService : ServiceBase<SnmpResultService>, ISnmpResultService
    {
        [Dependency]
        protected IRepository<SnmpTarget> SnmpTargets { get; set; }

        [Dependency]
        protected IRepository<MonitoredPort> MonitoredPorts { get; set; }

        [Dependency]
        protected IRepository<CollectedTrafficData> CollectedTrafficDatas { get; set; }

        [Dependency]
        protected IRepository<Grouping> Groupings { get; set; }

        [Dependency]
        protected IRepository<MacAddress> MacAddresses { get; set; }

        [Dependency]
        protected IRepository<MacPortMapping> MacPortMappings { get; set; }

        public async Task ProcessTrafficResults(List<SnmpResult<int, long>> results)
        {
            foreach (var snmpResult in results)
            {
                var monitoredPort = await EnsureMonitoredPort(snmpResult.SnmpTarget, snmpResult.Identifier);

                TrafficType type;

                if (snmpResult.Oid.StartsWith(SnmpOids.IfHcInOctets))
                    type = TrafficType.Outbound;
                else if (snmpResult.Oid.StartsWith(SnmpOids.IfHcOutOctets))
                    type = TrafficType.Inbound;
                else
                {
                    continue;
                }

                //switch (snmpResult.Oid)
                //{
                //    case SnmpOids.IfHcInOctets:
                //        type = TrafficType.Inbound;
                //        break;
                //    case SnmpOids.IfHcOutOctets:
                //        type = TrafficType.Outbound;
                //        break;
                //    default:
                //        continue;
                //}

                await ProcessTrafficResult(monitoredPort, snmpResult.Result, type);
                await Context.SaveAsync();
            }
        }

        public async Task ProcessMacScanResults(List<SnmpResult<int, string>> results)
        {
            foreach (var snmpResult in results)
            {
                var monitoredPort = await EnsureMonitoredPort(snmpResult.SnmpTarget, snmpResult.Identifier);

                if (monitoredPort.SkipMacAddressScan)
                    continue;

                var existing = await MacAddresses.Query().FirstOrDefaultAsync(mac => mac.Mac == snmpResult.Result);

                if (existing == null)
                {
                    existing = new MacAddress
                    {
                        Mac = snmpResult.Result
                    };

                    MacAddresses.Insert(existing);
                }

                var mapping = await MacPortMappings.Query()
                    .FirstOrDefaultAsync(map => map.Mac == snmpResult.Result && map.PortNumber == snmpResult.Identifier);

                if (mapping == null)
                {
                    mapping = new MacPortMapping
                    {
                        Mac = snmpResult.Result,
                        PortNumber = snmpResult.Identifier,
                        SnmpIpAddress = snmpResult.SnmpTarget.IpAddress,
                        LastSeen = DateTime.Now
                    };

                    MacPortMappings.Insert(mapping);
                }
                else
                {
                    mapping.LastSeen = DateTime.Now;
                    MacPortMappings.Update(mapping);
                }
            }

            await Context.SaveAsync();
        }

        private async Task<MonitoredPort> EnsureMonitoredPort(SnmpTarget target, int portNumber)
        {
            var existingPort = await MonitoredPorts.Query()
                .FirstOrDefaultAsync(port => port.PortNumber == portNumber && port.SnmpIpAddress == target.IpAddress);

            if (existingPort == null)
            {
                var newPort = new MonitoredPort
                {
                    AllInOctets = 0,
                    AllOutOctets = 0,
                    SnmpIpAddress = target.IpAddress,
                    PortNumber = portNumber,
                    FirstTimeScanned = DateTime.Now,
                    LastInOctets = 0,
                    LastOutOctets = 0,
                    ExcludeFromStatistics = false,
                    Comment = null
                };

                newPort = MonitoredPorts.Insert(newPort);

                await Context.SaveAsync();

                return newPort;
            }

            return existingPort;
        }

        private async Task WriteTrafficData(MonitoredPort port, long octets, TrafficType trafficType)
        {
            var entity = new CollectedTrafficData
            {
                TimeScanned = DateTime.Now,
                Octets = octets,
                TrafficType = trafficType,
                PortNumber = port.PortNumber,
                SnmpIpAddress = port.SnmpIpAddress
            };

            var existing = await CollectedTrafficDatas.Query()
                .Where(data => data.TrafficType == trafficType)
                .Include(data => data.MonitoredPort)
                .Where(data => data.MonitoredPort.SnmpIpAddress == port.SnmpIpAddress)
                .Where(data => data.MonitoredPort.PortNumber == port.PortNumber)
                .OrderByDescending(data => data.TimeScanned)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                entity.DiffedTimeSpan = null;
                entity.Octets = octets;
                entity.AbsoluteOctets = octets;
            }
            else
            {
                entity.DiffedTimeSpan = (entity.TimeScanned - existing.TimeScanned);
                entity.Octets = (octets - existing.AbsoluteOctets);
                entity.AbsoluteOctets = octets;
            }

            CollectedTrafficDatas.Insert(entity);
        }

        private async Task ProcessTrafficResult(MonitoredPort port, long octets, TrafficType trafficType)
        {
            if (port.FirstTimeScanned == null)
            {
                port.FirstTimeScanned = DateTime.Now;

                port.LastInOctets = 0;
                port.LastOutOctets = 0;
                port.AllInOctets = 0;
                port.AllOutOctets = 0;

                switch (trafficType)
                {
                    case TrafficType.Inbound:
                    {
                        port.LastInOctets = octets;
                        port.AllInOctets = octets;

                        break;
                    }
                    case TrafficType.Outbound:
                    {
                        port.LastOutOctets = octets;
                        port.LastInOctets = octets;

                        break;
                    }
                }
            }

            long lastOctets = 0;
            long overallOctets = 0;

            switch (trafficType)
            {
                case TrafficType.Inbound:
                    lastOctets = port.LastInOctets;
                    overallOctets = port.AllInOctets;

                    break;
                case TrafficType.Outbound:
                    lastOctets = port.LastOutOctets;
                    overallOctets = port.AllOutOctets;

                    break;
            }

            if ((lastOctets - octets) > 10)
            {
                overallOctets += octets;
                lastOctets = octets;
            }
            else
            {
                overallOctets += (octets - lastOctets);
                lastOctets = octets;
            }

            switch (trafficType)
            {
                case TrafficType.Inbound:
                    port.LastInOctets = lastOctets;
                    port.AllInOctets = overallOctets;

                    break;
                case TrafficType.Outbound:
                    port.LastOutOctets = lastOctets;
                    port.AllOutOctets = overallOctets;

                    break;
            }
            port.LastTimeScanned = DateTime.Now;
            MonitoredPorts.Update(port);

            await WriteTrafficData(port, octets, trafficType);
        }
    }
}