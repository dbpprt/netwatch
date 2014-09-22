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

using Microsoft.Practices.Unity;
using Netwatch.DataAccessLayer;
using Netwatch.DataAccessLayer.Contracts;
using Netwatch.Model.Entities;
using Netwatch.ServiceLayer.Contracts;

namespace Netwatch.ServiceLayer
{
    public static class DependencyConfig
    {
        public static void RegisterDependencies(IUnityContainer container)
        {
            container
                .RegisterType<IDbContext, EntityContext>(new HierarchicalLifetimeManager(), new InjectionConstructor())
                //.RegisterType<IDbContext, MemoryDbContext>(new InjectionConstructor())
                .RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager())
                .RegisterType<IRepository<CollectedTrafficData>, Repository<CollectedTrafficData>>(
                    new HierarchicalLifetimeManager())
                .RegisterType<IRepository<MonitoredPort>, Repository<MonitoredPort>>(new HierarchicalLifetimeManager())
                .RegisterType<IRepository<MonitoredService>, Repository<MonitoredService>>(
                    new HierarchicalLifetimeManager())
                .RegisterType<IRepository<SnmpTarget>, Repository<SnmpTarget>>(new HierarchicalLifetimeManager())
                .RegisterType<IRepository<PortReport>, Repository<PortReport>>(new HierarchicalLifetimeManager())
                .RegisterType<IRepository<Grouping>, Repository<Grouping>>(new HierarchicalLifetimeManager())
                .RegisterType<IRepository<MacAddress>, Repository<MacAddress>>(new HierarchicalLifetimeManager())
                .RegisterType<IRepository<User>, Repository<User>>(new HierarchicalLifetimeManager())
                .RegisterType<IRepository<MacPortMapping>, Repository<MacPortMapping>>(new HierarchicalLifetimeManager());
        }
    }
}