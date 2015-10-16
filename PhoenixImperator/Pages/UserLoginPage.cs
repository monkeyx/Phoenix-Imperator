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
			set {
				userIdEntry.Text = value.ToString();
			}
		}

		public string UserCode {
			get {
				return userCodeEntry.Text;
			}
			set {
				userCodeEntry.Text = value;
			}
		}

		public UserLoginPage () : base()
		{
			Title = "Login";

			string headerText;
			if (Phoenix.Application.UserManager.Count () < 1) {
				headerText = "Setup your account";
			} else {
				headerText = "Login";
			}

			header = new Label { 
				XAlign = TextAlignment.Center,
				Text = headerText,
				FontAttributes = FontAttributes.Bold
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
				VerticalOptions = LayoutOptions.Center,
				Children = {
					activityIndicator,
					header,
					userIdEntry,
					userCodeEntry,
					loginButton
				}
			};
		}

		void LoginButtonClicked(object sender, EventArgs e)
		{
			activityIndicator.IsRunning = true;
			userIdEntry.IsEnabled = false;
			userCodeEntry.IsEnabled = false;
			loginButton.IsEnabled = false;
			Phoenix.Application.UserManager.Save (this.UserId, this.UserCode, (user) => {
				Phoenix.Application.UserLoggedIn(user);

				HomePage homePage = new HomePage();

				if(Phoenix.Application.GameStatusManager.Count() < 1){
					// fresh setup
					ShowInfoAlert("Set Up", "This is the first time getting information from Nexus so this may take a bit of time. Please be patient");
					Phoenix.Application.GameStatusManager.Fetch ((results, ex) => {
						if(ex == null){
							Device.BeginInvokeOnMainThread(() => {
								header.Text = "Please be patient. Fetched game status. Now fetching info (2/6).";
							});
							IEnumerator<GameStatus> i = results.GetEnumerator();
							if(i.MoveNext())
								homePage.SetStatus(i.Current);
							Phoenix.Application.InfoManager.Fetch((infoResults, ex2) => {
								Device.BeginInvokeOnMainThread(() => {
									header.Text = "Please be patient. Fetched info data. Now fetching star systems (3/6).";
								});
								if(ex2 == null){
									Phoenix.Application.StarSystemManager.Fetch((systemResults, ex3) => {
										Device.BeginInvokeOnMainThread(() => {
											header.Text = "Please be patient. Fetched star systems. Now fetching order types (4/6).";
										});
										if(ex3 == null){
											Phoenix.Application.OrderTypeManager.Fetch((orderTypeResults, ex4) => {
												if(ex4 == null){
													Device.BeginInvokeOnMainThread(() => {
														header.Text = "Please be patient. Fetched order types. Now fetching items (5/6).";
													});
													Phoenix.Application.ItemManager.Fetch((itemResults, ex5) => {
														if(ex5 == null){
															Device.BeginInvokeOnMainThread(() => {
																header.Text = "Please be patient. Fetched items. Now fetching positions (6/6).";
															});
															Phoenix.Application.PositionManager.Fetch((positionResults, ex6) => {
																if(ex6 == null){
																	Device.BeginInvokeOnMainThread(() => {
																		activityIndicator.IsRunning = false;
																		App.NavigationPage.PushAsync(homePage);
																	});
																}
																else {
																	#if DEBUG
																	ShowErrorAlert(ex6);
																	#else
																	ShowErrorAlert("Problem connecting to Nexus");
																	#endif
																	Device.BeginInvokeOnMainThread(() => {
																		activityIndicator.IsRunning = false;
																		App.NavigationPage.PushAsync(homePage);
																	});
																}
															});
														}
														else {
															#if DEBUG
															ShowErrorAlert(ex5);
															#else
															ShowErrorAlert("Problem connecting to Nexus");
															#endif
															Device.BeginInvokeOnMainThread(() => {
																activityIndicator.IsRunning = false;
																App.NavigationPage.PushAsync(homePage);
															});
														}
													});
												}
												else {
													#if DEBUG
													ShowErrorAlert(ex4);
													#else
													ShowErrorAlert("Problem connecting to Nexus");
													#endif
													Device.BeginInvokeOnMainThread(() => {
														activityIndicator.IsRunning = false;
														App.NavigationPage.PushAsync(homePage);
													});
												}

											});
										}
										else {
											#if DEBUG
											ShowErrorAlert(ex3);
											#else
											ShowErrorAlert("Problem connecting to Nexus");
											#endif
											Device.BeginInvokeOnMainThread(() => {
												activityIndicator.IsRunning = false;
												App.NavigationPage.PushAsync(homePage);
											});
										}
									});
								}
								else {
									#if DEBUG
									ShowErrorAlert(ex2);
									#else
									ShowErrorAlert("Problem connecting to Nexus");
									#endif
									Device.BeginInvokeOnMainThread(() => {
										activityIndicator.IsRunning = false;
										App.NavigationPage.PushAsync(homePage);
									});
								}
							});
						}
						else {
							#if DEBUG
							ShowErrorAlert(ex);
							#else
							ShowErrorAlert("Problem connecting to Nexus");
							#endif
							Device.BeginInvokeOnMainThread(() => {
								activityIndicator.IsRunning = false;
								App.NavigationPage.PushAsync(homePage);
							});
						}

					});
				}
				else {
					// show home page
					Phoenix.Application.GameStatusManager.First((result) => {
						Device.BeginInvokeOnMainThread(() => {
							activityIndicator.IsRunning = false;
							homePage.SetStatus(result);
							App.NavigationPage.PushAsync(homePage);
						});
					});
				}
			});
		}

		private Label header;
		private Entry userIdEntry;
		private Entry userCodeEntry;
		private Button loginButton;
		private ActivityIndicator activityIndicator;
	}
}


