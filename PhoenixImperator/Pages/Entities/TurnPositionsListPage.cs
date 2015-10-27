//
// TurnsPositionsListPage.cs
//
// Author:
//       Seyed Razavi <monkeyx@gmail.com>
//
// Copyright (c) 2015 Seyed Razavi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin;
using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.Util;

namespace PhoenixImperator.Pages.Entities
{
	/// <summary>
	/// Order positions list page.
	/// </summary>
	public class TurnsPositionsListPage : EntityListPage<Position>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.Entities.OrderPositionsListPage"/> class.
		/// </summary>
		/// <param name="positions">Positions.</param>
		public TurnsPositionsListPage (IEnumerable<Position> positions) : base("Turns",Phoenix.Application.PositionManager,positions,true,false,false)
		{
		}

		/// <summary>
		/// Entities the selected.
		/// </summary>
		/// <param name="manager">Manager.</param>
		/// <param name="item">Item.</param>
		protected override void EntitySelected(NexusManager<Position> manager, Position item)
		{
			EntityPageBuilderFactory.ShowEntityPage<Position>(manager,item.Id,(int)PositionPageBuilder.PositionTab.TurnReport);
		}
	}
}


