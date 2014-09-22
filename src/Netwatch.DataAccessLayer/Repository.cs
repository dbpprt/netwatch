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
using System.Data.Entity;
using System.Linq;
using TrafficStats.DataAccessLayer.Contracts;

namespace TrafficStats.DataAccessLayer
{

    public sealed class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IDbContext _context;
        private readonly IDbSet<TEntity> _dbSet;

        public Repository(IDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public void Update(TEntity entity)
        {
            _dbSet.Attach(entity);
            _context.SetState(entity, EntityState.Modified);
        }

        public void DeleteById(object id)
        {
            var entity = _dbSet.Find(id);

            _dbSet.Attach(entity);
            _context.SetState(entity, EntityState.Deleted);
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Attach(entity);
            _dbSet.Remove(entity);
        }

        public TEntity Insert(TEntity entity)
        {
            _dbSet.Attach(entity);
            _context.SetState(entity, EntityState.Added);

            return entity;
        }

        public void InsertRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Insert(entity);
            }
        }

        public IQueryable<TEntity> Query()
        {
            return _dbSet;
        }
    }
}
