//
// EntityPage.cs
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

using XLabs.Forms.Controls;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;

using PhoenixImperator.Pages;

namespace PhoenixImperator.Pages.Entities
{
	public abstract class BaseEntityPageBuilder<T> : IEntityPageBuilder<T> where T :   EntityBase, new()
	{
		public NexusManager<T> Manager { get; set; }

		public TabbedPage BuildPage(T item)
		{
			if (item == null) {
				return new TabbedPage {
					Title = "Not Found",
					Children = {
						new PhoenixPage {
							Content = new StackLayout{
								VerticalOptions = LayoutOptions.CenterAndExpand,
								Children = {
									new Label{
										Text = "These aren't the droids you are looking for",
										HorizontalOptions = LayoutOptions.CenterAndExpand
									}
								}
							}
						}
					}
				};
			}
			entityPage = new EntityContentPage (item);
			DisplayEntity (item);
			return entityPage;
		}

		protected abstract void DisplayEntity(T item);

		protected EntityContentPage entityPage;
		protected PhoenixPage currentTab;
		protected StackLayout currentLayout;

		protected void ShowInfoAlert(string title, object info)
		{
			Device.BeginInvokeOnMainThread(() => {
				currentTab.ShowInfoAlert(title,info);
			});
		}

		protected void ShowErrorAlert(object error)
		{
			Device.BeginInvokeOnMainThread(() => {
				currentTab.ShowErrorAlert(error);
			});
		}

		protected void AddContentTab(string title, string icon)
		{
			currentLayout = new StackLayout {
				Padding = new Thickness (10,20),
				VerticalOptions = LayoutOptions.StartAndExpand
			};
			currentTab = new PhoenixPage {
				Content = currentLayout,
				Title = title,
				Icon = icon
			};
			entityPage.Children.Add (currentTab);
		}

		protected void AddHeading(string heading)
		{
			if (heading == null || currentLayout == null)
				return;
			currentLayout.Children.Add (
				new ExtendedLabel {
					HorizontalOptions = LayoutOptions.Start,
					FontAttributes = FontAttributes.Bold,
					Text = heading,
					FontSize = 18,
					IsUnderline = true
			});
		}

		protected void AddProperty(string key, string value)
		{
			if (currentLayout == null)
				return;
			currentLayout.Children.Add (
				new StackLayout{
					VerticalOptions = LayoutOptions.Start,
					Orientation = StackOrientation.Horizontal,
					Children = {
						new Label{
							HorizontalOptions = LayoutOptions.Start,
							FontAttributes = FontAttributes.Bold,
							Text = key == null ? "" : key + ":"
						},
						new Label {
							HorizontalOptions = LayoutOptions.EndAndExpand,
							Text = value == null ? "" : value
						}
					}
				}
			);
		}

		protected void AddPropertyDoubleLine(string key, string value)
		{
			if (currentLayout == null)
				return;
			if (key != null) {
				currentLayout.Children.Add (
					new Label {
						HorizontalOptions = LayoutOptions.Start,
						FontAttributes = FontAttributes.Bold,
						Text = key
					});
			}
			if (value != null) {
				currentLayout.Children.Add (
					new Label {
						HorizontalOptions = LayoutOptions.Start,
						Text = value
					}
				);
			}
		}

		protected void AddLabel(string value)
		{
			if (currentLayout == null || value == null)
				return;
			currentLayout.Children.Add (
				new Label {
					HorizontalOptions = LayoutOptions.Start,
					Text = value
				}
			);
		}

		protected void AddEntityProperty<T2>(NexusManager<T2> manager, EntityBase entity, string label = null, string entityFormat = "{0}") where T2 :   EntityBase, new()
		{
			if (entity == null)
				return;
			if (label != null) {
				currentLayout.Children.Add (
					new Label {
						HorizontalOptions = LayoutOptions.Start,
						FontAttributes = FontAttributes.Bold,
						Text = label
					});
			}
			ExtendedLabel entityLabel = CreateEntityLabel<T2> (manager, entity, entityFormat);
			currentLayout.Children.Add (entityLabel);
		}

		protected ExtendedLabel CreateEntityLabel<T2>(NexusManager<T2> manager, EntityBase entity, string entityFormat = "{0}") where T2 :   EntityBase, new()
		{
			ExtendedLabel entityLabel = new ExtendedLabel {
				Text = string.Format(entityFormat, entity.ToString ()),
				TextColor = Color.Blue,
				IsUnderline = true
			};
			TapGestureRecognizer tapGesture = new TapGestureRecognizer();
			tapGesture.Command = new Command (() => {
				EntityPageBuilderFactory.ShowEntityPage<T2>(manager,entity.Id);
			});
			entityLabel.GestureRecognizers.Add (tapGesture);
			return entityLabel;
		}
	}
}


