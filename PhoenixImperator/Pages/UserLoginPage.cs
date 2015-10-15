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
	public class UserLoginPage : ContentPage
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

		public UserLoginPage ()
		{
			Title = "Login";

			string headerText;
			if (Phoenix.Application.UserManager.Count () < 1) {
				headerText = "Setup your account";
			} else {
				headerText = "Login";
			}

			Label header = new Label { 
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

			Button loginButton = new Button {
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
					header,
					userIdEntry,
					userCodeEntry,
					loginButton,
					activityIndicator
				}
			};
		}

		void LoginButtonClicked(object sender, EventArgs e)
		{
			activityIndicator.IsRunning = true;
			Phoenix.Application.UserManager.Save (this.UserId, this.UserCode, (user) => {
				Phoenix.Application.UserLoggedIn(user);

				// now get status from Nexus and update the UI accordingly
				Phoenix.Application.GameStatusManager.Fetch ((results, statusCode) => {
					Log.WriteLine(Log.Layer.UI, this.GetType(), "GameStatus: Status: " + statusCode);
					HomePage homePage = new HomePage();
					IEnumerator<GameStatus> i = results.GetEnumerator();
					if(i.MoveNext())
						homePage.SetStatus(i.Current);

					if(Phoenix.Application.InfoManager.Count() < 1){ // no info or star systems - fetch first
						Device.BeginInvokeOnMainThread(() => {
							DisplayAlert("Set Up", "This is the first time getting information from Nexus so this may take a bit of time. Please be patient","OK");
						});
						Phoenix.Application.InfoManager.Fetch((infoResults, s1) => {
							Phoenix.Application.StarSystemManager.Fetch((systemResults, s2) => {
								Device.BeginInvokeOnMainThread(() => {
									activityIndicator.IsRunning = false;
									App.NavigationPage.PushAsync(homePage);
								});
							});
						});
					}
					else {
						Device.BeginInvokeOnMainThread(() => {
							activityIndicator.IsRunning = false;
							App.NavigationPage.PushAsync(homePage);
						});
					}
				}, true); // clear all previous status messages
			});
		}

		private Entry userIdEntry;
		private Entry userCodeEntry;
		private ActivityIndicator activityIndicator;
	}
}


