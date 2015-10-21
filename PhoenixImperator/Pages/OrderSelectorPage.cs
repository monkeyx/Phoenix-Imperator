//
// OrderSelectorPage.cs
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

using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.Util;

namespace PhoenixImperator.Pages
{
	public class OrderSelectorPage : PhoenixPage
	{
		public static ObservableCollection<OrderType> Orders { get; set; }

		public static void UpdateOrders(IEnumerable<OrderType> orders)
		{
			Orders.Clear ();
			foreach (OrderType o in orders) {
				Orders.Add (o);
			}
		}

		public OrderSelectorPage (Position position)
		{
			Title = "New Order";

			Padding = new Thickness (10, Device.OnPlatform (20, 0, 0), 10, 5);

			BackgroundColor = Color.Black;

			Orders = new ObservableCollection<OrderType> ();

			SearchBar searchBar = new SearchBar () {
				Placeholder = "Search"
			};
			searchBar.TextChanged += (sender, e) => FilterList (searchBar.Text);
			searchBar.SearchButtonPressed += (sender, e) => {
				FilterList (searchBar.Text);
			};

			listView = new ListView ();
			listView.ItemTemplate = new DataTemplate (typeof(TextCell));
			listView.ItemTemplate.SetBinding (TextCell.TextProperty, "ListText");
			listView.ItemTemplate.SetBinding (TextCell.DetailProperty, "ListDetail");
			listView.ItemsSource = Orders;

			Phoenix.Application.OrderTypeManager.GetOrderTypesForPosition ((Position.PositionFlag)position.PositionType, (results) => {
				UpdateOrders(results);
			});

			listView.ItemTapped += (sender, e) => {
				Log.WriteLine (Log.Layer.UI, this.GetType (), "Tapped: " + e.Item + "(" + e.Item.GetType() + ")");
				((ListView)sender).SelectedItem = null; // de-select the row
				Phoenix.Application.OrderTypeManager.Get(((OrderType)e.Item).Id,(orderType) => {
					Order order = new Order{
						PositionId = position.Id,
						OrderType = orderType,
						OrderTypeId = orderType.Id
					};
					OrderEditPage editPage = new OrderEditPage(order);
					Device.BeginInvokeOnMainThread(() => {
						RootPage.Root.NextPageAfterModal(editPage);
					});
				});
			};


			Content = new StackLayout { 
				Children = {
					searchBar,
					listView
				}
			};
		}

		private void FilterList(string filter)
		{
			listView.BeginRefresh ();

			if (string.IsNullOrWhiteSpace (filter)) {
				listView.ItemsSource = Orders;
			} else {
				listView.ItemsSource = (Orders.Where (x => x.ToString ().ToLower ().Contains (filter.ToLower ())));
			}

			listView.EndRefresh ();
		}

		private ListView listView;
	}
}


