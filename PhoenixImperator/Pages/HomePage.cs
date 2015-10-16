//
// HomePage.cs
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

using PhoenixImperator.Pages.Entities;

namespace PhoenixImperator.Pages
{
	public class HomePage : PhoenixPage
	{
		public void SetStatus(GameStatus status)
		{
			Log.WriteLine(Log.Layer.UI, this.GetType(), "GameStatus: " + status);
			if (status != null) {
				starDateLabel.Text = status.StarDate;
				statusMessageLabel.Text = status.StatusMessage;
			}
		}

		public HomePage () : base()
		{
			Title = "Home";

			BackgroundColor = Color.Black;

			Image logo = new Image { Aspect = Aspect.AspectFill };
			logo.Source = ImageSource.FromFile ("logo.png");

			starDateLabel = new Label {
				TextColor = Color.Silver
			};
			statusMessageLabel = new Label {
				TextColor = Color.Silver
			};

			StackLayout header = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Children = {
					new Label{
						Text = "Status:",
						FontAttributes = FontAttributes.Bold,
						TextColor = Color.White
					},
					statusMessageLabel,
					new Label{
						Text = "Star Date:",
						FontAttributes = FontAttributes.Bold,
						TextColor = Color.White
					},
					starDateLabel
				}
			};

			ListView navigationList = new ListView () {
				BackgroundColor = Color.Gray,
				SeparatorColor = Color.Silver,
				#if DEBUG
				ItemsSource = new [] {"Positions", "Orders", "Items", "Star Systems", "Order Types", "Info"}
				#else
				ItemsSource = new [] {"Positions", "Orders", "Items", "Star Systems"}
				#endif
			};

			navigationList.IsPullToRefreshEnabled = true;

			navigationList.ItemTapped += (sender, e) => {
				Log.WriteLine(Log.Layer.UI, this.GetType(), "Tapped: " + e.Item);
				((ListView)sender).SelectedItem = null; // de-select the row
				switch(e.Item.ToString()){
				case "Positions":
					ShowPage<Position> (e.Item.ToString(), Phoenix.Application.PositionManager);
					break;
				case "Order Types":
					ShowPage<OrderType> (e.Item.ToString(), Phoenix.Application.OrderTypeManager);
					break;
				case "Items":
					ShowPage<Item> (e.Item.ToString(), Phoenix.Application.ItemManager);
					break;
				case "Info":
					ShowPage<InfoData> (e.Item.ToString(), Phoenix.Application.InfoManager,false);
					break;
				case "Star Systems":
					ShowPage<StarSystem> (e.Item.ToString(), Phoenix.Application.StarSystemManager);
					break;
				}
			};

			navigationList.RefreshCommand = new Command ((e) => {
				refreshHelpText.IsVisible = false;
				Phoenix.Application.GameStatusManager.Fetch ((results, ex) => {
					IEnumerator<GameStatus> i = results.GetEnumerator();
					if(i.MoveNext())
						SetStatus(i.Current);
					refreshHelpText.IsVisible = true;
					navigationList.IsRefreshing = false;
				}, true);
				Phoenix.Application.InfoManager.Fetch((infoResults, ex2) => {});
				Phoenix.Application.OrderTypeManager.Fetch((orderTypeResults, ex4) => {});
			});

			activityIndicator = new ActivityIndicator {
				IsEnabled = true,
				IsRunning = false,
				BindingContext = this
			};

			refreshHelpText = new Label {
				HorizontalOptions = LayoutOptions.Center,
				Text = "Pull down to check Nexus status",
				TextColor = Color.White,
				FontAttributes = FontAttributes.Italic
			};

			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			Content = new StackLayout { 
				Children = {
					activityIndicator,
					logo,
					header,
					navigationList,
					refreshHelpText
				}
			};
		}

		private void ShowPage<T>(string title, NexusManager<T> manager, bool entityHasDetail = true) where T :   EntityBase, new()
		{
			activityIndicator.IsRunning = true;
			if (manager.Count() > 0) {
				// show local results
				manager.All ((results) => {
					EntityListPage<T> page = new EntityListPage<T> (title, manager, results);
					page.EntityHasDetail = entityHasDetail;
					Device.BeginInvokeOnMainThread (() => {
						activityIndicator.IsRunning = false;
						App.NavigationPage.PushAsync (page);
					});
				});
			} else {
				// fetch and show results
				manager.Fetch ((results, ex) => {
					if(ex == null){
						Page page = new EntityListPage<T> (title, manager, results);
						Device.BeginInvokeOnMainThread (() => {
							activityIndicator.IsRunning = false;
							App.NavigationPage.PushAsync (page);
						});
					}
					else {
						#if DEBUG
						ShowErrorAlert(ex);
						#else
						ShowErrorAlert("Problem connecting to Nexus");
						#endif
					}
				}, false);
			}
		}

		private Label starDateLabel;
		private Label statusMessageLabel;
		private ActivityIndicator activityIndicator;
		private Label refreshHelpText;
	}
}


