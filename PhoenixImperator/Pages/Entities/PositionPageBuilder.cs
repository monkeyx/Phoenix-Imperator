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
using System.Collections.ObjectModel;

using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.Util;

namespace PhoenixImperator.Pages.Entities
{
	public class PositionPageBuilder : BaseEntityPageBuilder<Position>
	{
		public static ObservableCollection<Order> Orders { get; set; }
		public static Position CurrentPosition;

		protected override void DisplayEntity(Position item)
		{
			CurrentPosition = item;
			AddContentTab ("General", "icon_general.png");
			AddLabel (item.PositionTypeString);
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

			Button requestUpdateButton = new Button {
				Text = "Request Update",
				TextColor = Color.White,
				BackgroundColor = Color.Green
			};
			requestUpdateButton.Clicked += RequestUpdateButtonClicked;
			currentLayout.Children.Add (requestUpdateButton);

			Phoenix.Application.PositionManager.GetTurnReport (item.Id, (turn) => {
				Device.BeginInvokeOnMainThread(() => {
					AddContentTab("Turn Report", "icon_report.png");
					WebView browser = new WebView();
					HtmlWebViewSource htmlSource = new HtmlWebViewSource();
					htmlSource.Html = turn;
					browser.Source = htmlSource;
					currentTab.Content = browser;
				});
			});

			AddContentTab("Orders", "icon_orders.png");
			ordersTab = currentTab;
			ActivityIndicator ordersActivity = new ActivityIndicator {
				IsEnabled = true,
				IsRunning = true,
				BindingContext = currentTab
			};
			ordersList = new ListView ();
			ordersList.ItemTemplate = new DataTemplate (typeof(OrderViewCell));
			ordersList.ItemTemplate.SetBinding (TextCell.TextProperty, "ListText");
			ordersList.ItemTemplate.SetBinding (TextCell.DetailProperty, "ListDetail");

			currentLayout.Children.Add (ordersActivity);
			currentLayout.Children.Add (new Label {
				HorizontalOptions = LayoutOptions.Center,
				FontAttributes = FontAttributes.Italic,
				Text = "Swipe left to delete an order"
			});

			currentLayout.Children.Add(ordersList);

			Button clearOrdersButton = new Button {
				Text = "Clear All Orders",
				TextColor = Color.White,
				BackgroundColor = Color.Red
			};
			clearOrdersButton.Clicked += ClearOrdersButtonClicked;
			currentLayout.Children.Add (clearOrdersButton);

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

		void RequestUpdateButtonClicked(object sender, EventArgs e)
		{
			Phoenix.Application.OrderManager.RequestUpdate (CurrentPosition.Id, (results) => {
				Orders = new ObservableCollection<Order> (results);
				ordersList.ItemsSource = Orders;
				SwitchToOrdersTab();
			});
		}

		void ClearOrdersButtonClicked(object sender, EventArgs e)
		{
			Phoenix.Application.OrderManager.ClearOrders (CurrentPosition.Id, (results) => {
				Orders = new ObservableCollection<Order> (results);
				ordersList.ItemsSource = Orders;
				SwitchToOrdersTab();
			});
		}

		protected void SwitchToOrdersTab()
		{
			if (ordersTab != null) {
				Device.BeginInvokeOnMainThread (() => {
					entityPage.CurrentPage = ordersTab;
				});
			}

		}

		private void UpdateOrdersList(IEnumerable<Order> orders)
		{
			Orders = new ObservableCollection<Order> (orders);
			Device.BeginInvokeOnMainThread (() => {
				ordersList.ItemsSource = Orders;
			});
		}

		private ListView ordersList;
		private Page ordersTab;
	}

	public class OrderViewCell : TextCell
	{
		public IEnumerable<Order> Orders { get; set; }

		public OrderViewCell()
		{
			var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
			deleteAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			deleteAction.Clicked += OnDelete;

			this.ContextActions.Add (deleteAction);
		}

		void OnDelete (object sender, EventArgs e)
		{
			var item = (MenuItem)sender;
			Log.WriteLine (Log.Layer.UI,GetType(),"OnDelete: " + item.CommandParameter);
			Phoenix.Application.OrderManager.DeleteOrder ((Order)item.CommandParameter);
			PositionPageBuilder.Orders.Remove ((Order)item.CommandParameter);
		}
	}
}


