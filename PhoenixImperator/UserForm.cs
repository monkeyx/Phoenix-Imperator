using System;

using Xamarin.Forms;

namespace PhoenixImperator
{
	public class UserForm : ContentPage
	{
		public int UserId {
			get {
				return Int32.Parse (userId.Text);
			}
			set {
				userId.Text = value.ToString();
			}
		}

		public string UserCode {
			get {
				return userCode.Text;
			}
			set {
				userCode.Text = value;
			}
		}

		public UserForm ()
		{
			Title = "Login";

			Label header = new Label { 
				XAlign = TextAlignment.Center,
				Text = "Setup Your User Account" 
			};

			userId = new Entry {
				Placeholder = "User ID",
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Keyboard = Keyboard.Numeric
			};

			userCode = new Entry {
				Placeholder = "Code",
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			Button loginButton = new Button {
				Text = "Login",
				BorderWidth = 1,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			loginButton.Clicked += LoginButtonClicked;

			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			Content = new StackLayout { 
				VerticalOptions = LayoutOptions.Center,
				Children = {
					header,
					new StackLayout {
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.Center,
						Children = {
							userId,
							userCode
						}
					},
					loginButton
				}
			};
		}

		void LoginButtonClicked(object sender, EventArgs e)
		{
			App.UserManager.Save (this.UserId, this.UserCode, (user) => {
				DisplayAlert ("Login", "Logged in " + user, "OK");
			});
		}

		private Entry userId;
		private Entry userCode;
	}
}


