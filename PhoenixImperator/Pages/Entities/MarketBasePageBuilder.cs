//
// MarketBasePageBuilder.cs
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

using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;

namespace PhoenixImperator.Pages.Entities
{
	public class MarketBasePageBuilder : BaseEntityPageBuilder<MarketBase>
	{
		public static MarketBase CurrentBase { get; set; }

		protected override void DisplayEntity (MarketBase item)
		{
			CurrentBase = item;

			AddBaseTab ();

			AddBuyingTab ();

			AddSellingTab ();
		}

		private void AddBaseTab()
		{
			AddContentTab ("Base", "icon_markets.png");

			// currentTab.AddProperty ("Aff", CurrentBase.AffiliationCode);

			if (CurrentBase.StarSystem != null) {
				currentTab.AddEntityProperty (Phoenix.Application.StarSystemManager, CurrentBase.StarSystem, "Star System");
			} else {
				currentTab.AddPropertyDoubleLine ("Star System", CurrentBase.StarSystemName);
			}

			if (!string.IsNullOrWhiteSpace (CurrentBase.CelestialBodyName)) {
				currentTab.AddPropertyDoubleLine ("Celestial Body", CurrentBase.CelestialBodyName);
			}

			currentTab.AddProperty ("Hiport", CurrentBase.Hiport.ToString ());

			if (CurrentBase.PatchPrice > 0) {
				currentTab.AddProperty ("Patches", "$" + CurrentBase.PatchPrice);
			}

			if (CurrentBase.DockCapacity > 0) {
				currentTab.AddProperty ("Docking", CurrentBase.DockCapacity.ToString() + " hulls");
			}

			if (CurrentBase.MaintenanceComplexes > 0) {
				currentTab.AddProperty ("Maintenance", CurrentBase.MaintenanceComplexes.ToString() + " complexes");
			}
		}

		private void AddBuyingTab()
		{
			AddContentTab ("Buying", "icon_market_buy.png");
			currentTab.AddListViewWithSearchBar (typeof(TextCell), CurrentBase.Buying, (sender, e) => {
				MarketItem item = (MarketItem)e.Item;
				EntityPageBuilderFactory.ShowEntityPage<Item>(Phoenix.Application.ItemManager,item.ItemId);
			});
		}

		private void AddSellingTab()
		{
			AddContentTab ("Selling", "icon_market_sell.png");
			currentTab.AddListViewWithSearchBar (typeof(TextCell), CurrentBase.Selling, (sender, e) => {
				MarketItem item = (MarketItem)e.Item;
				EntityPageBuilderFactory.ShowEntityPage<Item>(Phoenix.Application.ItemManager,item.ItemId);
			});
		}
	}
}

