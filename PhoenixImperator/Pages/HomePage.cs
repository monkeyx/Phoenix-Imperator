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
			} else {
				starDateLabel.Text = "...";
				statusMessageLabel.Text = "...";	
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
				ItemsSource = new [] {"Positions", "Orders", "Items", "Star Systems", "Order Types", "Info"}
			};

			navigationList.IsPullToRefreshEnabled = true;

			navigationList.ItemTapped += (sender, e) => {
				Log.WriteLine(Log.Layer.UI, this.GetType(), "Tapped: " + e.Item);
				((ListView)sender).SelectedItem = null; // de-select the row
				switch(e.Item.ToString()){
				case "Positions":
					RootPage.Root.ShowPage<Position> (activityIndicator, e.Item.ToString(), Phoenix.Application.PositionManager);
					break;
				case "Order Types":
					RootPage.Root.ShowPage<OrderType> (activityIndicator, e.Item.ToString(), Phoenix.Application.OrderTypeManager);
					break;
				case "Items":
					RootPage.Root.ShowPage<Item> (activityIndicator, e.Item.ToString(), Phoenix.Application.ItemManager);
					break;
				case "Info":
					RootPage.Root.ShowPage<InfoData> (activityIndicator, e.Item.ToString(), Phoenix.Application.InfoManager,false);
					break;
				case "Star Systems":
					RootPage.Root.ShowPage<StarSystem> (activityIndicator, e.Item.ToString(), Phoenix.Application.StarSystemManager);
					break;
				case "Orders":
					RootPage.Root.ShowOrdersPage(activityIndicator);
					break;
				}
			};

			navigationList.RefreshCommand = new Command ((e) => {
				SetStatus(null);
				Phoenix.Application.GameStatusManager.Fetch ((results, ex) => {
					if(ex == null){
						Phoenix.Application.InfoManager.Fetch((infoResults, ex2) => {
							if(ex2 == null){
								Log.WriteLine(Log.Layer.UI,GetType(),"Info: " + Phoenix.Application.InfoManager.Count());
								Phoenix.Application.OrderTypeManager.Fetch((orderTypeResults, ex3) => {
									if(ex3 != null){
										ShowErrorAlert(ex3);
									} else {
										Log.WriteLine(Log.Layer.UI,GetType(),"Order Types: " + Phoenix.Application.OrderTypeManager.Count());
									}
									navigationList.IsRefreshing = false;
									IEnumerator<GameStatus> i = results.GetEnumerator();
									if(i.MoveNext()){
										SetStatus(i.Current);
									}
								},true);
							}
							else {
								ShowErrorAlert(ex2);
							}

						},true);
					}
					else {
						ShowErrorAlert(ex);
					}

				}, true);
			});

			activityIndicator = new ActivityIndicator {
				IsEnabled = true,
				IsRunning = false,
				BindingContext = this
			};

			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			Content = new StackLayout { 
				Children = {
					activityIndicator,
					logo,
					header,
					navigationList
				}
			};

			Phoenix.Application.GameStatusManager.First((result) => {
				SetStatus(result);
			});
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			Onboarding.ShowOnboarding ((int)UserFlags.SHOWN_ONBOARDING_NEXUS_PULL_TO_REFRESH, "Help", "Pull down to check Nexus status");
		}

		private Label starDateLabel;
		private Label statusMessageLabel;
		private ActivityIndicator activityIndicator;
	}
}


