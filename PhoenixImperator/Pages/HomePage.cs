//
// HomePage.cs
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

using Phoenix.BL.Entities;
using Phoenix.Util;

namespace PhoenixImperator
{
	public class HomePage : ContentPage
	{
		public HomePage ()
		{
			Title = "Home";

			starDateLabel = new Label ();
			statusMessageLabel = new Label ();

			StackLayout header = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Fill,
				Children = {
					statusMessageLabel,
					starDateLabel
				}
			};

			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			Content = new StackLayout { 
				Children = {
					header
				}
			};

			// get status from local DB first
			Phoenix.Application.GameStatusManager.First ((status) => {
				SetStatus(status);

				// now get status from Nexus and update the UI accordingly
				Phoenix.Application.GameStatusManager.Fetch ((results, statusCode) => {
					Log.WriteLine(Log.Layer.UI, this.GetType(), "GameStatus: Count: " + results.Count, " Status: " + statusCode);
					if(results.Count > 0){
						SetStatus(results[0]);
					}
				}, true); // clear all previous status messages
			});
		}

		private void SetStatus(GameStatus status)
		{
			Log.WriteLine(Log.Layer.UI, this.GetType(), "GameStatus: " + status);
			if (status != null) {
				starDateLabel.Text = status.StarDate;
				statusMessageLabel.Text = status.StatusMessage;
			}
		}

		private Label starDateLabel;
		private Label statusMessageLabel;
	}
}


