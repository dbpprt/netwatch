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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netwatch.Model.Entities
{
    public class MacPortMapping
    {
        [Required]
        [Key, ForeignKey("MacAddress"), Column(Order = 2)]
        public string Mac { get; set; }

        public MacAddress MacAddress { get; set; }

        [Required]
        [Key, ForeignKey("MonitoredPort"), Column(Order = 0)]
        public int PortNumber { get; set; }

        [Required]
        [Key, ForeignKey("MonitoredPort"), Column(Order = 1)]
        public string SnmpIpAddress { get; set; }

        public MonitoredPort MonitoredPort { get; set; }

        [Required]
        public DateTime LastSeen { get; set; }
    }
}