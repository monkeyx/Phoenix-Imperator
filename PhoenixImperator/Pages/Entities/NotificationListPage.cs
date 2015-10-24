//
// NotificationPage.cs
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
using System.Linq;

using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.Util;

namespace PhoenixImperator.Pages.Entities
{
	public class NotificationTabbedPage : TabbedPage
	{
		public static IEnumerable<Notification> FilterByPriority(IEnumerable<Notification> results, Notification.NotificationPriority priority)
		{
			return from element in results
					where element.Priority == priority
				orderby element.DaysAgo
				select element;
		}

		public NotificationTabbedPage ()
		{
			Title = "Notifications";
			BackgroundColor = Color.White;
		}

		public void AddNotificationListPage(string title, string icon, IEnumerable<Notification> filtered, Notification.NotificationPriority priority)
		{
			NotificationListPage page = new NotificationListPage (title, icon, filtered, priority);
			Device.BeginInvokeOnMainThread (() => {
				Children.Add (page);

			});
		}
	}

	public class NotificationListPage : EntityListPage<Notification>
	{
		public Notification.NotificationPriority Priority { get; private set; }

		public NotificationListPage(string title, string icon, IEnumerable<Notification> notifications, Notification.NotificationPriority priority) : base(title,Phoenix.Application.NotificationManager,NotificationTabbedPage.FilterByPriority(notifications,priority),true,true,false)
		{
			Icon = icon;
			Priority = priority;
		}

		protected override void RefreshList()
		{
			Manager.Fetch((results, ex) => {
				results = NotificationTabbedPage.FilterByPriority(results,Priority);
				if(ex == null){
					GroupEntities (results, (groupedResults) => {
						Device.BeginInvokeOnMainThread (() => {
							listView.ItemsSource = groupedResults;
							listView.IsRefreshing = false;
						});
					});
				}
				else {
					ShowErrorAlert(ex);
				}
			},true);
		}
	}
}

