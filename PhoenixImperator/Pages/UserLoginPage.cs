//
// UserLoginPage.cs
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

using Phoenix.BL.Managers;
using Phoenix.BL.Entities;
using Phoenix.Util;

namespace PhoenixImperator.Pages
{
	public class UserLoginPage : PhoenixPage
	{
		public int UserId {
			get {
				return Int32.Parse (userIdEntry.Text);
			}
			private set {
				userIdEntry.Text = value.ToString();
			}
		}

		public string UserCode {
			get {
				return userCodeEntry.Text;
			}
			private set {
				userCodeEntry.Text = value;
			}
		}

		public UserLoginPage () : base()
		{
			BackgroundColor = Color.Black;

			Image logo = new Image { Aspect = Aspect.AspectFill };
			logo.Source = ImageSource.FromFile ("logo.png");

			string headerText;
			if (Phoenix.Application.UserManager.Count () < 1) {
				headerText = "Setup your account";
			} else {
				headerText = "Phoenix Imperator";
			}

			header = new Label { 
				XAlign = TextAlignment.Center,
				Text = headerText,
				TextColor = Color.White,
				FontAttributes = FontAttributes.Bold
			};

			statusMessage = new Label {
				XAlign = TextAlignment.Center,
				Text = "",
				TextColor = Color.Silver
			};

			userIdEntry = new Entry {
				Placeholder = "User ID",
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Keyboard = Keyboard.Numeric,
			};

			userCodeEntry = new Entry {
				Placeholder = "Code",
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};

			userIdEntry.Focus ();

			loginButton = new Button {
				Text = "Login",
				Font = Font.SystemFontOfSize(NamedSize.Large),
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.Black,
				BackgroundColor = Color.White,
				BorderWidth = 1,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			activityIndicator = new ActivityIndicator {
				IsEnabled = true,
				IsRunning = false,
				BindingContext = this
			};

			loginButton.Clicked += LoginButtonClicked;

			Button linkButton = new Button {
				VerticalOptions = LayoutOptions.EndAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Text = "Visit Nexus to get your XML Access Id and Code",
				TextColor = Color.White,
				BackgroundColor = Color.Black
			};

			linkButton.Clicked += (sender, e) => {
				Button button = new Button{
					Text = "Back to Imperator",
					BackgroundColor = Color.Black,
					TextColor = Color.White
				};
				button.Clicked += (sender2, e2) => {
					RootPage.Root.DismissModal();
				};
				ScrollView view = new ScrollView {
					VerticalOptions = LayoutOptions.FillAndExpand
				};
				StackLayout layout = new StackLayout{
					Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5),
					BackgroundColor = Color.Black,
					Children = {
						view,
						button
					}
				};
				WebView browser = new WebView{
					Source = "http://www.phoenixbse.com/index.php?a=user&sa=xml"
				};
				view.Content = browser;

				PhoenixPage page = new PhoenixPage{
					Title = "Nexus",
					Content = layout
				};
				RootPage.Root.NextPageModal(page);
			};

			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			Content = new StackLayout { 
				VerticalOptions = LayoutOptions.StartAndExpand,
				Children = {
					activityIndicator,
					logo,
					header,
					userIdEntry,
					userCodeEntry,
					loginButton,
					linkButton,
					statusMessage
				}
			};

			int userCount = Phoenix.Application.UserManager.Count ();
			Log.WriteLine (Log.Layer.AL, GetType(), "Users: " + userCount);


			if (userCount > 0) {
				Phoenix.Application.UserManager.First ((user) => {
					Device.BeginInvokeOnMainThread (() => {
						linkButton.IsEnabled = false;
						linkButton.IsVisible = false;
					});
					UserCode = user.Code;
					UserId = user.Id;
					Phoenix.Application.UserLoggedIn(user);
				});
			}
		}

