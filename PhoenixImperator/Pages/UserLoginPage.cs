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

		public UserLoginPage () : base("Phoenix Imperator")
		{
			BackgroundColor = Color.Black;

			AddLogo ();

			string headerText;
			if (Phoenix.Application.UserManager.Count () < 1) {
				headerText = "Setup your account";
			} else {
				headerText = "Phoenix Imperator";
			}

			header = AddHeading (headerText);

			statusMessage = new Label {
				HorizontalTextAlignment = TextAlignment.Center,
				Text = "",
				TextColor = Color.Silver
			};

			userIdEntry = new Entry {
				Placeholder = "User ID",
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Keyboard = Keyboard.Numeric,
			};

			userCodeEntry = new Entry {
				Placeholder = "Code",
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			userIdEntry.Focus ();

			loginButton = new Button {
				Text = "Login",
				Font = Font.SystemFontOfSize(NamedSize.Large),
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.Black,
				BackgroundColor = Color.White,
				BorderWidth = 1,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			loginButton.Clicked += LoginButtonClicked;

			linkButton = new Button {
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Text = "Visit Nexus to get your XML Access Id and Code",
				// TextColor = Color.White,
				// BackgroundColor = Color.Black
			};

			linkButton.Clicked += LinkButtonClicked;

			PageLayout.Children.Add (userIdEntry);
			PageLayout.Children.Add (userCodeEntry);
			PageLayout.Children.Add (loginButton);
			PageLayout.Children.Add (linkButton);
			PageLayout.Children.Add (statusMessage);

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

		void LinkButtonClicked(object sender, EventArgs e)
		{
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

			PhoenixPage page = new PhoenixPage("Nexus"){
				Content = layout
			};
			RootPage.Root.NextPageModal(page);
		}

		void LoginButtonClicked(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace (userCodeEntry.Text) || string.IsNullOrWhiteSpace (userIdEntry.Text)) {
				ShowErrorAlert ("User Id and Code are required");
				return;
			}
			Spinner.IsRunning = true;
			userIdEntry.IsEnabled = false;
			userCodeEntry.IsEnabled = false;
			loginButton.IsEnabled = false;
			linkButton.IsVisible = false;
			Phoenix.Application.UserManager.Save (this.UserId, this.UserCode, (user) => {
				Phoenix.Application.UserLoggedIn(user);

				Xamarin.Insights.Identify(UserId.ToString(),new Dictionary<string, string>{
					{"Version",App.Version}
				});

				if(Phoenix.Application.GameStatusManager.Count() < 1){
					// fresh setup
					StackLayout layout = (StackLayout) Content;
					ProgressBar progressBar = new ProgressBar{
						Progress = 0f
					};
					layout.Children.Add(progressBar);
					header.Text = "Setting Up. Please be patient.";
					loginButton.Text = "Setup";
					int totalSetupActions = SetupActions.Length;
					int actionCompleteCount = 0;
					float progressPerAction = 1.0f / (float)totalSetupActions;
					foreach(Action<Action<Exception>> setupAction in SetupActions){
						setupAction((ex) => {
							UpdateProgressBar(progressBar,progressPerAction);
							if(ex != null){
								ShowErrorAlert(ex);
							}
							actionCompleteCount += 1;
							if(actionCompleteCount >= totalSetupActions) {
								ShowHomePage();
							}
						});
					}
				}
				else {
					ShowHomePage();
				}
			});
		}

		private void UpdateProgressBar(ProgressBar progressBar, float progressTo)
		{
			Device.BeginInvokeOnMainThread (() => {
				progressBar.ProgressTo(progressBar.Progress + progressTo,250,Easing.Linear);
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
				Spinner.IsRunning = false;
				userIdEntry.IsEnabled = true;
				userCodeEntry.IsEnabled = true;
				loginButton.IsEnabled = true;
				statusMessage.Text = "";
				header.Text = "Phoenix Imperator";
				RootPage.Root.GoHome();

			});
		}

		private void FetchGameStatus(Action<Exception> callback)
		{
			Phoenix.Application.GameStatusManager.Fetch ((results, ex) => {
				if (ex == null) {
					UpdateStatusMessage ("Fetched game status.");
				}
				callback (ex);
			});
		}

		 
		private void FetchInfo(Action<Exception> callback)
		{
			Phoenix.Application.InfoManager.Fetch ((results, ex) => {
				if (ex == null) {
					UpdateStatusMessage ("Fetched game info.");
				}
				callback (ex);
			});		
		}

		private void FetchStarSystems(Action<Exception> callback)
		{
			Phoenix.Application.StarSystemManager.Fetch ((results, ex) => {
				if (ex == null) {
					UpdateStatusMessage ("Fetched star systems.");
				}
				callback (ex);
			});
		}

		private void FetchOrderTypes(Action<Exception> callback)
		{
			Phoenix.Application.OrderTypeManager.Fetch ((results, ex) => {
				if (ex == null) {
					UpdateStatusMessage ("Fetched order types.");
				}
				callback (ex);
			});
		}

		private void FetchItems(Action<Exception> callback)
		{
			Phoenix.Application.ItemManager.Fetch ((results, ex) => {
				if (ex == null) {
					UpdateStatusMessage ("Fetched items.");
				}
				callback (ex);
			});
		}

		private void FetchPositions(Action<Exception> callback)
		{
			Phoenix.Application.PositionManager.Fetch ((results, ex) => {
				if (ex == null) {
					UpdateStatusMessage ("Fetched positions.");
				}
				callback (ex);
			});
		}

		private void FetchNotifications(Action<Exception> callback)
		{
			Phoenix.Application.NotificationManager.Fetch ((results, ex) => {
				if (ex == null) {
					UpdateStatusMessage ("Fetched notifications.");
				}
				callback (ex);
			});
		}

		private void FetchMarkets(Action<Exception> callback)
		{
			Phoenix.Application.MarketManager.Fetch ((results, ex) => {
				if (ex == null) {
					UpdateStatusMessage ("Fetched markets.");
				}
				callback (ex);
			});
		}

		private Action<Action<Exception>>[] SetupActions {
			get {
				return new Action<Action<Exception>>[] {
					FetchGameStatus,
					FetchInfo,
					FetchStarSystems,
					FetchOrderTypes,
					FetchItems,
					FetchPositions,
					FetchNotifications,
					FetchMarkets
				};
			}
		}

		private Label statusMessage;
		private Entry userIdEntry;
		private Entry userCodeEntry;
		private Button loginButton;
		private Button linkButton;
		private Label header;
	}
}


