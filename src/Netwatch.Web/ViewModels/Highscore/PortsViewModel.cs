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

using System.Collections.Generic;
using Netwatch.Model.DataTransfer;

namespace Netwatch.Web.ViewModels.Highscore
{
    public class IndexViewModel
    {
        public List<PortStatistics> InboundLastMinutes { get; set; }
        public List<PortStatistics> OutboundLastMinutes { get; set; }
        public List<PortStatistics> InboundLastHour { get; set; }
        public List<PortStatistics> OutboundLastHour { get; set; }
        public List<PortStatistics> InboundHighscore { get; set; }
        public List<PortStatistics> OutboundHighscore { get; set; }
    }
}