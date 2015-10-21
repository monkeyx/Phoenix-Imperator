//
// ListPage.cs
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
using System.Threading.Tasks;

using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.Util;

namespace PhoenixImperator.Pages.Entities
{
	public class EntityListPage<T> : PhoenixPage where T :   EntityBase, new()
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PhoenixImperator.Pages.Entities.EntityListPage`1"/> entity
		/// has detail.
		/// </summary>
		/// <value><c>true</c> if entity has detail; otherwise, <c>false</c>.</value>
		public bool EntityHasDetail { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="PhoenixImperator.Pages.Entities.EntityListPage`1"/> pull to refresh.
		/// </summary>
		/// <value><c>true</c> if pull to refresh; otherwise, <c>false</c>.</value>
		public bool PullToRefresh { get; private set;}


		public EntityListPage (string title, NexusManager<T> manager, IEnumerable<T> entities, bool entityHasDetail = true, bool pullToRefresh = true)
		{
			Title = title;

			Padding = new Thickness (10, Device.OnPlatform (20, 0, 0), 10, 5);

			BackgroundColor = Color.Black;

			EntityHasDetail = entityHasDetail;
			PullToRefresh = pullToRefresh;

			allEntities = entities;

			// List View
			listView = new ListView {
				IsGroupingEnabled = true,
				GroupDisplayBinding = new Binding ("GroupName"),
				GroupShortNameBinding = new Binding ("GroupShortName")
			};
			listView.ItemTemplate = new DataTemplate (typeof(TextCell));
			listView.ItemTemplate.SetBinding (TextCell.TextProperty, "ListText");
			listView.ItemTemplate.SetBinding (TextCell.DetailProperty, "ListDetail");
			listView.IsRefreshing = true;
			GroupEntities (entities, (results) => {
				Device.BeginInvokeOnMainThread (() => {
					listView.ItemsSource = results;
					listView.IsRefreshing = false;
				});
			});


			if (pullToRefresh) {
				listView.IsPullToRefreshEnabled = true;
				listView.RefreshCommand = new Command ((e) => {
					if(!isSearching) {
						manager.Fetch((results, ex) => {
							if(ex == null){
								GroupEntities (results, (groupedResults) => {
									Device.BeginInvokeOnMainThread (() => {
										listView.ItemsSource = groupedResults;
										listView.IsRefreshing = false;
									});
								});
							}
							else {
								ShowErrorAlert(ex);
							}
						},true);
					}
				});
			}
			listView.ItemTapped += (sender, e) => {
				Log.WriteLine (Log.Layer.UI, this.GetType (), "Tapped: " + e.Item + "(" + e.Item.GetType() + ")");
				listView.IsEnabled = false;
				((ListView)sender).SelectedItem = null; // de-select the row
				EntitySelected(manager, (T)e.Item);
			};

			// Search bar
			isSearching = false;

			searchBar = new SearchBar () {
				Placeholder = "Search"
			};

			searchBar.TextChanged += (sender, e) => FilterList (searchBar.Text);
			searchBar.SearchButtonPressed += (sender, e) => {
				FilterList (searchBar.Text);
			};

			// Activity
			activityIndicator = new ActivityIndicator {
				IsEnabled = true,
				IsRunning = false,
				BindingContext = this
			};
			layout =  new StackLayout { 
				Children = {
					searchBar,
					activityIndicator,
					listView
				}
			};

			Content = layout;
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			if (PullToRefresh) {
				Onboarding.ShowOnboarding ((int)UserFlags.SHOWN_ONBOARDING_ENTITY_LIST_PULL_TO_REFRESH, "Help", "Pull down to refresh");
			}
		}

		protected virtual void EntitySelected(NexusManager<T> manager, T item)
		{
			if(EntityHasDetail){
				EntityPageBuilderFactory.ShowEntityPage<T>(manager,item.Id);
			}
			listView.IsEnabled = true;
		}

		protected void GroupEntities(IEnumerable<T> entities, Action<IEnumerable<EntityGroup<T>>> callback)
		{
			Task.Factory.StartNew (() => {
				Dictionary<string, EntityGroup<T>> mapping = new Dictionary<string, EntityGroup<T>> ();
				foreach(T item in entities){
					EntityGroup<T> group;
					if (item.Group == null) {
						if (mapping.ContainsKey ("")) {
							group = mapping [""];
						} else {
							group = new EntityGroup<T> ("", "*");
							mapping.Add ("", group);
						}
					}
					else if (mapping.ContainsKey (item.Group)) {
						group = mapping [item.Group];
					} else {
						group = new EntityGroup<T> (item.Group, item.GroupShortName);
						mapping.Add (item.Group, group);
					}
					group.Add (item);
				}
				IEnumerable<EntityGroup<T>> grouped = from element in mapping.Values
					orderby element.GroupName
					select element;
				callback(grouped);
			});
		}

		protected StackLayout layout;
		protected ListView listView;

		private void FilterList(string filter)
		{
			isSearching = true;
			listView.BeginRefresh ();

			if (string.IsNullOrWhiteSpace (filter)) {
				GroupEntities (allEntities, (results) => {
					Device.BeginInvokeOnMainThread(() => {
						listView.ItemsSource = results;
						listView.EndRefresh ();
						isSearching = false;
					});
				});
			} else {
				Task.Factory.StartNew (() => {
					GroupEntities(allEntities.Where (x => x.ToString ().ToLower ().Contains (filter.ToLower ())),(results) => {
						Device.BeginInvokeOnMainThread(() => {
							listView.ItemsSource = results;
							listView.EndRefresh ();
							isSearching = false;
						});
					});
				});
			}
		}

		private IEnumerable<T> allEntities;
		private ActivityIndicator activityIndicator;
		private SearchBar searchBar;
		private bool isSearching;

	}

	public class EntityGroup<T> : ObservableCollection<T>  where T :   EntityBase, new()
	{
		public string GroupName { get; private set; }

		public string GroupShortName { get; private set; }

		public EntityGroup(string groupName, string groupShortName)
		{
			GroupName = groupName;
			GroupShortName = groupShortName;
		}
	}
}


