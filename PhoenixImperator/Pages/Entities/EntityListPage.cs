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
	/// <summary>
	/// Entity list.
	/// </summary>
	public static class EntityList
	{
		/// <summary>
		/// Gets or sets the entities.
		/// </summary>
		/// <value>The entities.</value>
		public static ObservableCollection<EntityBase> Entities { get; set; }

		/// <summary>
		/// The delete entity callback.
		/// </summary>
		public static Action<EntityBase, Action<IEnumerable<EntityBase>>> DeleteEntityCallback;

		/// <summary>
		/// Updates the entities.
		/// </summary>
		/// <param name="entities">Entities.</param>
		public static void UpdateEntities(IEnumerable<EntityBase> entities)
		{
			if (Entities == null) {
				Entities = new ObservableCollection<EntityBase> ();
			} else {
				Entities.Clear ();
			}
			foreach (EntityBase e in entities) {
				Entities.Add (e);
			}
		}
	}

	/// <summary>
	/// Entity list page.
	/// </summary>
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

		/// <summary>
		/// Gets the manager.
		/// </summary>
		/// <value>The manager.</value>
		public NexusManager<T> Manager { get; private set;}

		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.Entities.EntityListPage`1"/> class.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="manager">Manager.</param>
		/// <param name="entities">Entities.</param>
		/// <param name="entityHasDetail">If set to <c>true</c> entity has detail.</param>
		/// <param name="pullToRefresh">If set to <c>true</c> pull to refresh.</param>
		/// <param name="swipeLeftToDelete">If set to <c>true</c> swipe left to delete.</param>
		public EntityListPage (string title, NexusManager<T> manager, IEnumerable<T> entities, bool entityHasDetail = true, bool pullToRefresh = true, bool swipeLeftToDelete = true) : base(title)
		{
			Manager = manager;
			EntityList.UpdateEntities (entities);
			EntityList.DeleteEntityCallback = DeleteEntity;

			EntityHasDetail = entityHasDetail;
			PullToRefresh = pullToRefresh;

			Type cellType;
			if (swipeLeftToDelete) {
				cellType = typeof(EntityViewCell);
			} else {
				cellType = typeof(TextCell);
			}
			listView = AddListViewWithSearchBar (cellType, null, (sender, e) => {
				EntitySelected(manager, (T)e.Item);
			});

			listView.IsGroupingEnabled = true;
			listView.GroupDisplayBinding = new Binding ("GroupName");
			listView.GroupShortNameBinding = new Binding ("GroupShortName");

			listView.IsRefreshing = true;
			EntityGroup.GroupEntities<T> (entities, (results) => {
				Device.BeginInvokeOnMainThread (() => {
					listView.ItemsSource = results;
					listView.IsRefreshing = false;
				});
			});


			if (pullToRefresh) {
				listView.IsPullToRefreshEnabled = true;
				listView.RefreshCommand = new Command ((e) => {
					if(!IsSearching) {
						RefreshList();
					}
				});
			}

			if (PullToRefresh) {
				if (swipeLeftToDelete) {
					helpLabel = AddHelpLabel ("Pull down to refresh. Swipe left to delete an entry.");
				} else {
					helpLabel = AddHelpLabel ("Pull down to refresh.");
				}
			} else if (swipeLeftToDelete) {
				helpLabel = AddHelpLabel ("Swipe left to delete an entry.");
			}
		}

		/// <summary>
		/// Refreshs the list.
		/// </summary>
		protected virtual void RefreshList()
		{
			Manager.Fetch((results, ex) => {
				if(ex == null){
					EntityGroup.GroupEntities<T> (results, (groupedResults) => {
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

		/// <summary>
		/// Deletes the entity.
		/// </summary>
		/// <param name="entity">Entity.</param>
		/// <param name="callback">Callback.</param>
		protected virtual void DeleteEntity(EntityBase entity, Action<IEnumerable<EntityBase>> callback)
		{
			listView.IsRefreshing = true;
			Manager.Delete ((T)entity, (results) => {
				callback(results);
				EntityGroup.GroupEntities<T> (results, (groupedResults) => {
					Device.BeginInvokeOnMainThread (() => {
						listView.ItemsSource = groupedResults;
						listView.IsRefreshing = false;
					});
				});
			});
		}

		/// <summary>
		/// Entities the selected.
		/// </summary>
		/// <param name="manager">Manager.</param>
		/// <param name="item">Item.</param>
		protected virtual void EntitySelected(NexusManager<T> manager, T item)
		{
			if(EntityHasDetail){
				EntityPageBuilderFactory.ShowEntityPage<T>(manager,item.Id);
			}
			listView.IsEnabled = true;
		}

		/// <summary>
		/// Filters the list view.
		/// </summary>
		/// <param name="listView">List view.</param>
		/// <param name="filter">Filter.</param>
		/// <param name="source">Source.</param>
		protected override void FilterListView(ListView listView, string filter, IEnumerable<EntityBase> source)
		{
			listView.BeginRefresh ();

			if (string.IsNullOrWhiteSpace (filter)) {
				EntityGroup.GroupEntities<T> (EntityList.Entities, (results) => {
					Device.BeginInvokeOnMainThread(() => {
						listView.ItemsSource = results;
						listView.EndRefresh ();
						IsSearching = false;
					});
				});
			} else {
				Task.Factory.StartNew (() => {
					EntityGroup.GroupEntities<T>(EntityList.Entities.Where (x => x.ToString ().ToLower ().Contains (filter.ToLower ())),(results) => {
						Device.BeginInvokeOnMainThread(() => {
							listView.ItemsSource = results;
							listView.EndRefresh ();
							IsSearching = false;
						});
					});
				});
			}
		}

		protected ListView listView;
		protected Label helpLabel;
	}

	/// <summary>
	/// Entity group.
	/// </summary>
	public class EntityGroup : ObservableCollection<EntityBase>
	{
		/// <summary>
		/// Groups the entities.
		/// </summary>
		/// <param name="entities">Entities.</param>
		/// <param name="callback">Callback.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void GroupEntities<T>(IEnumerable<EntityBase> entities, Action<IEnumerable<EntityGroup>> callback)  where T :   EntityBase, new()
		{
			Task.Factory.StartNew (() => {
				Dictionary<string, EntityGroup> mapping = new Dictionary<string, EntityGroup> ();
				foreach(T item in entities){
					EntityGroup group;
					if (item.Group == null) {
						if (mapping.ContainsKey ("")) {
							group = mapping [""];
						} else {
							group = new EntityGroup ("", "*");
							mapping.Add ("", group);
						}
					}
					else if (mapping.ContainsKey (item.Group)) {
						group = mapping [item.Group];
					} else {
						group = new EntityGroup (item.Group, item.GroupShortName);
						mapping.Add (item.Group, group);
					}
					group.Add (item);
				}
				IEnumerable<EntityGroup> grouped = from element in mapping.Values
					orderby element.GroupName
					select element;
				callback(grouped);
			});
		}

		/// <summary>
		/// Gets the name of the group.
		/// </summary>
		/// <value>The name of the group.</value>
		public string GroupName { get; private set; }

		/// <summary>
		/// Gets the name of the group short.
		/// </summary>
		/// <value>The name of the group short.</value>
		public string GroupShortName { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.Entities.EntityGroup"/> class.
		/// </summary>
		/// <param name="groupName">Group name.</param>
		/// <param name="groupShortName">Group short name.</param>
		public EntityGroup(string groupName, string groupShortName)
		{
			GroupName = groupName;
			GroupShortName = groupShortName;
		}
	}

	/// <summary>
	/// Entity view cell.
	/// </summary>
	public class EntityViewCell : TextCell
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.Entities.EntityViewCell"/> class.
		/// </summary>
		public EntityViewCell()
		{
			var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
			deleteAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			deleteAction.Clicked += OnDelete;

			this.ContextActions.Add (deleteAction);
		}

		/// <summary>
		/// Raises the delete event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void OnDelete (object sender, EventArgs e)
		{
			var item = (MenuItem)sender;
			Log.WriteLine (Log.Layer.UI,GetType(),"OnDelete: " + item.CommandParameter);
			EntityList.DeleteEntityCallback ((EntityBase)item.CommandParameter,(results) => {
				EntityList.UpdateEntities(results);
			});
		}
	}
}


