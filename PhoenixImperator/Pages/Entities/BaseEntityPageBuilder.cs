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

using Xamarin;
using Xamarin.Forms;

using XLabs.Forms.Controls;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;

using PhoenixImperator.Pages;

namespace PhoenixImperator.Pages.Entities
{
	/// <summary>
	/// Base entity page builder.
	/// </summary>
	public abstract class BaseEntityPageBuilder<T> : IEntityPageBuilder<T> where T :   EntityBase, new()
	{
		/// <summary>
		/// Gets or sets the manager.
		/// </summary>
		/// <value>The manager.</value>
		public NexusManager<T> Manager { get; set; }

		/// <summary>
		/// Builds the page.
		/// </summary>
		/// <returns>The page.</returns>
		/// <param name="item">Item.</param>
		public TabbedPage BuildPage(T item)
		{
			if (item == null) {
				return new TabbedPage {
					Title = "Not Found",
					Children = {
						new PhoenixPage("Not Found") {
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
			Insights.Track (item.GetType().ToString());
			entityPage = new EntityContentPage (item);
			DisplayEntity (item);
			return entityPage;
		}

		/// <summary>
		/// Shows the info alert.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="info">Info.</param>
		protected void ShowInfoAlert(string title, object info)
		{
			currentTab.ShowInfoAlert(title,info);
		}

		/// <summary>
		/// Shows the error alert.
		/// </summary>
		/// <param name="error">Error.</param>
		protected void ShowErrorAlert(object error)
		{
			currentTab.ShowErrorAlert(error);
		}

		/// <summary>
		/// Adds the content tab.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="icon">Icon.</param>
		protected PhoenixPage AddContentTab(string title, string icon)
		{
			currentTab = new PhoenixPage(title) {
				Icon = icon
			};
			entityPage.Children.Add (currentTab);
			return currentTab;
		}

		/// <summary>
		/// Adds the copy button.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="value">Value.</param>
		protected void AddCopyButton(string label, string value)
		{
			if (currentTab != null) {
				Button button = new Button {
					Text = label,
					VerticalOptions = LayoutOptions.Start,
				};
				currentTab.PageLayout.Children.Add (button);
				button.Clicked += (sender, e) => {
					App.ClipboardService.CopyToClipboard (value);
				};
			}
		}

		/// <summary>
		/// Displaies the entity.
		/// </summary>
		/// <param name="item">Item.</param>
		protected abstract void DisplayEntity(T item);

		protected EntityContentPage entityPage;
		protected PhoenixPage currentTab;
	}
}


