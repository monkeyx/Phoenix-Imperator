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
using System.Linq;

using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.Util;

namespace PhoenixImperator.Pages.Entities
{
	public class PositionPageBuilder : BaseEntityPageBuilder<Position>
	{
		public static ObservableCollection<Order> Orders { get; set; } = new ObservableCollection<Order> ();
		public static ObservableCollection<Notification> Notifications { get; set; } = new ObservableCollection<Notification> ();
		public static Position CurrentPosition;

		public static void UpdateOrders(IEnumerable<Order> orders)
		{
			Orders.Clear ();
			foreach (Order o in orders) {
				Orders.Add (o);
			}
		}

		public static void UpdateNotifications(IEnumerable<Notification> notifications)
		{
			Notifications.Clear ();
			foreach (Notification n in notifications) {
				Notifications.Add (n);
			}
		}

		protected override void DisplayEntity(Position item)
		{
			CurrentPosition = item;
			AddContentTab ("General", "icon_general.png");
			AddCopyButton ("Copy ID", item.Id.ToString ());
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

			AddContentTab ("Notifications", "icon_notifications.png");

			ListView notificationList = new ListView {
				IsGroupingEnabled = true,
				GroupDisplayBinding = new Binding ("GroupName"),
				GroupShortNameBinding = new Binding ("GroupShortName")
			};
			notificationList.ItemTemplate = new DataTemplate (typeof(NotificationViewCell));
			notificationList.ItemTemplate.SetBinding (TextCell.TextProperty, "ListText");
			notificationList.ItemTemplate.SetBinding (TextCell.DetailProperty, "ListDetail");
			notificationList.ItemTapped += (sender, e) => {
				EntityPageBuilderFactory.ShowEntityPage<Notification>(Phoenix.Application.NotificationManager,((Notification)e.Item).Id);
			};
			currentLayout.Children.Add (notificationList);
			notificationList.IsRefreshing = true;

			Phoenix.Application.NotificationManager.AllForPosition (item.Id, (results) => {
				if(results.Count > 0){
					UpdateNotifications(results);
					EntityListPage<Notification>.GroupEntities (results, (groupedResults) => {
						Device.BeginInvokeOnMainThread (() => {
							notificationList.ItemsSource = groupedResults;
							notificationList.IsRefreshing = false;
						});
					});
				}
			});

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

			StackLayout orderFormLayout = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.StartAndExpand
			};

			currentLayout.Children.Add (orderFormLayout);

			Button addOrderButton = new Button {
				Text = "Add Order",
				TextColor = Color.White,
				BackgroundColor = Color.Blue
			};
			addOrderButton.Clicked += (sender, e) => {
				OrderSelectorPage page = new OrderSelectorPage(CurrentPosition);
				RootPage.Root.NextPageModal(page);
			};


			ordersList = new ListView ();
			ordersList.ItemTemplate = new DataTemplate (typeof(OrderViewCell));
			ordersList.ItemTemplate.SetBinding (TextCell.TextProperty, "ListText");
			ordersList.ItemTemplate.SetBinding (TextCell.DetailProperty, "ListDetail");
			ordersList.ItemTapped += (sender, e) => {
				ordersActivity.IsRunning = true;
				Phoenix.Application.OrderManager.Get(((Order)e.Item).Id,(order) => {
					OrderEditPage page = new OrderEditPage(order);
					Device.BeginInvokeOnMainThread(() => {
						RootPage.Root.NextPage(page);
						ordersList.SelectedItem = null;
						ordersActivity.IsRunning = false;
					});
				});
			};
			ordersList.ItemsSource = Orders;

			Button clearOrdersButton = new Button {
				Text = "Clear Orders",
				TextColor = Color.White,
				BackgroundColor = Color.Red
			};
			clearOrdersButton.Clicked += async (sender, e) => {
				bool confirm = await currentTab.DisplayAlert("Clear Orders","Are you sure?","Yes","No");
				if(confirm){
					Phoenix.Application.OrderManager.ClearOrders (CurrentPosition.Id, (results) => {
						Orders = new ObservableCollection<Order> (results);
						Device.BeginInvokeOnMainThread(() => {
							ordersList.ItemsSource = Orders;
						});
						SwitchToOrdersTab();
					});
				}
			};

			currentLayout.Children.Add (clearOrdersButton);
			currentLayout.Children.Add (ordersActivity);
			currentLayout.Children.Add(ordersList);
			currentLayout.Children.Add (addOrderButton);
			currentLayout.Children.Add(new Label {
				Text = "Tap an order to edit. Swipe left to delete an order.",
				FontSize = Device.GetNamedSize (NamedSize.Small, typeof(Label)),
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				FontAttributes = FontAttributes.Italic
			});

			Phoenix.Application.OrderManager.AllForPosition (item.Id, (results) => {
				if(results.Count > 0){
					Device.BeginInvokeOnMainThread(() => {
						ordersActivity.IsRunning = false;
					});
					UpdateOrders(results);
				}
				else {
					Phoenix.Application.OrderManager.FetchForPosition(item.Id,(fetchResults,ex) => {
						Device.BeginInvokeOnMainThread(() => {
							ordersActivity.IsRunning = false;
						});
						if(ex == null){
							UpdateOrders(fetchResults);
						}
						else {
							ShowErrorAlert(ex);
						}
					});
				}
			});
		}

		void RequestUpdateButtonClicked(object sender, EventArgs e)
		{
			Phoenix.Application.OrderManager.RequestUpdate (CurrentPosition.Id, (results) => {
				Orders = new ObservableCollection<Order> (results);
				Device.BeginInvokeOnMainThread(() => {
					ordersList.ItemsSource = Orders;
				});
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

		private ListView ordersList;
		private Page ordersTab;
	}

	public class OrderViewCell : TextCell
	{
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
			Phoenix.Application.OrderManager.DeleteOrder ((Order)item.CommandParameter,(results) => {
				PositionPageBuilder.UpdateOrders(results);
			});
		}
	}

	public class NotificationViewCell : TextCell
	{
		public NotificationViewCell()
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
			Phoenix.Application.NotificationManager.DeleteNotification ((Notification)item.CommandParameter,(results) => {
				PositionPageBuilder.UpdateNotifications(results);
			});
		}
	}
}


