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
	/// <summary>
	/// Home page.
	/// </summary>
	public class HomePage : PhoenixPage
	{
		/// <summary>
		/// Sets the status.
		/// </summary>
		/// <param name="status">Status.</param>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.HomePage"/> class.
		/// </summary>
		public HomePage () : base("Home")
		{
			BackgroundColor = Color.Black;

			AddHeader ();

			AddHomeNavigation ();

			AddHelpLabel ("Pull down to update status from Nexus");

			Phoenix.Application.GameStatusManager.First((result) => {
				Device.BeginInvokeOnMainThread(() => {
					SetStatus(result);
				});
			});
		}

		private void AddHeader()
		{
			AddLogo ();

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

			PageLayout.Children.Add (header);
		}

		private void AddHomeNavigation()
		{
			ListView navigationList = new ListView () {
				BackgroundColor = Color.White,
				SeparatorColor = Color.Silver,
				ItemsSource = new [] {"Turns", "Notifications", "Positions", "Orders", "Items", "Star Systems", "Notes"}
			};

			navigationList.IsPullToRefreshEnabled = true;

			navigationList.ItemTapped += (sender, e) => {
				Log.WriteLine(Log.Layer.UI, this.GetType(), "Tapped: " + e.Item);
				((ListView)sender).SelectedItem = null; // de-select the row
				Spinner.IsRunning = true;
				switch(e.Item.ToString()){
				case "Turns":
					RootPage.Root.ShowTurnsPage (Spinner);
					break;
				case "Notifications":
					RootPage.Root.ShowNotificationsPage (Spinner);
					break;
				case "Positions":
					RootPage.Root.ShowPage<Position> (Spinner, e.Item.ToString(), Phoenix.Application.PositionManager);
					break;
				case "Order Types":
					RootPage.Root.ShowPage<OrderType> (Spinner, e.Item.ToString(), Phoenix.Application.OrderTypeManager);
					break;
				case "Items":
					RootPage.Root.ShowPage<Item> (Spinner, e.Item.ToString(), Phoenix.Application.ItemManager);
					break;
				case "Info":
					RootPage.Root.ShowPage<InfoData> (Spinner, e.Item.ToString(), Phoenix.Application.InfoManager,false);
					break;
				case "Star Systems":
					RootPage.Root.ShowPage<StarSystem> (Spinner, e.Item.ToString(), Phoenix.Application.StarSystemManager);
					break;
				case "Orders":
					RootPage.Root.ShowOrdersPage(Spinner);
					break;
				case "Notes":
					RootPage.Root.ShowNotesPage(Spinner);
					break;
				}
			};

			navigationList.RefreshCommand = new Command ((e) => {
				string oldStardate = starDateLabel.Text;
				string oldStatus = statusMessageLabel.Text;
				SetStatus(null);
				Phoenix.Application.GameStatusManager.Fetch ((results, ex) => {
					if(ex == null){
						GameStatus newStatus = null;
						IEnumerator<GameStatus> i = results.GetEnumerator();
						if(i.MoveNext()){
							newStatus = i.Current;
						}
						if(newStatus != null) {
							if(newStatus.StarDate != oldStardate || newStatus.StatusMessage != oldStatus){
								// status has changed so fetch data from Neuxs
								Phoenix.Application.NotificationManager.Fetch((notificationResults,ex2) => {
									if(ex2 == null){
										Phoenix.Application.PositionManager.Fetch((positionResults,ex3) => {
											if(ex3 == null){
												Device.BeginInvokeOnMainThread(() => {
													navigationList.IsRefreshing = false;
													SetStatus(newStatus);
												});
											}
											else {
												Device.BeginInvokeOnMainThread(() => {
													navigationList.IsRefreshing = false;
												});
												ShowErrorAlert(ex3);
											}
										});
									}
									else {
										Device.BeginInvokeOnMainThread(() => {
											navigationList.IsRefreshing = false;
										});
										ShowErrorAlert(ex2);
									}

								});
							} else {
								Device.BeginInvokeOnMainThread(() => {
									navigationList.IsRefreshing = false;
									SetStatus(newStatus);
								});
							}
						}
						else {
							Device.BeginInvokeOnMainThread(() => {
								navigationList.IsRefreshing = false;
							});
							ShowErrorAlert("Unable to get status from Nexus");
						}
					}
					else {
						Device.BeginInvokeOnMainThread(() => {
							navigationList.IsRefreshing = false;
						});
						ShowErrorAlert(ex);
					}

				}, true);
			});

			PageLayout.Children.Add (navigationList);
		}

		private Label starDateLabel;
		private Label statusMessageLabel;
	}
}


