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
	public class ItemPageBuilder : BaseEntityPageBuilder<Item>
	{
		protected override void DisplayEntity(Item item)
		{
			AddContentTab ("General", "icon_general.png");
			AddCopyButton ("Copy Item ID", item.Id.ToString ());
			AddProperty ("Type", item.ItemType);
			AddProperty ("Sub Type", item.SubType);
			AddProperty ("Mass Units", item.MassUnits.ToString ());
			AddProperty ("Race", item.Race);

			if (item.Production > 0) {
				AddContentTab ("Production", "icon_production.png");
				AddProperty ("Production", item.Production.ToString ());
				if (item.BlueprintId > 0) {
					AddEntityProperty(Manager, item.BlueprintItem, "Blueprint");
				}
				if (item.SubstituteItemId > 0) {
					AddEntityProperty(Manager, item.SubstituteItem, "Substitute");
					AddProperty("Substitute Ratio", item.SubstituteRatio.ToString());
				}
				if (item.RawMaterials.Count > 0) {
					AddHeading ("Raw Materials");
					foreach (RawMaterial rm in item.RawMaterials) {
						AddEntityProperty(Manager, rm.RawMaterialItem, null, "{0} x " + rm.Quantity);
					}
				}
			}

			AddContentTab ("Tech Manual", "icon_techmanual.png");
			AddLabel (item.TechManual);

			if (item.Properties.Count > 0) {
				AddContentTab ("More", "icon_more.png");
				foreach (ItemProperty prop in item.Properties.Values) {
					AddProperty (prop.Key, prop.Value);
				}
			}

		}
	}
}


