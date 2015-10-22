﻿//
// BasePhoenixPage.cs
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

namespace PhoenixImperator
{
	public class PhoenixPage : ContentPage
	{
		public PhoenixPage ()
		{
			BackgroundColor = Color.White;
		}

		public void ShowInfoAlert(string title, object info)
		{
			if (title == null || info == null) {
				return;
			}
			Device.BeginInvokeOnMainThread(() => {
				DisplayAlert(title, info.ToString(),"OK");
			});
		}

		public void ShowErrorAlert(object error)
		{
			if (error == null) {
				return;
			}
			#if DEBUG
			string errorMessage = error.ToString ();
			#else
			string errorMessage = "There was a problem connecting to Nexus";
			#endif

			Device.BeginInvokeOnMainThread(() => {
				DisplayAlert("Problem", errorMessage,"OK");
			});
		}
	}
}

