//
// PositionSelectorPage.cs
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
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.Util;

namespace PhoenixImperator.Pages.Entities
{
	/// <summary>
	/// Position selector page.
	/// </summary>
	public class PositionSelectorPage : PhoenixPage
	{
		/// <summary>
		/// Gets or sets the positions.
		/// </summary>
		/// <value>The positions.</value>
		public static ObservableCollection<Position> Positions { get; set; } = new ObservableCollection<Position> ();

		/// <summary>
		/// Updates the positions.
		/// </summary>
		/// <param name="positions">Positions.</param>
		public static void UpdatePositions(IEnumerable<Position> positions)
		{
			Positions.Clear ();
			foreach (Position p in positions) {
				Positions.Add (p);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.Entities.PositionSelectorPage"/> class.
		/// </summary>
		/// <param name="positionType">Position type.</param>
		/// <param name="selectionAction">Selection action.</param>
		public PositionSelectorPage (Position.PositionFlag positionType, Action<Position> selectionAction) : base("Choose Position")
		{
			BackgroundColor = Color.Black;

			ListView listView = AddListViewWithSearchBar (typeof(TextCell), Positions, (sender, e) => {
				Phoenix.Application.PositionManager.Get(((Position)e.Item).Id,(position) => {
					RootPage.Root.DismissModal();
					selectionAction(position);
				});
			});
			listView.IsRefreshing = true;

			Button cancelButton = new Button {
				Text = "Cancel",
				TextColor = Color.White
			};

			cancelButton.Clicked += (sender, e) => {
				Device.BeginInvokeOnMainThread(() => {
					RootPage.Root.DismissModal();
				});
			};

			PageLayout.Children.Add (cancelButton);

			Phoenix.Application.PositionManager.GetPositionsOfType (positionType, (results) => {
				UpdatePositions(results);
				listView.IsRefreshing = false;
			});
		}
	}
}

