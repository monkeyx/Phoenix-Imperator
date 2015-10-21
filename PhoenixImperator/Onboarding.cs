//
// Onboarding.cs
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
using System.Threading.Tasks;

using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.Util;

using PhoenixImperator.Pages;

namespace PhoenixImperator
{
	public static class Onboarding
	{
		public static void ShowOnboarding(int flag, string title, string content)
		{
			if (Phoenix.Application.User != null && !Phoenix.Application.User.HasPreference (flag)) {
				Phoenix.Application.User.SetPreference (flag);
				Phoenix.Application.UserManager.Save (Phoenix.Application.User,(user) => {
					Log.WriteLine(Log.Layer.UI, typeof(Onboarding), "User preference saved");
				});
				// WaitAndDisplayPopup (title, content);
				Device.BeginInvokeOnMainThread (() => {
					RootPage.Root.DisplayAlert (title, content, "OK");
				});
			}
		}

		private static async void WaitAndDisplayPopup(string title, string content)
		{
			await Task.Delay(TimeSpan.FromSeconds(1));

			Device.BeginInvokeOnMainThread (() => {
				RootPage.Root.DisplayAlert (title, content, "OK");
			});
		}

		static Onboarding ()
		{
		}
	}
}

