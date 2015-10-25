//
// RootPage.cs
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

using Xamarin;
using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.Util;

using PhoenixImperator.Pages.Entities;

namespace PhoenixImperator.Pages
{
	/// <summary>
	/// Root page.
	/// </summary>
	public class RootPage : MasterDetailPage
	{
		/// <summary>
		/// Gets or sets the root.
		/// </summary>
		/// <value>The root.</value>
		public static RootPage Root { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.RootPage"/> class.
		/// </summary>
		public RootPage ()
		{
			Root = this;

			menuPage = new MenuPage ();
			Master = menuPage;
			menuPage.Menu.ItemTapped += (sender, e) => NavigateTo (e.Item as SideMenuItem);

			var navigationPage = new NavigationPage (new UserLoginPage ()){
				BarBackgroundColor = Color.Black,
				BarTextColor = Color.White
			};
			Detail = navigationPage;
		}

		/// <summary>
		/// Gos the home.
		/// </summary>
		public void GoHome()
		{
			GoToPage (new HomePage());
		}

		/// <summary>
		/// Navigates to.
		/// </summary>
		/// <param name="menu">Menu.</param>
		public void NavigateTo (SideMenuItem menu)
		{
			Insights.Track (menu.TargetType.ToString());
			Log.WriteLine (Log.Layer.UI, GetType (), "Navigate To: " + menu.TargetType);
			string menuChoice = menu.TargetType.ToString ();
			switch (menuChoice) {
			case "Notifications":
				ShowNotificationsPage (menuPage.Spinner);
				break;
			case "Items":
				ShowPage<Item> (menuPage.Spinner, menuChoice, Phoenix.Application.ItemManager);
				break;
			case "Orders":
				ShowOrdersPage (menuPage.Spinner);
				break;
			case "Order Types":
				ShowPage<OrderType> (menuPage.Spinner, menuChoice, Phoenix.Application.OrderTypeManager);
				break;
			case "Positions":
				ShowPage<Position> (menuPage.Spinner, menuChoice, Phoenix.Application.PositionManager);
				break;
			case "Star Systems":
				ShowPage<StarSystem> (menuPage.Spinner, menuChoice, Phoenix.Application.StarSystemManager);
				break;
			case "Info":
				ShowPage<InfoData> (menuPage.Spinner, menuChoice, Phoenix.Application.InfoManager, false);
				break;
			case "About":
				GoToPage (new AboutPage ());
				break;
			default:
				Page displayPage = (Page)Activator.CreateInstance ((Type)menu.TargetType);
				GoToPage (displayPage);
				break;
			}

		}

		/// <summary>
		/// Gos to page.
		/// </summary>
		/// <param name="displayPage">Display page.</param>
		public void GoToPage(Page displayPage)
		{
			Log.WriteLine (Log.Layer.UI, GetType (), "Go To Page: " + displayPage);
			if (displayPage == null)
				return;
			var navigationPage = new NavigationPage (displayPage){
				BarBackgroundColor = Color.Black,
				BarTextColor = Color.White
			};
			Detail = navigationPage;

			IsPresented = false;

			menuPage.DeselectMenuItem ();
		}

		/// <summary>
		/// Nexts the page modal.
		/// </summary>
		/// <param name="modalPage">Modal page.</param>
		public void NextPageModal(Page modalPage)
		{
			((NavigationPage)Detail).Navigation.PushModalAsync (modalPage);
		}

		/// <summary>
		/// Nexts the page after modal.
		/// </summary>
		/// <param name="nextPage">Next page.</param>
		public void NextPageAfterModal(Page nextPage)
		{
			NextPage (nextPage);
			DismissModal ();
		}

		/// <summary>
		/// Dismisses the modal.
		/// </summary>
		public void DismissModal()
		{
			((NavigationPage)Detail).Navigation.PopModalAsync ();
		}

		/// <summary>
		/// Nexts the page.
		/// </summary>
		/// <param name="nextPage">Next page.</param>
		public void NextPage(Page nextPage)
		{
			((NavigationPage)Detail).PushAsync (nextPage);
		}

		/// <summary>
		/// Previouses the page.
		/// </summary>
		public void PreviousPage()
		{
			((NavigationPage)Detail).PopAsync ();
		}

		/// <summary>
		/// Shows the page.
		/// </summary>
		/// <param name="spinner">Spinner.</param>
		/// <param name="title">Title.</param>
		/// <param name="manager">Manager.</param>
		/// <param name="entityHasDetail">If set to <c>true</c> entity has detail.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void ShowPage<T>(ActivityIndicator spinner, string title, NexusManager<T> manager, bool entityHasDetail = true) where T :   EntityBase, new()
		{
			spinner.IsRunning = true;
			if (manager.Count() > 0) {
				// show local results
				manager.All ((results) => {
					EntityListPage<T> page = new EntityListPage<T> (title, manager, results, entityHasDetail);
					Device.BeginInvokeOnMainThread (() => {
						spinner.IsRunning = false;
						GoToPage (page);
					});
				});
			} else {
				// fetch and show results
				manager.Fetch ((results, ex) => {
					Device.BeginInvokeOnMainThread (() => {
						spinner.IsRunning = false;
					});
					if(ex == null){
						Page page = new EntityListPage<T> (title, manager, results);
						Device.BeginInvokeOnMainThread (() => {
							GoToPage (page);
						});
					}
					else {
						menuPage.ShowErrorAlert(ex);
					}
				}, false);
			}
		}

		/// <summary>
		/// Shows the orders page.
		/// </summary>
		/// <param name="spinner">Spinner.</param>
		public void ShowOrdersPage(ActivityIndicator spinner)
		{
			spinner.IsRunning = true;
			Phoenix.Application.PositionManager.GetPositionsWithOrders ((results) => {
				OrderPositionsListPage page = new OrderPositionsListPage(results);
				Device.BeginInvokeOnMainThread (() => {
					spinner.IsRunning = false;
					GoToPage (page);
				});
			});
		}

		/// <summary>
		/// Shows the notifications page.
		/// </summary>
		/// <param name="spinner">Spinner.</param>
		public void ShowNotificationsPage(ActivityIndicator spinner)
		{
			spinner.IsRunning = true;
			if (Phoenix.Application.NotificationManager.Count() > 0) {
				// show local results
				Phoenix.Application.NotificationManager.All ((results) => {
					ShowNotificationsPage(spinner,results);
				});
			} else {
				// fetch and show results
				Phoenix.Application.NotificationManager.Fetch ((results, ex) => {
					if(ex == null){
						ShowNotificationsPage(spinner,results);
					}
					else {
						menuPage.ShowErrorAlert(ex);
					}
				}, false);
			}
		}

		private void ShowNotificationsPage(ActivityIndicator spinner, IEnumerable<Notification> results)
		{
			NotificationTabbedPage page = new NotificationTabbedPage ();
			page.AddNotificationListPage("High", "icon_red_circle.png", results,Notification.NotificationPriority.Red);
			page.AddNotificationListPage("Medium", "icon_amber_circle.png", results,Notification.NotificationPriority.Amber);
			page.AddNotificationListPage("Low", "icon_green_circle.png", results,Notification.NotificationPriority.Green);
			Device.BeginInvokeOnMainThread (() => {
				spinner.IsRunning = false;
				GoToPage (page);
			});
		}

		private MenuPage menuPage;
	}
}

