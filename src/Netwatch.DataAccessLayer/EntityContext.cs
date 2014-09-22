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


using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using TrafficStats.DataAccessLayer.Common;
using TrafficStats.DataAccessLayer.Contracts;
using TrafficStats.DataAccessLayer.Migrations;
using TrafficStats.Model.Entities;

namespace TrafficStats.DataAccessLayer
{
    public class EntityContext : DbContext, IDbContext
    {
        public DbSet<SnmpTarget> SnmpTargets { get; set; }
        public DbSet<MonitoredPort> MonitoredPorts { get; set; }
        public DbSet<CollectedTrafficData> CollectedTrafficDatas { get; set; }
        public DbSet<MonitoredService> MonitoredServices { get; set; }
        public DbSet<PortReport> PortReports { get; set; }
        public DbSet<Grouping> Groupings { get; set; }
        public DbSet<MacAddress> MacAddresses { get; set; }
        public DbSet<MacPortMapping> MacPortMappings { get; set; }
        public DbSet<User> Users { get; set; } 

        static EntityContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EntityContext, Configuration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Add(new DateTime2Convention());
        }

        public EntityContext()
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
        }

        public EntityContext(string nameOrConnectionString) :
            base(nameOrConnectionString)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;

        }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public void SetState(object o, EntityState state)
        {
            Entry(o).State = state;
        }

        public EntityState GetState(object o)
        {
            return Entry(o).State;
        }

    }
}
