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
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SnmpSharpNet;
using Topshelf;

namespace TrafficStats.Synchronization
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(hostConfigurator =>
            {
                hostConfigurator.Service<SynchronizationService>(serviceConfigurator =>
                {
                    serviceConfigurator.ConstructUsing(() => new SynchronizationService());
                    serviceConfigurator.WhenStarted(agentService => agentService.Start());
                    serviceConfigurator.WhenStopped(agentService => agentService.Stop());
                });

                hostConfigurator.RunAsPrompt();

                hostConfigurator.DependsOnMsSql();
                hostConfigurator.StartAutomaticallyDelayed();

                hostConfigurator.SetDisplayName("netwatch service");
                hostConfigurator.SetDescription("Keeps all statistics up-to-date.");
                hostConfigurator.SetServiceName("netwatch");
            });
        }
    }
}
