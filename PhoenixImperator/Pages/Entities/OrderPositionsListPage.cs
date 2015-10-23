//
// OrderPositionsListPage.cs
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
	public class OrderPositionsListPage : EntityListPage<Position>
	{
		public OrderPositionsListPage (IEnumerable<Position> positions) : base("Orders",Phoenix.Application.PositionManager,positions,true,false)
		{
			Title = "Pending Orders";
			positionsWithOrders = new List<Position> (positions);
			if (positionsWithOrders.Count > 0) {
				submitButton = new Button {
					Text = "Submit Orders",
					TextColor = Color.White,
					BackgroundColor = Color.Green
				};
				submitButton.Clicked += (sender, e) => {
					submitButton.IsEnabled = false;
					SubmitOrders();
				};
				layout.Children.Add (submitButton);
			}
		}

		protected override void EntitySelected(NexusManager<Position> manager, Position item)
		{
			EntityPageBuilderFactory.ShowEntityPage<Position>(manager,item.Id,1);
		}

		protected override void DeleteEntity(EntityBase entity, Action<IEnumerable<EntityBase>> callback)
		{
			listView.IsRefreshing = true;
			Phoenix.Application.OrderManager.DeleteLocalOrders (entity.Id, (response) => {
				Phoenix.Application.PositionManager.GetPositionsWithOrders((results) => {
					callback(results);
					GroupEntities (results, (groupedResults) => {
						Device.BeginInvokeOnMainThread (() => {
							listView.ItemsSource = groupedResults;
							listView.IsRefreshing = false;
						});
					});
				});
			});
		}

		private void SubmitOrders()
		{
			Insights.Track ("Submit Orders");
			Log.WriteLine (Log.Layer.UI, GetType (), "Submitting Orders for " + positionsWithOrders.Count);
			if (positionsWithOrders.Count < 1) {
				return;
			}
			ProgressBar progressBar = new ProgressBar{
				Progress = 0f
			};

			layout.Children.Add (progressBar);

			float progressPerPosition = 1.0f / (float)positionsWithOrders.Count;

			int totalPositions = positionsWithOrders.Count;
			int positionsSent = 0;

			Device.BeginInvokeOnMainThread(() => {
				listView.IsRefreshing = true;
			});

			foreach (Position position in positionsWithOrders) {
				Phoenix.Application.OrderManager.SubmitOrdersForPosition (position.Id, (count, e) => {
					if(e == null){
						Log.WriteLine(Log.Layer.UI,GetType(),"Successfuly uploaded " + count + " orders for " + position );
					}
					else {
						ShowErrorAlert(e);
						Device.BeginInvokeOnMainThread(() => {
							submitButton.IsEnabled = true;
						});
						return;
					}
					UpdateProgressBar(progressBar, (float) (progressBar.Progress + progressPerPosition));
					positionsSent += 1;
					positionsWithOrders.Remove(position);
					GroupEntities(positionsWithOrders,(results) => {
						Device.BeginInvokeOnMainThread(() => {
							listView.ItemsSource = results;
						});
					});
					if(positionsSent >= totalPositions){
						ShowInfoAlert("Orders Submitted", "Submitted orders for " + positionsSent + " positions");
						Device.BeginInvokeOnMainThread(() => {
							listView.IsRefreshing = false;
							submitButton.IsEnabled = true;
						});
					}
				});
			}
		}

		private void UpdateProgressBar(ProgressBar progressBar, float progressTo)
		{
			Device.BeginInvokeOnMainThread (() => {
				progressBar.ProgressTo(progressTo,250,Easing.Linear);
			});
		}

		private List<Position> positionsWithOrders;
		private Button submitButton;
	}
}


