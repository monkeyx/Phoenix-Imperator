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
				// Text = ""
			};

			userCodeEntry = new Entry {
				Placeholder = "Code",
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				// Text = ""
			};

			userIdEntry.MinimumWidthRequest = 20;
			userCodeEntry.MinimumWidthRequest = 40;

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
					statusMessage
				}
			};

			int userCount = Phoenix.Application.UserManager.Count ();
			Log.WriteLine (Log.Layer.AL, GetType(), "Users: " + userCount);

			if (userCount > 0) {
				Phoenix.Application.UserManager.First ((user) => {
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
							progressBar.ProgressTo(0.16,250,Easing.Linear);
							Phoenix.Application.InfoManager.Fetch((infoResults, ex2) => {
								UpdateStatusMessage("Fetched info data. Now fetching star systems.");
								if(ex2 == null){
									progressBar.ProgressTo(0.33,250,Easing.Linear);
									Phoenix.Application.StarSystemManager.Fetch((systemResults, ex3) => {
										UpdateStatusMessage("Fetched star systems. Now fetching order types.");
										if(ex3 == null){
											progressBar.ProgressTo(0.5,250,Easing.Linear);
											Phoenix.Application.OrderTypeManager.Fetch((orderTypeResults, ex4) => {
												if(ex4 == null){
													progressBar.ProgressTo(0.67,250,Easing.Linear);
													UpdateStatusMessage("Fetched order types. Now fetching items");
													Phoenix.Application.ItemManager.Fetch((itemResults, ex5) => {
														if(ex5 == null){
															progressBar.ProgressTo(0.83,250,Easing.Linear);
															UpdateStatusMessage("Fetched items. Now fetching positions.");
															Phoenix.Application.PositionManager.Fetch((positionResults, ex6) => {
																if(ex6 == null){
																	ShowHomePage();
																}
																else {
																	progressBar.ProgressTo(1.0,250,Easing.Linear);
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


