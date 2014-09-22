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
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using SnmpSharpNet;
using TrafficStats.DataAccessLayer.Contracts;
using TrafficStats.Model.DataTransfer;
using TrafficStats.Model.Entities;
using TrafficStats.ServiceLayer.Common;
using TrafficStats.ServiceLayer.Contracts;

namespace TrafficStats.ServiceLayer.Services
{
    public class SnmpScannerService : ServiceBase<SnmpScannerService>, ISnmpScannerService
    {
        [Dependency]
        protected IRepository<SnmpTarget> SnmpTargets { get; set; }

        [Dependency]
        protected IRepository<MonitoredPort> MonitoredPorts { get; set; }

        [Dependency]
        protected IRepository<CollectedTrafficData> CollectedTrafficDatas { get; set; }

        [Dependency]
        protected ISnmpResultService SnmpResultService { get; set; }

        public async Task ExecuteTrafficScan()
        {
            var snmpTargets = await SnmpTargets.Query().ToListAsync();

            foreach (var snmpTarget in snmpTargets)
            {
                var avaliable = false;
                try
                {
                    var results = ProcessTargetsTraffic(snmpTarget, SnmpOids.IfHcInOctets);
                    avaliable = true;
                    await SnmpResultService.ProcessTrafficResults(results);
                    results = ProcessTargetsTraffic(snmpTarget, SnmpOids.IfHcOutOctets);
                    await SnmpResultService.ProcessTrafficResults(results);
                }
                catch (Exception)
                {
                    // TODO: Logging
                }


                if (snmpTarget.Reachable != avaliable)
                {
                    await SetReachability(avaliable, snmpTarget.IpAddress);
                }
            }
        }

        public async Task ExecuteMacScan()
        {
            var snmpTargets = await SnmpTargets.Query().ToListAsync();

            foreach (var snmpTarget in snmpTargets)
            {
                var avaliable = false;
                try
                {
                    var results = MapMacAddresses(snmpTarget);
                    await SnmpResultService.ProcessMacScanResults(results);
                    avaliable = true;
                }
                catch (Exception)
                {

                    // TODO: Logging
                }

                if (snmpTarget.Reachable != avaliable)
                {
                    await SetReachability(avaliable, snmpTarget.IpAddress);
                }
            }
        }

        private async Task SetReachability(bool reachable, string snmpTargetIp)
        {
            var target = await SnmpTargets.Query()
                .FirstOrDefaultAsync(snmp => snmp.IpAddress == snmpTargetIp);

            if (target != null)
            {
                target.Reachable = reachable;
                SnmpTargets.Update(target);
                await Context.SaveAsync();
            }
        }

        List<SnmpResult<int, string>> MapMacAddresses(SnmpTarget snmpTarget)
        {
            var fdbAddressTable = ReadFdbAddressTable(snmpTarget);
            var fdbPortTable = ReadFdbPortTable(snmpTarget);

            var portMappings = fdbPortTable.Select(port => new
            {
                PortNumber = Convert.ToInt32(port.Value.ToString()),
                Oid = port.Oid.ToString()
            })
                .GroupBy(mappings => mappings.PortNumber)
                .Where(mappings => mappings.Any())
                .ToDictionary(mapping => mapping.Key, mapping => mapping.ToList());

            var macMappings = fdbAddressTable.Select(port => new
            {
                MacAddress = port.Value.ToString(),
                Oid = port.Oid.ToString().Replace(SnmpOids.Dot1DTpFdbAddress, SnmpOids.Dot1DTpFdbPort)
            })
                .GroupBy(mappings => mappings.Oid)
                .Where(mappings => mappings.Any())
                .ToDictionary(mapping => mapping.Key, mapping => mapping.First().MacAddress);

            var results = new List<SnmpResult<int, string>>();

            foreach (var portMapping in portMappings)
            {
                string macAddress;

                foreach (var oid in portMapping.Value)
                {
                    if (macMappings.TryGetValue(oid.Oid, out macAddress))
                    {
                        var result = new SnmpResult<int, string>
                        {
                            Identifier = portMapping.Key,
                            Result = macAddress.Replace(' ', ':'),
                            SnmpTarget = snmpTarget,
                            Oid = ""
                        };

                        results.Add(result);
                    }
                }


            }

            return results;
        }

