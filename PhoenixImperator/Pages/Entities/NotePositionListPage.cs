//
// NotePositionListPage.cs
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

using Xamarin;
using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;
using Phoenix.Util;

namespace PhoenixImperator.Pages.Entities
{
	/// <summary>
	/// Note position list page.
	/// </summary>
	public class NotePositionListPage : EntityListPage<Position>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.Entities.NotePositionListPage"/> class.
		/// </summary>
		/// <param name="positions">Positions.</param>
		public NotePositionListPage (IEnumerable<Position> positions) : base("Notes",Phoenix.Application.PositionManager,positions,true,false)
		{
		}

		/// <summary>
		/// Raises the disappearing event.
		/// </summary>
		protected override void OnDisappearing ()
		{
			hasDisappeared = true;
		}

		/// <summary>
		/// Raises the appearing event.
		/// </summary>
		protected override void OnAppearing ()
		{
			if (hasDisappeared) {
				Phoenix.Application.PositionManager.GetPositionsWithNotes ((results) => {
					EntityGroup.GroupEntities<Position> (results, (groupedResults) => {
						Device.BeginInvokeOnMainThread (() => {
							listView.ItemsSource = groupedResults;
							listView.IsRefreshing = false;
						});
					});
				});
			}
		}

		/// <summary>
		/// Afters the list.
		/// </summary>
		protected override void AfterList ()
		{
			Button addButton = new Button {
				Text = "Add",
				TextColor = Color.White,
				BackgroundColor = Color.Blue
			};

			addButton.Clicked += (sender, e) => {
				PositionSelectorPage page = new PositionSelectorPage(Position.PositionFlag.None,(position) => {
					EntitySelected(Manager,position);
				});
				RootPage.Root.NextPageModal(page);
			};

			PageLayout.Children.Add (addButton);
		}

		/// <summary>
		/// Entities the selected.
		/// </summary>
		/// <param name="manager">Manager.</param>
		/// <param name="item">Item.</param>
		protected override void EntitySelected(NexusManager<Position> manager, Position item)
		{
			EntityPageBuilderFactory.ShowEntityPage<Position>(manager,item.Id,(int)PositionPageBuilder.PositionTab.Notes);
		}

		/// <summary>
		/// Deletes the entity.
		/// </summary>
		/// <param name="entity">Entity.</param>
		/// <param name="callback">Callback.</param>
		protected override void DeleteEntity(EntityBase entity, Action<IEnumerable<EntityBase>> callback)
		{
			listView.IsRefreshing = true;
			Phoenix.Application.PositionManager.DeleteNote (entity.Id, (results) => {
				EntityGroup.GroupEntities<Position> (results, (groupedResults) => {
					Device.BeginInvokeOnMainThread (() => {
						listView.ItemsSource = groupedResults;
						listView.IsRefreshing = false;
					});
				});
			});
		}

		private bool hasDisappeared = false;
	}
}

