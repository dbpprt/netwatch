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
using System.Web;

namespace TrafficStats.Web.Common
{
    public static class TimespanExtensions
    {
        public static string ToPrettyFormat(this TimeSpan span)
        {

            if (span == TimeSpan.Zero) return "0 minutes";

            var sb = new StringBuilder();
            if (span.Days > 0)
                sb.AppendFormat("{0} day{1} ", span.Days, span.Days > 1 ? "s" : String.Empty);
            if (span.Hours > 0)
                sb.AppendFormat("{0} hour{1} ", span.Hours, span.Hours > 1 ? "s" : String.Empty);
            if (span.Minutes > 0)
                sb.AppendFormat("{0} minute{1} ", span.Minutes, span.Minutes > 1 ? "s" : String.Empty);
            return sb.ToString();

        }
    }
}
