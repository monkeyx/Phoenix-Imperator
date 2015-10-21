//
// MenuPage.cs
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

namespace PhoenixImperator.Pages
{
	public class MenuPage : PhoenixPage
	{
		public ListView Menu { get; set; }
		public ActivityIndicator Spinner { get; set; }

		public MenuPage ()
		{
			Icon = "settings.png";
			Title = "menu"; // The Title property must be set.

			Menu = new MenuListView ();

			var menuLabel = new ContentView {
				Padding = new Thickness (10, 36, 0, 5),
				Content = new Label {
					TextColor = Color.FromHex ("AAAAAA"),
					Text = "MENU", 
				},
			};

			Spinner = new ActivityIndicator {
				IsEnabled = true,
				IsRunning = false,
				BindingContext = this
			};

			var layout = new StackLayout { 
				Spacing = 0, 
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			layout.Children.Add (menuLabel);
			layout.Children.Add (Menu);
			layout.Children.Add (Spinner);

			Content = layout;
		}

		public void DeselectMenuItem()
		{
			Device.BeginInvokeOnMainThread (() => {
				Menu.SelectedItem = null;
			});
		}
	}

	public class MenuListView : ListView
	{
		public MenuListView()
		{
			List<SideMenuItem> data = new MenuListData ();

			ItemsSource = data;
			VerticalOptions = LayoutOptions.FillAndExpand;
			BackgroundColor = Color.White;

			var cell = new DataTemplate (typeof(ImageCell));
			cell.SetBinding (TextCell.TextProperty, "Title");
			cell.SetBinding (ImageCell.ImageSourceProperty, "IconSource");

			ItemTemplate = cell;
		}
	}

	public class SideMenuItem
	{
		public string Title { get; set; }

		public string IconSource { get; set; }

		public object TargetType { get; set; }
	}

	public class MenuListData : List<SideMenuItem>
	{
		public MenuListData()
		{
			Add (new SideMenuItem {
				Title = "Home",
				IconSource = "home.png",
				TargetType = typeof(HomePage)
			});

			Add (new SideMenuItem {
				Title = "Positions",
				IconSource = "icon_positions.png",
				TargetType = "Positions"
			});

			Add (new SideMenuItem {
				Title = "Orders",
				IconSource = "icon_orders.png",
				TargetType = "Orders"
			});

			Add (new SideMenuItem {
				Title = "Items",
				IconSource = "icon_production.png",
				TargetType = "Items"
			});

			Add (new SideMenuItem {
				Title = "Star Systems",
				IconSource = "icon_celestialbodies.png",
				TargetType = "Star Systems"
			});

			Add (new SideMenuItem {
				Title = "Order Types",
				IconSource = "icon_general.png",
				TargetType = "Order Types"
			});

			Add (new SideMenuItem {
				Title = "Info",
				IconSource = "icon_techmanual.png",
				TargetType = "Info"
			});

			Add (new SideMenuItem {
				Title = "About",
				IconSource = "settings.png",
				TargetType = "About"
			});
		}
	}
}

