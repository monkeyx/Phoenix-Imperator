//
// OrderSelectorPage.cs
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

namespace PhoenixImperator.Pages
{
	/// <summary>
	/// Order selector page.
	/// </summary>
	public class OrderSelectorPage : PhoenixPage
	{
		/// <summary>
		/// Gets or sets the orders.
		/// </summary>
		/// <value>The orders.</value>
		public static ObservableCollection<OrderType> Orders { get; set; } = new ObservableCollection<OrderType> ();

		/// <summary>
		/// Updates the orders.
		/// </summary>
		/// <param name="orders">Orders.</param>
		public static void UpdateOrders(IEnumerable<OrderType> orders)
		{
			Orders.Clear ();
			foreach (OrderType o in orders) {
				Orders.Add (o);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.OrderSelectorPage"/> class.
		/// </summary>
		/// <param name="position">Position.</param>
		public OrderSelectorPage (Position position) : base("New Order")
		{
			BackgroundColor = Color.Black;

			AddListViewWithSearchBar (typeof(TextCell), Orders, (sender, e) => {
				Phoenix.Application.OrderTypeManager.Get(((OrderType)e.Item).Id,(orderType) => {
					Order order = new Order{
						PositionId = position.Id,
						OrderType = orderType,
						OrderTypeId = orderType.Id
					};
					OrderEditPage editPage = new OrderEditPage(order);
					Device.BeginInvokeOnMainThread(() => {
						RootPage.Root.NextPageAfterModal(editPage);
					});
				});
			});

			Phoenix.Application.OrderTypeManager.GetOrderTypesForPosition ((Position.PositionFlag)position.PositionType, (results) => {
				UpdateOrders(results);
			});
		}
	}
}