		void LoginButtonClicked(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace (userCodeEntry.Text) || string.IsNullOrWhiteSpace (userIdEntry.Text)) {
				ShowErrorAlert ("User Id and Code are required");
				return;
			}
			activityIndicator.IsRunning = true;
			userIdEntry.IsEnabled = false;
			userCodeEntry.IsEnabled = false;
			loginButton.IsEnabled = false;
			Phoenix.Application.UserManager.Save (this.UserId, this.UserCode, (user) => {
				Phoenix.Application.UserLoggedIn(user);

				if(Phoenix.Application.GameStatusManager.Count() < 1){
					// fresh setup
					StackLayout layout = (StackLayout) Content;
					ProgressBar progressBar = new ProgressBar{
						Progress = 0f
					};
					layout.Children.Add(progressBar);
					header.Text = "Setting Up. Please be patient.";
					UpdateStatusMessage("Fetching game status");
					Phoenix.Application.GameStatusManager.Fetch ((results, ex) => {
						UpdateStatusMessage("Fetched game status. Now fetching info.");
						if(ex == null){
							UpdateProgressBar(progressBar, 0.16f);
							Phoenix.Application.InfoManager.Fetch((infoResults, ex2) => {
								UpdateStatusMessage("Fetched info data. Now fetching star systems.");
								if(ex2 == null){
									UpdateProgressBar(progressBar, 0.33f);
									Phoenix.Application.StarSystemManager.Fetch((systemResults, ex3) => {
										UpdateStatusMessage("Fetched star systems. Now fetching order types.");
										if(ex3 == null){
											UpdateProgressBar(progressBar, 0.5f);
											Phoenix.Application.OrderTypeManager.Fetch((orderTypeResults, ex4) => {
												if(ex4 == null){
													UpdateProgressBar(progressBar, 0.67f);
													UpdateStatusMessage("Fetched order types. Now fetching items");
													Phoenix.Application.ItemManager.Fetch((itemResults, ex5) => {
														if(ex5 == null){
															UpdateProgressBar(progressBar, 0.83f);
															UpdateStatusMessage("Fetched items. Now fetching positions.");
															Phoenix.Application.PositionManager.Fetch((positionResults, ex6) => {
																if(ex6 == null){
																	ShowHomePage();
																}
																else {
																	UpdateProgressBar(progressBar, 1.0f);
																	ShowErrorAndThenHome(ex6);
																}
															});
														}
														else {
															ShowErrorAndThenHome(ex5);
														}
													});
												}
												else {
													ShowErrorAndThenHome(ex4);
												}

											});
										}
										else {
											ShowErrorAndThenHome(ex3);
										}
									});
								}
								else {
									ShowErrorAndThenHome(ex2);
								}
							});
						}
						else {
							ShowErrorAndThenHome(ex);
						}

					});
				}
				else {
					ShowHomePage();
				}
			});
		}

		private void UpdateProgressBar(ProgressBar progressBar, float progressTo)
		{
			Device.BeginInvokeOnMainThread (() => {
				progressBar.ProgressTo(progressTo,250,Easing.Linear);
			});
		}

		private void UpdateStatusMessage(string message)
		{
			Device.BeginInvokeOnMainThread(() => {
				statusMessage.Text = message;
			});
		}

		private void ShowErrorAndThenHome(object error)
		{
			ShowErrorAlert(error);
			ShowHomePage ();
		}

		private void ShowHomePage()
		{
			Device.BeginInvokeOnMainThread (() => {
				activityIndicator.IsRunning = false;
				userIdEntry.IsEnabled = true;
				userCodeEntry.IsEnabled = true;
				loginButton.IsEnabled = true;
				statusMessage.Text = "";
				header.Text = "Phoenix Imperator";
				RootPage.Root.GoHome();

			});
		}

		private Label header;
		private Label statusMessage;
		private Entry userIdEntry;
		private Entry userCodeEntry;
		private Button loginButton;
		private ActivityIndicator activityIndicator;
	}
}


