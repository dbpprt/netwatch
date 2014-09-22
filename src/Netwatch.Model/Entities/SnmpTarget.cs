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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficStats.Model.Entities
{
    public class SnmpTarget
    {
        [Required]
        public string DisplayName { get; set; }

        [Key]
        public string IpAddress { get; set; }

        [Required]
        public string Location { get; set; }

        public string Description { get; set; }

        [Required, ForeignKey("Group")]
        public Guid GroupId { get; set; }

        [Required]
        public Grouping Group { get; set; }

        public bool Reachable { get; set; }
    }
}
