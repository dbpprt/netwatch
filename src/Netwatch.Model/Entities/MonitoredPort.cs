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
    public class MonitoredPort
    {
        [Key, Column(Order = 0)]
        public int PortNumber { get; set; }

        [Required]
        [Key, Column(Order = 1)]
        public string SnmpIpAddress { get; set; }

        [ForeignKey("SnmpIpAddress")]
        public SnmpTarget SnmpTarget { get; set; }

        public string Comment { get; set; }

        [Required]
        public bool ExcludeFromStatistics { get; set; }

        public long LastInOctets { get; set; }

        public long LastOutOctets { get; set; }

        public DateTime? FirstTimeScanned { get; set; }

        public DateTime? LastTimeScanned { get; set; }

        public long AllInOctets { get; set; }

        public long AllOutOctets { get; set; }

        public bool SkipMacAddressScan { get; set; }
    }
}