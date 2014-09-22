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

using System.Threading.Tasks;
using System.Web.Mvc;
using Netwatch.Model.Entities;
using Netwatch.ServiceLayer.Contracts;
using Netwatch.Web.ViewModels.Groupings;

namespace Netwatch.Web.Controllers
{
    public class GroupingsController : BaseController
    {
        private readonly IGroupingService _groupingService;

        public GroupingsController(
            IGroupingService groupingService
            )
        {
            _groupingService = groupingService;
        }

        public async Task<ActionResult> Index()
        {
            var result = await _groupingService.GetGroupings();
            return View(new IndexViewModel
            {
                Groupings = result,
                Group = new Grouping()
            });
        }

        [HttpPost]
        public async Task<ActionResult> AddGrouping(IndexViewModel model)
        {
            await _groupingService.AddGrouping(model.Group);
            return RedirectToAction("Index");
        }
    }
}