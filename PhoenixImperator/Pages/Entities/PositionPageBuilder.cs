//
// PositionPage.cs
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
using Phoenix.Util;

namespace PhoenixImperator.Pages.Entities
{
	public class PositionPageBuilder : BaseEntityPageBuilder<Position>
	{
		protected override void DisplayEntity(Position item)
		{
			AddContentTab ("General");
			if (item.StarSystem != null) {
				AddEntityProperty (Phoenix.Application.StarSystemManager, item.StarSystem, "Star System", item.SystemText);
			} else {
				AddPropertyDoubleLine ("Star System", item.SystemText);
			}
			AddPropertyDoubleLine ("Location", item.LocationText);
			if (!string.IsNullOrWhiteSpace (item.PositionClass)) {
				AddProperty ("Class", item.PositionClass);
			}
			if (!string.IsNullOrWhiteSpace (item.Size)) {
				AddProperty ("Size", item.Size);
			}
			if (!string.IsNullOrWhiteSpace (item.Design)) {
				AddProperty ("Design", item.Design);
			}

			Phoenix.Application.PositionManager.GetTurnReport (item.Id, (turn) => {
				Device.BeginInvokeOnMainThread(() => {
					WebView browser = new WebView();
					HtmlWebViewSource htmlSource = new HtmlWebViewSource();
					htmlSource.Html = turn.Replace("body {background:url(black/space.png);}","body {background-color: black;}");
					browser.Source = htmlSource;
					AddContentTab("Turn Report");
					currentTab.Content = browser;
				});
			});
		}
	}
}


