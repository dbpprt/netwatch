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
using System.Data.Entity;
using System.Threading.Tasks;
using Netwatch.DataAccessLayer.Contracts;
using Netwatch.Model.Entities;
using Netwatch.ServiceLayer.Common;
using Netwatch.ServiceLayer.Contracts;

namespace Netwatch.ServiceLayer.Services
{
    public class GroupingService : ServiceBase<GroupingService>, IGroupingService
    {
        private readonly IRepository<Grouping> _groupings;

        public GroupingService(
            IRepository<Grouping> groupings
            )
        {
            _groupings = groupings;
        }

        public Task<List<Grouping>> GetGroupings()
        {
            return _groupings.Query()
                .ToListAsync();
        }

        public Task<Grouping> GetGrouping(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Grouping> AddGrouping(Grouping grouping)
        {
            grouping.Id = Guid.NewGuid();
            _groupings.Insert(grouping);
            await Context.SaveAsync();
            return grouping;
        }

        public Task<Grouping> UpdateGrouping(Grouping grouping)
        {
            throw new NotImplementedException();
        }
    }
}