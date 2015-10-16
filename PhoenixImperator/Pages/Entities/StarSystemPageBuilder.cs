//
// StarSystemPage.cs
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
using System.Linq;

using Xamarin.Forms;

using XLabs.Forms.Controls;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.Util;

namespace PhoenixImperator.Pages.Entities
{
	public class StarSystemPageBuilder : BaseEntityPageBuilder<StarSystem>
	{
		protected override void DisplayEntity(StarSystem item)
		{
			if (item.CelestialBodies.Count > 0) {
				AddContentTab ("Celestial Bodies","icon_celestialbodies.png");

				ListView listView = new ListView ();
				listView.ItemTemplate = new DataTemplate (typeof(TextCell));
				listView.ItemTemplate.SetBinding (TextCell.TextProperty, "ListText");
				listView.ItemTemplate.SetBinding (TextCell.DetailProperty, "ListDetail");
				listView.ItemsSource = item.CelestialBodies;

				listView.ItemTapped += (sender, e) => {
					Log.WriteLine(Log.Layer.UI, this.GetType(), "Tapped: " + e.Item);
					((ListView)sender).SelectedItem = null; // de-select the row
				};
				currentLayout.Children.Add (listView);
			}

			if (item.JumpLinks.Count > 0) {
				AddContentTab ("Jump Links","icon_jumplink.png");

				ListView listView = new ListView ();
				listView.ItemTemplate = new DataTemplate (typeof(TextCell));
				listView.ItemTemplate.SetBinding (TextCell.TextProperty, "ListText");
				listView.ItemTemplate.SetBinding (TextCell.DetailProperty, "ListDetail");
				listView.ItemsSource = item.JumpLinks;

				listView.ItemTapped += (sender, e) => {
					Log.WriteLine (Log.Layer.UI, this.GetType (), "Tapped: " + e.Item);
					((ListView)sender).SelectedItem = null; // de-select the row
					StarSystem ss = ((JumpLink)e.Item).ToStarSysytem;
					EntityPageBuilderFactory.ShowEntityPage<StarSystem> (Manager, ss.Id);
				};

				currentLayout.Children.Add (listView);
			}

			Phoenix.Application.PositionManager.GetPositionsInStarSystem (item, (results) => {
				allPositions = results;
				if(results.GetEnumerator().MoveNext()){
					Device.BeginInvokeOnMainThread (() => {
						AddContentTab ("Positions","icon_positions.png");
						SearchBar searchBar = new SearchBar () {
							Placeholder = "Search"
						};
						searchBar.TextChanged += (sender, e) => FilterList (searchBar.Text);
						searchBar.SearchButtonPressed += (sender, e) => {
							FilterList (searchBar.Text);
						};
						positionListView = new ListView ();
						positionListView.ItemTemplate = new DataTemplate (typeof(TextCell));
						positionListView.ItemTemplate.SetBinding (TextCell.TextProperty, "ListText");
						positionListView.ItemTemplate.SetBinding (TextCell.DetailProperty, "ListDetail");
						positionListView.ItemsSource = results;
						positionListView.ItemTapped += (sender, e) => {
							Log.WriteLine (Log.Layer.UI, this.GetType (), "Tapped: " + e.Item);
							((ListView)sender).SelectedItem = null; // de-select the row
							Position p = (Position)e.Item;
							EntityPageBuilderFactory.ShowEntityPage<Position> (Phoenix.Application.PositionManager, p.Id);
						};
						currentLayout.Children.Add (searchBar);
						currentLayout.Children.Add (positionListView);
					});
				}
			});
		}

		private void FilterList(string filter)
		{
			positionListView.BeginRefresh ();

			if (string.IsNullOrWhiteSpace (filter)) {
				positionListView.ItemsSource = allPositions;
			} else {
				positionListView.ItemsSource = allPositions.Where (x => x.ToString ().ToLower ().Contains (filter.ToLower ()));
			}

			positionListView.EndRefresh ();
		}

		private IEnumerable<Position> allPositions;
		private ListView positionListView;
	}
}


