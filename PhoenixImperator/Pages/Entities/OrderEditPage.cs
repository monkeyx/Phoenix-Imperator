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
using System.Threading.Tasks;

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

			formTable = new TableView {
				Root = new TableRoot(),
				Intent = TableIntent.Form,
				BackgroundColor = Color.White
			};

			formTable.Root.Add (new TableSection ());

			Button deleteButton = new Button {
				Text = "Delete Order",
				TextColor = Color.White,
				BackgroundColor = Color.Red
			};
			deleteButton.Clicked += async(sender, e) => {
				bool confirm = await DisplayAlert("Delete Order","Are you sure?","Yes","No");
				if(confirm){
					deleteButton.IsEnabled = false;
					DeleteOrder();
				}
					
			};

			ShowParameters ();

			AddSection ("Description");
			LastSection.Add (new ViewCell () {
				View = new Label {
					VerticalOptions = LayoutOptions.StartAndExpand,
					Text = CurrentOrder.OrderType.Description,
					BackgroundColor = Color.Silver
				}
			});

			LastSection.Add (new ViewCell { View = deleteButton });

			Content = new ScrollView {
				Content = new StackLayout{
							VerticalOptions = LayoutOptions.Start,
							Padding = new Thickness(10,20),
							Children = {
								formTable
							}
						}
			};
		}

		private void SaveOrder()
		{
			Phoenix.Application.OrderManager.SaveOrder(CurrentOrder,(results) => {
				PositionPageBuilder.UpdateOrders(results);
			});
		}

		private void DeleteOrder()
		{
			Phoenix.Application.OrderManager.DeleteOrder(CurrentOrder,(results) => {
				PositionPageBuilder.UpdateOrders(results);
				RootPage.Root.PreviousPage();
			});
		}

		private void ShowParameters ()
		{
			if (CurrentOrder.OrderType.Parameters.Count < 1) {
				formTable.Root [0].Add (new TextCell{
					Text = "No parameters"
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

		private TableSection AddSection()
		{
			TableSection section = new TableSection ();
			formTable.Root.Add (section);
			return section;
		}

		private TableSection AddSection(string title)
		{
			TableSection section = new TableSection (title);
			formTable.Root.Add (section);
			return section;
		}

		private TableSection LastSection{
			get {
				return formTable.Root [(formTable.Root.Count - 1)];
			}
		}

		private void AddParameter(OrderParameterType paramType, OrderParameter param = null)
		{
			if (param == null) {
				param = new OrderParameter ();
				CurrentOrder.Parameters.Add (param);
			}
			switch (paramType.DataType) {
			case OrderType.DataTypes.Integer:
				AddIntegerParam (paramType, param);
				break;
			case OrderType.DataTypes.String:
				AddStringParam (paramType, param);
				break;
			case OrderType.DataTypes.Boolean:
				AddBooleanParam (paramType, param);
				break;
			default:
				AddInfoParam (paramType, param);
				break;
			}
		}

		private void AddIntegerParam(OrderParameterType paramType, OrderParameter param)
		{
			EntryCell entry = new EntryCell {
				Label = paramType.Name,
				Text = param.Value,
				Keyboard = Keyboard.Numeric
			};
			entry.Completed += (sender, e) => {
				param.Value = entry.Text;
				SaveOrder();
			};
			LastSection.Add (entry);
		}

		private void AddStringParam(OrderParameterType paramType, OrderParameter param)
		{
			EntryCell entry = new EntryCell {
				Label = paramType.Name,
				Text = param.Value
			};
			entry.Completed += (sender, e) => {
				param.Value = entry.Text;
				SaveOrder();
			};
			LastSection.Add (entry);
		}

		private void AddBooleanParam(OrderParameterType paramType, OrderParameter param)
		{
			SwitchCell switchCell = new SwitchCell {
				Text = paramType.Name,
				On = param.Value == "True"
			};

			switchCell.OnChanged += (sender, e) => {
				param.Value = switchCell.On ? "True" : "False";
				SaveOrder();
			};
			LastSection.Add (switchCell);
		}

		private void AddInfoParam(OrderParameterType paramType, OrderParameter param)
		{
			if (paramType.InfoType == 0) {
				AddIntegerParam (paramType, param);
				return;
			}
			TableSection section = AddSection (paramType.Name);

			Picker infoPicker = new Picker {
				Title = param.DisplayValue,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
			};

			EntryCell entry = new EntryCell {
				Text = param.Value,
				Keyboard = Keyboard.Numeric,
				Label = paramType.Name
			};
			entry.Completed += (sender, e) => {
				param.Value = entry.Text;
				if (infoData.ContainsKey (paramType.InfoType)) {
					Dictionary<string,InfoData> data = infoData [paramType.InfoType];
					int i = 0;
					foreach(string value in data.Keys){
						if(entry.Text == data[value].NexusId.ToString()){
							if(infoPicker.Items.Count > i){
								infoPicker.SelectedIndex = i;
								infoPicker.Title = data[value].ToString();
							}
							break;
						}
						i += 1;
					}
				}
				SaveOrder();
			};
			section.Add (entry);

			if (infoData.ContainsKey (paramType.InfoType)) {
				Dictionary<string,InfoData> data = infoData [paramType.InfoType];
				if (data.Count > 0) {
					section.Add (new ViewCell { View = infoPicker });
					int i = 0;
					foreach (string value in data.Keys) {
						infoPicker.Items.Add (value.ToString());
						if (param.Value != null && param.Value == data[value].ToString()) {
							infoPicker.SelectedIndex = i;
						}
						i += 1;
					}
				}

			} else {
				Phoenix.Application.InfoManager.GetInfoDataByGroupId (paramType.InfoType, (results) => {
					Dictionary<string,InfoData> data = new Dictionary<string,InfoData>();
					int i = 0;
					foreach(InfoData info in results){
						if(!data.ContainsKey(info.ToString())){
							data.Add(info.ToString(),info);
							infoPicker.Items.Add(info.ToString());
							if (param.Value != null && param.Value == info.NexusId.ToString()) {
								infoPicker.SelectedIndex = i;
							}
							i += 1;
						}
					}
					if(data.Count > 0){
						section.Add (new ViewCell { View = infoPicker });
					}
					infoData.Add(paramType.InfoType,data);
				});
			}
			infoPicker.Unfocused += (sender, e) => {
				if (infoData.ContainsKey (paramType.InfoType)) {
					Dictionary<string,InfoData> data = infoData [paramType.InfoType];
					string value = infoPicker.Items[infoPicker.SelectedIndex];
					if(data.ContainsKey(value)){
						param.Value = data[value].NexusId.ToString();
						param.DisplayValue = data[value].ToString();
						entry.Text = data[value].NexusId.ToString();
						SaveOrder();
					}
				}
			};
			AddSection ();
		}

		private TableView formTable;
		private Dictionary<int, Dictionary<string,InfoData>> infoData = new Dictionary<int, Dictionary<string,InfoData>>();
	}
}


