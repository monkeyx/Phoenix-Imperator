//
// ItemPage.cs
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
	/// <summary>
	/// Item page builder.
	/// </summary>
	public class ItemPageBuilder : BaseEntityPageBuilder<Item>
	{
		/// <summary>
		/// Gets or sets the current item.
		/// </summary>
		/// <value>The current item.</value>
		public static Item CurrentItem { get; set; }

		/// <summary>
		/// Displaies the entity.
		/// </summary>
		/// <param name="item">Item.</param>
		protected override void DisplayEntity(Item item)
		{
			CurrentItem = item;

			AddGeneralTab ();

			AddProductionTab ();

			AddTechManual ();

			AddMarketsTab ();

		}

		private void AddGeneralTab()
		{
			AddContentTab ("General", "icon_general.png");
			AddCopyButton ("Copy Item ID", CurrentItem.Id.ToString ());
			currentTab.AddProperty ("Type", CurrentItem.ItemType);
			currentTab.AddProperty ("Sub Type", CurrentItem.SubType);
			currentTab.AddProperty ("Mass Units", CurrentItem.MassUnits.ToString ());
			currentTab.AddProperty ("Race", CurrentItem.Race);

			if (CurrentItem.Properties.Count > 0) {
				foreach (ItemProperty prop in CurrentItem.Properties.Values) {
					currentTab.AddProperty (prop.Key, prop.Value);
				}
			}
		}

		private void AddProductionTab()
		{
			if (CurrentItem.Production > 0) {
				AddContentTab ("Production", "icon_production.png");
				currentTab.AddProperty ("Production", CurrentItem.Production.ToString ());
				if (CurrentItem.BlueprintId > 0) {
					currentTab.AddEntityProperty(Manager, CurrentItem.BlueprintItem, "Blueprint");
				}
				if (CurrentItem.SubstituteItemId > 0) {
					currentTab.AddEntityProperty(Manager, CurrentItem.SubstituteItem, "Substitute");
					currentTab.AddProperty("Substitute Ratio", CurrentItem.SubstituteRatio.ToString());
				}
				if (CurrentItem.RawMaterials.Count > 0) {
					currentTab.AddHeading ("Raw Materials");
					foreach (RawMaterial rm in CurrentItem.RawMaterials) {
						currentTab.AddEntityProperty(Manager, rm.RawMaterialItem, null, "{0} x " + rm.Quantity);
					}
				}
			}
		}

		private void AddTechManual()
		{
			AddContentTab ("Tech Manual", "icon_techmanual.png");
			currentTab.AddLabel (CurrentItem.TechManual);
		}

		private void AddMarketsTab()
		{
			AddContentTab ("Markets", "icon_markets.png");
			currentTab.AddListViewWithSearchBar (typeof(TextCell), CurrentItem.Markets, (sender, e) => {
				MarketBase mb = (MarketBase) e.Item;
				EntityPageBuilderFactory.ShowEntityPage<MarketBase> (Phoenix.Application.MarketManager, mb.Id);
			});
		}
	}
}


