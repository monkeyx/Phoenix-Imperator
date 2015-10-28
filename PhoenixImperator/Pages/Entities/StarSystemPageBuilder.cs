//
// StarSystemPage.cs
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
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

using XLabs.Forms.Controls;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.Util;

namespace PhoenixImperator.Pages.Entities
{
	/// <summary>
	/// Star system page builder.
	/// </summary>
	public class StarSystemPageBuilder : BaseEntityPageBuilder<StarSystem>
	{
		/// <summary>
		/// Gets or sets the current star system.
		/// </summary>
		/// <value>The current star system.</value>
		public static StarSystem CurrentStarSystem { get; set; }

		/// <summary>
		/// Displaies the entity.
		/// </summary>
		/// <param name="item">Item.</param>
		protected override void DisplayEntity(StarSystem item)
		{
			CurrentStarSystem = item;
			AddCelestialBodiesTab ();
			AddJumpLinksTab ();
			AddPositionsTab ();
			AddMarketsTab ();
		}

		private void AddCelestialBodiesTab()
		{
			if (CurrentStarSystem.CelestialBodies.Count > 0) {
				AddContentTab ("Celestial Bodies","icon_celestialbodies.png");

				currentTab.AddProperty ("Periphery", CurrentStarSystem.PeripheryName);
				AddCopyButton ("Copy System ID", CurrentStarSystem.Id.ToString ());

				currentTab.AddListView (typeof(TextCell),CurrentStarSystem.CelestialBodies,(sender,e) => {
					App.ClipboardService.CopyToClipboard(((CelestialBody)e.Item).LocalCelestialBodyId.ToString());
				});

				currentTab.AddHelpLabel ("Tap a planet to copy its ID");
			}
		}

		private void AddJumpLinksTab()
		{
			if (CurrentStarSystem.JumpLinks.Count > 0) {
				AddContentTab ("Jump Links","icon_jumplink.png");

				currentTab.AddProperty ("Periphery", CurrentStarSystem.PeripheryName);

				currentTab.AddListView (typeof(TextCell),CurrentStarSystem.JumpLinks,(sender,e) => {
					StarSystem ss = ((JumpLink)e.Item).ToStarSysytem;
					EntityPageBuilderFactory.ShowEntityPage<StarSystem> (Manager, ss.Id);
				});

				currentTab.AddHelpLabel ("Tap a link to view system");
			}
		}

		private void AddPositionsTab()
		{
			Phoenix.Application.PositionManager.GetPositionsInStarSystem (CurrentStarSystem, (results) => {
				if(results.GetEnumerator().MoveNext()){
					Device.BeginInvokeOnMainThread (() => {
						AddContentTab ("Positions","icon_positions.png");

						currentTab.AddListViewWithSearchBar (typeof(TextCell),results,(sender,e) => {
							Position p = (Position)e.Item;
							EntityPageBuilderFactory.ShowEntityPage<Position> (Phoenix.Application.PositionManager, p.Id);
						});
					});
				}
			});
		}

		private void AddMarketsTab()
		{
			AddContentTab ("Markets", "icon_markets.png");
			currentTab.AddListViewWithSearchBar (typeof(TextCell), CurrentStarSystem.Markets, (sender, e) => {
				MarketBase mb = (MarketBase) e.Item;
				EntityPageBuilderFactory.ShowEntityPage<MarketBase> (Phoenix.Application.MarketManager, mb.Id);
			});
		}
	}
}


