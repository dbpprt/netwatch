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
using System.Data.Entity.Migrations;
using TrafficStats.Model.Entities;

namespace TrafficStats.DataAccessLayer.Migrations
{

    internal sealed class Configuration : DbMigrationsConfiguration<EntityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

#if DEBUG
            AutomaticMigrationDataLossAllowed = true;
            AutomaticMigrationsEnabled = true;
#endif
        }

        protected override void Seed(EntityContext context)
        {
            //context.SnmpTargets.AddOrUpdate(snmpTarget => snmpTarget.IpAddress, new SnmpTarget[]
            //{
            //    new SnmpTarget
            //    {
            //        Description = "Switch 1 - 3.OG ",
            //        DisplayName = "switch-og3-1",
            //        IpAddress = "134.93.50.1",
            //        Location = "3. Obergeschoss - Altbau"
            //    },
            //    new SnmpTarget
            //    {
            //        Description = "Switch 2 - 3.OG ",
            //        DisplayName = "switch-og3-2",
            //        IpAddress = "134.93.50.2",
            //        Location = "3. Obergeschoss - Altbau"
            //    },
            //    new SnmpTarget
            //    {
            //        Description = "Switch 3 - 3.OG ",
            //        DisplayName = "switch-og3-3",
            //        IpAddress = "134.93.50.3",
            //        Location = "3. Obergeschoss - Altbau"
            //    },
            //    new SnmpTarget
            //    {
            //        Description = "Switch 1 - 3.OG ",
            //        DisplayName = "switch-og3-4",
            //        IpAddress = "134.93.50.4",
            //        Location = "3. Obergeschoss - Altbau"
            //    },
            //    new SnmpTarget
            //    {
            //        Description = "Switch 5 - 3.OG ",
            //        DisplayName = "switch-og3-5",
            //        IpAddress = "134.93.50.5",
            //        Location = "3. Obergeschoss - Altbau"
            //    },
            //    new SnmpTarget
            //    {
            //        Description = "Switch - Keller ",
            //        DisplayName = "switch-keller",
            //        IpAddress = "134.93.50.6",
            //        Location = "NAG - Keller"
            //    },
            //    new SnmpTarget
            //    {
            //        Description = "Switch 1 - Neubau",
            //        DisplayName = "switch-neubau-1",
            //        IpAddress = "134.93.50.7",
            //        Location = "Neubau Eingang Außenbereich"
            //    },
            //    new SnmpTarget
            //    {
            //        Description = "Switch 2 - Neubau",
            //        DisplayName = "switch-neubau-2",
            //        IpAddress = "134.93.50.8",
            //        Location = "Neubau Eingang Außenbereich"
            //    },
            //});

            //context.MonitoredServices.AddOrUpdate(service => service.Name, new MonitoredService[]
            //{
            //    new MonitoredService
            //    {
            //        Address = "134.93.48.49",
            //        IsUpAndRunning = true,
            //        LastTimeScanned = DateTime.Now,
            //        LastStatusChange = DateTime.Now,
            //        Name = "Uplink Funkbrücke"
            //    },
            //    new MonitoredService
            //    {
            //        Address = "bruce.core.wohnheim.uni-mainz.de",
            //        IsUpAndRunning = true,
            //        LastTimeScanned = DateTime.Now,
            //        LastStatusChange = DateTime.Now,
            //        Name = "DHCP Server"
            //    },
            //    new MonitoredService
            //    {
            //        Address = "login.wohnheim.uni-mainz.de",
            //        IsUpAndRunning = true,
            //        LastTimeScanned = DateTime.Now,
            //        LastStatusChange = DateTime.Now,
            //        Name = "Login Server"
            //    },
            //    new MonitoredService
            //    {
            //        Address = "134.93.50.1",
            //        IsUpAndRunning = true,
            //        LastTimeScanned = DateTime.Now,
            //        LastStatusChange = DateTime.Now,
            //        Name = "switch-3og-1"
            //    },
            //   new MonitoredService
            //    {
            //        Address = "134.93.50.2",
            //        IsUpAndRunning = true,
            //        LastTimeScanned = DateTime.Now,
            //        LastStatusChange = DateTime.Now,
            //        Name = "switch-3og-2"
            //    },
            //    new MonitoredService
            //    {
            //        Address = "134.93.50.3",
            //        IsUpAndRunning = true,
            //        LastTimeScanned = DateTime.Now,
            //        LastStatusChange = DateTime.Now,
            //        Name = "switch-3og-3"
            //    },
            //    new MonitoredService
            //    {
            //        Address = "134.93.50.4",
            //        IsUpAndRunning = true,
            //        LastTimeScanned = DateTime.Now,
            //        LastStatusChange = DateTime.Now,
            //        Name = "switch-3og-4"
            //    },
            //    new MonitoredService
            //    {
            //        Address = "134.93.50.5",
            //        IsUpAndRunning = true,
            //        LastTimeScanned = DateTime.Now,
            //        LastStatusChange = DateTime.Now,
            //        Name = "switch-3og-5"
            //    },
            //    new MonitoredService
            //    {
            //        Address = "134.93.50.6",
            //        IsUpAndRunning = true,
            //        LastTimeScanned = DateTime.Now,
            //        LastStatusChange = DateTime.Now,
            //        Name = "switch-keller"
            //    },
            //    new MonitoredService
            //    {
            //        Address = "134.93.50.7",
            //        IsUpAndRunning = true,
            //        LastTimeScanned = DateTime.Now,
            //        LastStatusChange = DateTime.Now,
            //        Name = "switch-neubau-1"
            //    },
            //    new MonitoredService
            //    {
            //        Address = "134.93.50.8",
            //        IsUpAndRunning = true,
            //        LastTimeScanned = DateTime.Now,
            //        LastStatusChange = DateTime.Now,
            //        Name = "switch-neubau-2"
            //    },
            //});
        }
    }
}
