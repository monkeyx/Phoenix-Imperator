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
					AddContentTab("Turn Report");
					WebView browser = new WebView();
					HtmlWebViewSource htmlSource = new HtmlWebViewSource();
					htmlSource.Html = turn;
					browser.Source = htmlSource;
					currentTab.Content = browser;
				});
			});

			AddContentTab("Orders");
			ActivityIndicator ordersActivity = new ActivityIndicator {
				IsEnabled = true,
				IsRunning = true,
				BindingContext = currentTab
			};
			currentLayout.Children.Add (ordersActivity);

			ordersList = new ListView ();
			ordersList.ItemTemplate = new DataTemplate (typeof(TextCell));
			ordersList.ItemTemplate.SetBinding (TextCell.TextProperty, "ListText");
			ordersList.ItemTemplate.SetBinding (TextCell.DetailProperty, "ListDetail");
			currentLayout.Children.Add(ordersList);

			Phoenix.Application.OrderManager.AllForPosition (item.Id, (results) => {
				if(results.Count > 0){
					ordersActivity.IsEnabled = false;
					ordersActivity.IsRunning = false;
					UpdateOrdersList(results);
				}
				else {
					Phoenix.Application.OrderManager.FetchForPosition(item.Id,(fetchResults,ex) => {
						ordersActivity.IsEnabled = false;
						ordersActivity.IsRunning = false;
						if(ex == null){
							UpdateOrdersList(fetchResults);
						}
						else {
							#if DEBUG
							ShowErrorAlert(ex);
							#else
							ShowErrorAlert("Problem connecting to Nexus");
							#endif
						}
					});
				}
			});
		}

		private void UpdateOrdersList(IEnumerable<Order> orders)
		{
			Device.BeginInvokeOnMainThread (() => {
				ordersList.ItemsSource = orders;
			});
		}

		private ListView ordersList;
	}
}