        List<Vb> ReadFdbAddressTable(SnmpTarget snmpTarget)
        {
            var community = new OctetString("public");
            var param = new AgentParameters(community) { Version = SnmpVersion.Ver2 };
            var agent = new IpAddress(snmpTarget.IpAddress);
            var target = new UdpTarget((IPAddress)agent, 161, 2000, 1);
            var rootOid = new Oid(SnmpOids.Dot1DTpFdbAddress);
            var lastOid = (Oid)rootOid.Clone();
            var pdu = new Pdu(PduType.GetBulk) { NonRepeaters = 0, MaxRepetitions = 5 };
            var results = new List<Vb>();

            while (lastOid != null)
            {
                if (pdu.RequestId != 0)
                {
                    pdu.RequestId += 1;
                }
                pdu.VbList.Clear();
                pdu.VbList.Add(lastOid);
                var result = (SnmpV2Packet)target.Request(pdu, param);

                if (result != null)
                {
                    if (result.Pdu.ErrorStatus != 0)
                    {
                        Console.WriteLine("Error in SNMP reply. Error {0} index {1}",
                            result.Pdu.ErrorStatus,
                            result.Pdu.ErrorIndex);
                        break;
                    }

                    foreach (var v in result.Pdu.VbList)
                    {
                        if (rootOid.IsRootOf(v.Oid))
                        {
                            Console.WriteLine("{0} ({1}): {2}",
                                v.Oid,
                                SnmpConstants.GetTypeName(v.Value.Type),
                                v.Value);

                            results.Add(v);

                            lastOid = v.Value.Type == SnmpConstants.SMI_ENDOFMIBVIEW ? null : v.Oid;
                        }
                        else
                        {
                            lastOid = null;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No response received from SNMP agent.");
                }
            }
            target.Close();

            return results;
        }

        List<Vb> ReadFdbPortTable(SnmpTarget snmpTarget)
        {
            var community = new OctetString("public");
            var param = new AgentParameters(community) { Version = SnmpVersion.Ver2 };
            var agent = new IpAddress(snmpTarget.IpAddress);
            var target = new UdpTarget((IPAddress)agent, 161, 2000, 1);
            var rootOid = new Oid(SnmpOids.Dot1DTpFdbPort);
            var lastOid = (Oid)rootOid.Clone();
            var pdu = new Pdu(PduType.GetBulk) { NonRepeaters = 0, MaxRepetitions = 5 };
            var results = new List<Vb>();

            while (lastOid != null)
            {
                if (pdu.RequestId != 0)
                {
                    pdu.RequestId += 1;
                }
                pdu.VbList.Clear();
                pdu.VbList.Add(lastOid);
                var result = (SnmpV2Packet)target.Request(pdu, param);

                if (result != null)
                {
                    if (result.Pdu.ErrorStatus != 0)
                    {
                        Console.WriteLine("Error in SNMP reply. Error {0} index {1}",
                            result.Pdu.ErrorStatus,
                            result.Pdu.ErrorIndex);
                        break;
                    }

                    foreach (var v in result.Pdu.VbList)
                    {
                        if (rootOid.IsRootOf(v.Oid))
                        {
                            Console.WriteLine("{0} ({1}): {2}",
                                v.Oid,
                                SnmpConstants.GetTypeName(v.Value.Type),
                                v.Value);

                            results.Add(v);

                            lastOid = v.Value.Type == SnmpConstants.SMI_ENDOFMIBVIEW ? null : v.Oid;
                        }
                        else
                        {
                            lastOid = null;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No response received from SNMP agent.");
                }
            }
            target.Close();

            return results;
        }

        List<SnmpResult<int, long>> ProcessTargetsTraffic(SnmpTarget snmpTarget, string snmpOid, string communityString = "public")
        {
            var results = new List<SnmpResult<int, long>>();

            var community = new OctetString(communityString);
            var param = new AgentParameters(community) { Version = SnmpVersion.Ver2 };
            var agent = new IpAddress(snmpTarget.IpAddress);
            var target = new UdpTarget((IPAddress)agent, 161, 2000, 1);
            var rootOid = new Oid(snmpOid);
            var lastOid = (Oid)rootOid.Clone();
            var pdu = new Pdu(PduType.GetBulk) { NonRepeaters = 0, MaxRepetitions = 5 };

            while (lastOid != null)
            {
                if (pdu.RequestId != 0)
                {
                    pdu.RequestId += 1;
                }
                pdu.VbList.Clear();
                pdu.VbList.Add(lastOid);
                var result = (SnmpV2Packet)target.Request(pdu, param);

                if (result != null)
                {
                    if (result.Pdu.ErrorStatus != 0)
                    {
                        Console.WriteLine("Error in SNMP reply. Error {0} index {1}",
                            result.Pdu.ErrorStatus,
                            result.Pdu.ErrorIndex);
                        break;
                    }

                    foreach (var v in result.Pdu.VbList)
                    {
                        var current = new SnmpResult<int, long>();

                        if (rootOid.IsRootOf(v.Oid))
                        {
                            Console.WriteLine("{0} ({1}): {2}",
                                v.Oid,
                                SnmpConstants.GetTypeName(v.Value.Type),
                                v.Value);

                            current.Result = Convert.ToInt64(v.Value.ToString());
                            current.Identifier = Convert.ToInt32(v.Oid.ToString().Split('.').Last());
                            current.Oid = v.Oid.ToString();
                            current.SnmpTarget = snmpTarget;

                            results.Add(current);

                            lastOid = v.Value.Type == SnmpConstants.SMI_ENDOFMIBVIEW ? null : v.Oid;
                        }
                        else
                        {
                            lastOid = null;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No response received from SNMP agent.");
                }
            }
            target.Close();

            return results;
        }
    }
}
