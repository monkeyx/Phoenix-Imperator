//
// OrderEditPage.cs
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
using Phoenix.BL.Managers;

using PhoenixImperator.Pages.Entities;

namespace PhoenixImperator.Pages
{
	public class OrderEditPage : PhoenixPage
	{
		public Order CurrentOrder { get; set; }

		public OrderEditPage (Order order)
		{
			Title = order.OrderType.Name;
			CurrentOrder = order;

			layout = new StackLayout{
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			ShowParameters ();

			Button saveButton = new Button {
				Text = "Save",
				BorderWidth = 1,
				TextColor = Color.White,
				BackgroundColor = Color.Blue
			};
			saveButton.Clicked += (sender, e) => {
				saveButton.IsEnabled = false;
				Phoenix.Application.OrderManager.SaveOrder(order,(results) => {
					PositionPageBuilder.UpdateOrders(results);
					RootPage.Root.PreviousPage();
				});
			};
			layout.Children.Add (saveButton);

			Content = layout;
		}

		private void ShowParameters ()
		{
			if (CurrentOrder.OrderType.Parameters.Count < 1) {
				layout.Children.Add (new Label {
					Text = "No parameters required",
					FontAttributes = FontAttributes.Italic,
					HorizontalOptions = LayoutOptions.CenterAndExpand
				});
				return;
			}
			int i = 0;
			foreach (OrderParameterType pt in CurrentOrder.OrderType.Parameters) {
				OrderParameter param = null;
				if (i < CurrentOrder.Parameters.Count) {
					param = CurrentOrder.Parameters [i];
				}
				AddParameter (pt, param);
				i += 1;
			}
		}

		private void AddParameter(OrderParameterType paramType, OrderParameter param = null)
		{
			layout.Children.Add (new Label {
				Text = paramType.Name,
				FontAttributes = FontAttributes.Bold
			});
			if (param == null) {
				param = new OrderParameter ();
				CurrentOrder.Parameters.Add (param);
			}
			switch (paramType.DataType) {
			case OrderType.DataTypes.Integer:
				AddIntegerParam (param);
				break;
			case OrderType.DataTypes.String:
				AddStringParam (param);
				break;
			case OrderType.DataTypes.Boolean:
				AddBooleanParam (param);
				break;
			default:
				AddInfoParam (paramType.InfoType, param);
				break;
			}
		}

		private void AddIntegerParam(OrderParameter param)
		{
			Entry entry = new Entry {
				Text = param.Value,
				Keyboard = Keyboard.Numeric
			};
			entry.TextChanged += (sender, e) => {
				param.Value = entry.Text;
			};
			layout.Children.Add (entry);
		}

		private void AddStringParam(OrderParameter param)
		{
			Entry entry = new Entry {
				Text = param.Value
			};
			entry.TextChanged += (sender, e) => {
				param.Value = entry.Text;
			};
			layout.Children.Add (entry);
		}

		private void AddBooleanParam(OrderParameter param)
		{
			Switch switcher = new Switch {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				IsToggled = param.Value == "True"
			};
			switcher.Toggled += (sender, e) => {
				param.Value = switcher.IsToggled ? "True" : "False";
			};
			layout.Children.Add (switcher);
		}

		private void AddInfoParam(int infoType, OrderParameter param)
		{
			if (infoType == 0) {
				AddIntegerParam (param);
				return;
			}
			Picker infoPicker = new Picker {
				Title = param.Value,
				HorizontalOptions = LayoutOptions.Fill
			};
			if (infoData.ContainsKey (infoType)) {
				Dictionary<string,int> data = infoData [infoType];
				int i = 0;
				foreach (string value in data.Keys) {
					infoPicker.Items.Add (value);
					if (param.Value != null && param.Value == data[value].ToString()) {
						infoPicker.SelectedIndex = i;
					}
					i += 1;
				}
			} else {
				Phoenix.Application.InfoManager.GetInfoDataByGroupId (infoType, (results) => {
					Dictionary<string,int> data = new Dictionary<string,int>();
					int i = 0;
					foreach(InfoData info in results){
						if(!data.ContainsKey(info.ToString())){
							data.Add(info.ToString(),info.NexusId);
							infoPicker.Items.Add(info.ToString());
							if (param.Value != null && param.Value == info.NexusId.ToString()) {
								infoPicker.SelectedIndex = i;
							}
							i += 1;
						}
					}
				});
			}
			infoPicker.Unfocused += (sender, e) => {
				if (infoData.ContainsKey (infoType)) {
					Dictionary<string,int> data = infoData [infoType];
					string value = infoPicker.Items[infoPicker.SelectedIndex];
					if(data.ContainsKey(value)){
						param.Value = data[value].ToString();
					}
				}
			};
			layout.Children.Add (infoPicker);
		}

		private StackLayout layout;
		private Dictionary<int, Dictionary<string,int>> infoData = new Dictionary<int, Dictionary<string,int>>();
	}
}


