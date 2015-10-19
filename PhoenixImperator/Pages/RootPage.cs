//
// RootPage.cs
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

using Xamarin.Forms;

using Phoenix.Util;

namespace PhoenixImperator.Pages
{
	public class RootPage : MasterDetailPage
	{
		public static RootPage Root { get; set; }

		public RootPage ()
		{
			Root = this;

			menuPage = new MenuPage ();
			Master = menuPage;
			menuPage.Menu.ItemTapped += (sender, e) => NavigateTo (e.Item as SideMenuItem);

			var navigationPage = new NavigationPage (new UserLoginPage ()){
				BarBackgroundColor = Color.Black,
				BarTextColor = Color.White
			};
			Detail = navigationPage;
		}

		public void GoHome()
		{
			GoToPage (new HomePage());
		}

		public void NavigateTo (SideMenuItem menu)
		{
			Log.WriteLine (Log.Layer.UI, GetType (), "Navigate To: " + menu.TargetType);
			Page displayPage = (Page)Activator.CreateInstance (menu.TargetType);
			menuPage.DeselectMenuItem ();
			GoToPage (displayPage);
		}

		public void GoToPage(Page displayPage)
		{
			Log.WriteLine (Log.Layer.UI, GetType (), "Go To Page: " + displayPage);
			var navigationPage = new NavigationPage (displayPage){
				BarBackgroundColor = Color.Black,
				BarTextColor = Color.White
			};
			Detail = navigationPage;

			IsPresented = false;

			menuPage.DeselectMenuItem ();
		}

		public void NextPage(Page nextPage)
		{
			((NavigationPage)Detail).PushAsync (nextPage);
		}

		public void PreviousPage()
		{
			((NavigationPage)Detail).PopAsync ();
		}

		private MenuPage menuPage;
	}
}

