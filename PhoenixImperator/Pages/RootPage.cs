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

using Xamarin;
using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.Util;

using PhoenixImperator.Pages.Entities;

namespace PhoenixImperator.Pages
{
	public class RootPage : MasterDetailPage
	{
		public static RootPage Root { get; set; }

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

		public void GoHome()
		{
			GoToPage (new HomePage());
		}

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

		public void NextPageModal(Page modalPage)
		{
			((NavigationPage)Detail).Navigation.PushModalAsync (modalPage);
		}

		public void NextPageAfterModal(Page nextPage)
		{
			NextPage (nextPage);
			DismissModal ();
		}

		public void DismissModal()
		{
			((NavigationPage)Detail).Navigation.PopModalAsync ();
		}

		public void NextPage(Page nextPage)
		{
			((NavigationPage)Detail).PushAsync (nextPage);
		}

		public void PreviousPage()
		{
			((NavigationPage)Detail).PopAsync ();
		}

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

		public void ShowNotificationsPage(ActivityIndicator spinner)
		{
			spinner.IsRunning = true;
			NotificationTabbedPage page = new NotificationTabbedPage ();
			if (Phoenix.Application.NotificationManager.Count() > 0) {
				// show local results
				Phoenix.Application.NotificationManager.All ((results) => {
					page.AddNotificationListPage("High", "icon_red_circle.png", results,Notification.NotificationPriority.Red);
					page.AddNotificationListPage("Medium", "icon_amber_circle.png", results,Notification.NotificationPriority.Amber);
					page.AddNotificationListPage("Low", "icon_green_circle.png", results,Notification.NotificationPriority.Green);
					Device.BeginInvokeOnMainThread (() => {
						spinner.IsRunning = false;
						GoToPage (page);
					});
				});
			} else {
				// fetch and show results
				Phoenix.Application.NotificationManager.Fetch ((results, ex) => {
					if(ex == null){
						page.AddNotificationListPage("High", "icon_red_circle.png", results,Notification.NotificationPriority.Red);
						page.AddNotificationListPage("Medium", "icon_amber_circle.png", results,Notification.NotificationPriority.Amber);
						page.AddNotificationListPage("Low", "icon_green_circle.png", results,Notification.NotificationPriority.Green);
						Device.BeginInvokeOnMainThread (() => {
							spinner.IsRunning = false;
							GoToPage (page);
						});
					}
					else {
						menuPage.ShowErrorAlert(ex);
					}
				}, false);
			}
		}

		private MenuPage menuPage;
	}
}

