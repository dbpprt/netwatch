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
using System.Security.Cryptography;
using System.Web;

namespace TrafficStats.Web.ViewModels.Shared
{
    public class LineChartDataSet
    {
        public string FillColor { get; set; }
        public string StrokeColor { get; set; }
        public string PointColor { get; set; }
        public string PointStrokeColor { get; set; }

        public Dictionary<string, string> Options { get; set; } 
        public List<long> Data { get; set; }

        public LineChartDataSet()
        {
            Options = new Dictionary<string, string>();
        }

        public LineChartDataSet WithFirstColorSchema()
        {
            FillColor = "rgba(220,220,220,0.5)";
            StrokeColor = "rgba(220,220,220,1)";
            PointColor = "rgba(220,220,220,1)";
            PointStrokeColor = "#fff";

            return this;
        }

        public LineChartDataSet WithSecondColorSchema()
        {
            FillColor = "rgba(151,187,205,0.5)";
            StrokeColor = "rgba(151,187,205,1)";
            PointColor = "rgba(151,187,205,1)";
            PointStrokeColor = "#fff";

            return this;
        }

        public string ToJavaScript()
        {
            //{
            //    fillColor: "rgba(220,220,220,0.5)",
            //    strokeColor: "rgba(220,220,220,1)",
            //    pointColor: "rgba(220,220,220,1)",
            //    pointStrokeColor: "#fff",
            //    data: [65, 59, 90, 81, 56, 55, 40]
            //},
            //{
            //    fillColor: "rgba(151,187,205,0.5)",
            //    strokeColor: "rgba(151,187,205,1)",
            //    pointColor: "rgba(151,187,205,1)",
            //    pointStrokeColor: "#fff",
            //    data: [28, 48, 40, 10, 96, 27, 100]
            //}

            var result = "{\n";
            result += "fillColor: " + "\"" + FillColor + "\",\n";
            result += "strokeColor: " + "\"" + StrokeColor + "\",\n";
            result += "pointColor: " + "\"" + PointColor + "\",\n";
            result += "pointStrokeColor: " + "\"" + PointStrokeColor + "\",\n";
            result += "data: [" + String.Join(", ", Data) + "]\n";

            result = Options.Aggregate(result, (current, option) => current + String.Format("{0}: \"{1}\",\n", option.Key, option.Value));

            result += "},\n";

            return result;
        }
    }

    public class LineChartViewModel
    {
        public string Id { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public List<string> Labels { get; set; }
        public List<LineChartDataSet> DataSets { get; set; }

        public Dictionary<string, string> Options { get; set; }

        public LineChartViewModel()
        {
            Options = new Dictionary<string, string>();
        }

        public LineChartViewModel AddOption(string key, string value)
        {
            Options.Add(key, value);
            return this;
        }
    }


    //{
    //                fillColor: "rgba(220,220,220,0.5)",
    //                strokeColor: "rgba(220,220,220,1)",
    //                pointColor: "rgba(220,220,220,1)",
    //                pointStrokeColor: "#fff",
    //                data: [65, 59, 90, 81, 56, 55, 40]
    //            },
}
