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
using System.Text;
using System.Threading.Tasks;
using TrafficStats.Model;
using TrafficStats.Model.DataTransfer;
using TrafficStats.Model.Entities;

namespace TrafficStats.ServiceLayer.Contracts
{
    public interface ISnmpStatisticsService
    {
        Task<List<PortStatistics>> GetPortStatisticsForSwitch(string ipAddress);
        Task<List<SwitchStatistics>> GetSwitchStatistics();
        Task<SnmpTarget> GetSnmpTarget(string ipAddress);
        Task<List<PortStatistics>> GetInboundHighscore(int countOfEntries);
        Task<List<PortStatistics>> GetOutboundHighscore(int countOfEntries);

        Task<List<PortStatistics>> GetHighscore(int countOfEntries, DateTime startTime, DateTime endTime,
            TrafficType type);

        Task<List<MacPortMapping>> GetMacDetails(string macAddress);
    }
}