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
	/// <summary>
	/// Menu page.
	/// </summary>
	public class MenuPage : PhoenixPage
	{
		/// <summary>
		/// Gets or sets the menu.
		/// </summary>
		/// <value>The menu.</value>
		public ListView Menu { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.MenuPage"/> class.
		/// </summary>
		public MenuPage () : base("Menu")
		{
			Icon = "settings.png";

			Menu = new MenuListView ();

			var menuLabel = new ContentView {
				Padding = new Thickness (10, 36, 0, 5),
				Content = new Label {
					TextColor = Color.Black,
					Text = "MENU", 
				},
			};

			PageLayout.Children.Add (menuLabel);
			PageLayout.Children.Add (Menu);
		}

		/// <summary>
		/// Deselects the menu item.
		/// </summary>
		public void DeselectMenuItem()
		{
			Device.BeginInvokeOnMainThread (() => {
				Menu.SelectedItem = null;
			});
		}
	}

	/// <summary>
	/// Menu list view.
	/// </summary>
	public class MenuListView : ListView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.MenuListView"/> class.
		/// </summary>
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

	/// <summary>
	/// Side menu item.
	/// </summary>
	public class SideMenuItem
	{
		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the icon source.
		/// </summary>
		/// <value>The icon source.</value>
		public string IconSource { get; set; }

		/// <summary>
		/// Gets or sets the type of the target.
		/// </summary>
		/// <value>The type of the target.</value>
		public object TargetType { get; set; }
	}

	/// <summary>
	/// Menu list data.
	/// </summary>
	public class MenuListData : List<SideMenuItem>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.MenuListData"/> class.
		/// </summary>
		public MenuListData()
		{
			
			Add (new SideMenuItem {
				Title = "Home",
				IconSource = "home.png",
				TargetType = typeof(HomePage)
			});

			Add (new SideMenuItem {
				Title = "Turns",
				IconSource = "icon_report.png",
				TargetType = "Turns"
			});

			Add (new SideMenuItem {
				Title = "Notifications",
				IconSource = "icon_notifications.png",
				TargetType = "Notifications"
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

