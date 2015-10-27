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
	/// <summary>
	/// Position page builder.
	/// </summary>
	public class PositionPageBuilder : BaseEntityPageBuilder<Position>
	{
		public enum PositionTab
		{
			General = 0,
			Notifications = 1,
			TurnReport = 2,
			Orders = 3,
			Notes = 4
		}

		/// <summary>
		/// Gets or sets the orders.
		/// </summary>
		/// <value>The orders.</value>
		public static ObservableCollection<Order> Orders { get; set; } = new ObservableCollection<Order> ();

		/// <summary>
		/// Gets or sets the notifications.
		/// </summary>
		/// <value>The notifications.</value>
		public static ObservableCollection<Notification> Notifications { get; set; } = new ObservableCollection<Notification> ();

		/// <summary>
		/// The current position.
		/// </summary>
		public static Position CurrentPosition;

		/// <summary>
		/// Updates the orders.
		/// </summary>
		/// <param name="orders">Orders.</param>
		public static void UpdateOrders(IEnumerable<Order> orders)
		{
			Orders.Clear ();
			foreach (Order o in orders) {
				Orders.Add (o);
			}
		}

		/// <summary>
		/// Updates the notifications.
		/// </summary>
		/// <param name="notifications">Notifications.</param>
		public static void UpdateNotifications(IEnumerable<Notification> notifications)
		{
			Notifications.Clear ();
			foreach (Notification n in notifications) {
				Notifications.Add (n);
			}
		}

		/// <summary>
		/// Displaies the entity.
		/// </summary>
		/// <param name="item">Item.</param>
		protected override void DisplayEntity(Position item)
		{
			CurrentPosition = item;

			AddGeneralTab ();

			AddNotificationsTab ();

			AddTurnReportTab ();

			AddOrdersTab ();

			AddNotesTab ();
		}

		void RequestUpdateButtonClicked(object sender, EventArgs e)
		{
			Phoenix.Application.OrderManager.RequestUpdate (CurrentPosition.Id, (results) => {
				UpdateOrders(results);
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

		private void AddGeneralTab()
		{
			AddContentTab ("General", "icon_general.png");
			AddCopyButton ("Copy ID", CurrentPosition.Id.ToString ());

			if (CurrentPosition.StarSystem != null) {
				currentTab.AddEntityProperty (Phoenix.Application.StarSystemManager, CurrentPosition.StarSystem, "Star System", CurrentPosition.SystemText);
			} else {
				currentTab.AddPropertyDoubleLine ("Star System", CurrentPosition.SystemText);
			}

			currentTab.AddPropertyDoubleLine ("Location", CurrentPosition.LocationText);

			if (!string.IsNullOrWhiteSpace (CurrentPosition.PositionClass)) {
				currentTab.AddProperty ("Class", CurrentPosition.PositionClass);
			}

			if (!string.IsNullOrWhiteSpace (CurrentPosition.Size)) {
				currentTab.AddProperty ("Size", CurrentPosition.Size);
			}

			if (!string.IsNullOrWhiteSpace (CurrentPosition.Design)) {
				currentTab.AddProperty ("Design", CurrentPosition.Design);
			}

			Button requestUpdateButton = new Button {
				Text = "Request Update",
				TextColor = Color.White,
				BackgroundColor = Color.Green
			};
			requestUpdateButton.Clicked += RequestUpdateButtonClicked;
			currentTab.PageLayout.Children.Add (requestUpdateButton);
		}

		private void AddNotificationsTab()
		{
			AddContentTab ("Notifications", "icon_notifications.png");
			ListView notificationList = currentTab.AddListView (typeof(NotificationViewCell), null, (sender, e) => {
				Notification note = (Notification)e.Item;
				EntityPageBuilderFactory.ShowEntityPage<Notification>(Phoenix.Application.NotificationManager,note.Id);
			});
			notificationList.IsGroupingEnabled = true;
			notificationList.GroupDisplayBinding = new Binding ("GroupName");
			notificationList.GroupShortNameBinding = new Binding ("GroupShortName");;
			notificationList.IsRefreshing = true;

			Phoenix.Application.NotificationManager.AllForPosition (CurrentPosition.Id, (results) => {
				if(results.Count > 0){
					UpdateNotifications(results);
					EntityGroup.GroupEntities<Notification> (results, (groupedResults) => {
						Device.BeginInvokeOnMainThread (() => {
							notificationList.ItemsSource = groupedResults;
							notificationList.IsRefreshing = false;
						});
					});
				} else {
					Device.BeginInvokeOnMainThread (() => {
						notificationList.IsRefreshing = false;
					});
				}
			});
		}

		private void AddTurnReportTab()
		{
			PhoenixPage turnsPage = AddContentTab("Turn Report", "icon_report.png");
			turnsPage.AddHelpLabel ("Loading...");
			Phoenix.Application.PositionManager.GetTurnReport (CurrentPosition.Id, (turn) => {
				Device.BeginInvokeOnMainThread(() => {
					WebView browser = new WebView();
					HtmlWebViewSource htmlSource = new HtmlWebViewSource();
					htmlSource.Html = turn;
					browser.Source = htmlSource;
					turnsPage.Content = browser;
				});
			});
		}

		private void AddOrdersTab()
		{
			AddContentTab("Orders", "icon_orders.png");
			ordersTab = currentTab;

			ListView ordersList = null;

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

			currentTab.PageLayout.Children.Add (clearOrdersButton);

			ordersList = currentTab.AddListView (typeof(OrderViewCell), null, (sender, e) => {
				currentTab.Spinner.IsRunning = true;
				Phoenix.Application.OrderManager.Get(((Order)e.Item).Id,(order) => {
					OrderEditPage page = new OrderEditPage(order);
					Device.BeginInvokeOnMainThread(() => {
						RootPage.Root.NextPage(page);
						currentTab.Spinner.IsRunning = false;
					});
				});
			});

			Button addOrderButton = new Button {
				Text = "Add Order",
				TextColor = Color.White,
				BackgroundColor = Color.Blue
			};
			addOrderButton.Clicked += (sender, e) => {
				OrderSelectorPage page = new OrderSelectorPage(CurrentPosition);
				RootPage.Root.NextPageModal(page);
			};
			currentTab.PageLayout.Children.Add (addOrderButton);

			currentTab.AddHelpLabel ("Tap an order to edit. Swipe left to delete an order.");

			ordersList.IsRefreshing = true;

			Phoenix.Application.OrderManager.AllForPosition (CurrentPosition.Id, (results) => {
				if(results.Count > 0){
					UpdateOrders(results);
					Device.BeginInvokeOnMainThread(() => {
						ordersList.IsRefreshing = false;
						ordersList.ItemsSource = Orders;
					});
				}
				else {
					Phoenix.Application.OrderManager.FetchForPosition(CurrentPosition.Id,(fetchResults,ex) => {
						if(ex == null){
							UpdateOrders(fetchResults);
						}
						else {
							Orders = new ObservableCollection<Order>();
							ShowErrorAlert(ex);
						}
						Device.BeginInvokeOnMainThread(() => {
							ordersList.IsRefreshing = false;
							ordersList.ItemsSource = Orders;
						});
					});
				}
			});
		}

		private void AddNotesTab()
		{
			AddContentTab ("Notes", "icon_notes.png");
			Editor editor = new Editor {
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.FromHex("eeeeff")
			};
			Phoenix.Application.PositionManager.GetNote (CurrentPosition.Id, (content) => {
				Device.BeginInvokeOnMainThread(() => {
					editor.Text = content;
				});
				editor.Completed += (sender, e) => {
					Phoenix.Application.PositionManager.SaveNote(CurrentPosition.Id,editor.Text,(savedContent) => {
						Log.WriteLine(Log.Layer.UI,GetType(),"Saved note for " + CurrentPosition);
					});
				};
			});
			currentTab.PageLayout.Children.Add (editor);
			currentTab.AddHelpLabel ("Write notes about a position in the text area above");
		}

		private Page ordersTab;
	}

	/// <summary>
	/// Order view cell.
	/// </summary>
	public class OrderViewCell : TextCell
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.Entities.OrderViewCell"/> class.
		/// </summary>
		public OrderViewCell()
		{
			var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
			deleteAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			deleteAction.Clicked += OnDelete;

			this.ContextActions.Add (deleteAction);
		}

		/// <summary>
		/// Raises the delete event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void OnDelete (object sender, EventArgs e)
		{
			var item = (MenuItem)sender;
			Log.WriteLine (Log.Layer.UI,GetType(),"OnDelete: " + item.CommandParameter);
			Phoenix.Application.OrderManager.DeleteOrder ((Order)item.CommandParameter,(results) => {
				PositionPageBuilder.UpdateOrders(results);
			});
		}
	}

	/// <summary>
	/// Notification view cell.
	/// </summary>
	public class NotificationViewCell : TextCell
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.Entities.NotificationViewCell"/> class.
		/// </summary>
		public NotificationViewCell()
		{
			var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
			deleteAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			deleteAction.Clicked += OnDelete;

			this.ContextActions.Add (deleteAction);
		}

		/// <summary>
		/// Raises the delete event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
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


