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
using System.Collections;
using System.Data.Common;
using System.Data.Entity;
using System.Threading.Tasks;
using Netwatch.DataAccessLayer.Contracts;

namespace Netwatch.DataAccessLayer.Common
{
    public class MemoryDbContext : IDbContext
    {
        private Hashtable _dbSets;


        public IDbSet<T> Set<T>() where T : class
        {
            if (_dbSets == null)
                _dbSets = new Hashtable();

            var type = typeof (T).Name;

            if (!_dbSets.ContainsKey(type))
            {
                var repositoryType = typeof (MemoryDbSet<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType
                        .MakeGenericType(typeof (T)));

                _dbSets.Add(type, repositoryInstance);
            }

            return (IDbSet<T>) _dbSets[type];
        }

        public int SaveChanges()
        {
            return 0;
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.Run(() => SaveChanges());
        }

        public void SetState(object o, EntityState state)
        {
        }

        public EntityState GetState(object o)
        {
            return EntityState.Unchanged;
        }

        public void Dispose()
        {
        }

        public DbConnection GetConnection()
        {
            return null;
        }
    }
}