//
// BasePhoenixPage.cs
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

using PhoenixImperator.Pages.Entities;

namespace PhoenixImperator
{
	/// <summary>
	/// Base class for most pages
	/// </summary>
	public class PhoenixPage : ContentPage
	{
		/// <summary>
		/// Gets the page layout.
		/// </summary>
		/// <value>The page layout.</value>
		public StackLayout PageLayout { get; private set; }

		/// <summary>
		/// Gets the spinner.
		/// </summary>
		/// <value>The spinner.</value>
		public ActivityIndicator Spinner { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is searching.
		/// </summary>
		/// <value><c>true</c> if this instance is searching; otherwise, <c>false</c>.</value>
		public bool IsSearching { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.PhoenixPage"/> class.
		/// </summary>
		public PhoenixPage (string title)
		{
			Title = title;
			BackgroundColor = Color.White;
			Spinner = new ActivityIndicator {
				IsEnabled = true,
				IsRunning = false,
				BindingContext = this
			};
			PageLayout = new StackLayout { 
				VerticalOptions = LayoutOptions.FillAndExpand,
				Padding = new Thickness (10, Device.OnPlatform (20, 0, 0), 10, 5),
				Children = {
					Spinner
				}
			};
			Content = PageLayout;
		}

		/// <summary>
		/// Shows the info alert.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="info">Info.</param>
		public void ShowInfoAlert(string title, object info)
		{
			if (title == null || info == null) {
				return;
			}
			Device.BeginInvokeOnMainThread(() => {
				DisplayAlert(title, info.ToString(),"OK");
			});
		}

		/// <summary>
		/// Shows the error alert.
		/// </summary>
		/// <param name="error">Error.</param>
		public void ShowErrorAlert(object error)
		{
			if (error == null) {
				return;
			}
			#if DEBUG
			string errorMessage = error.ToString ();
			#else
			string errorMessage = "There was a problem connecting to Nexus";
			#endif

			Device.BeginInvokeOnMainThread(() => {
				DisplayAlert("Problem", errorMessage,"OK");
			});
		}

		/// <summary>
		/// Adds the heading.
		/// </summary>
		/// <param name="heading">Heading.</param>
		public ExtendedLabel AddHeading(string heading)
		{
			if (heading == null || PageLayout == null)
				return null;
			ExtendedLabel label = new ExtendedLabel {
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				FontAttributes = FontAttributes.Bold,
				Text = heading,
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
				IsUnderline = true
			};
			PageLayout.Children.Add (label);
			return label;
		}

		/// <summary>
		/// Adds the property.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public Label AddProperty(string key, string value)
		{
			if (PageLayout == null)
				return null;
			Label label = new Label {
				HorizontalOptions = LayoutOptions.EndAndExpand,
				Text = value == null ? "" : value
			};
			PageLayout.Children.Add (
				new StackLayout{
					VerticalOptions = LayoutOptions.Start,
					Orientation = StackOrientation.Horizontal,
					Children = {
						new Label{
							HorizontalOptions = LayoutOptions.Start,
							FontAttributes = FontAttributes.Bold,
							Text = key == null ? "" : key + ":"
						},
						label
					}
				}
			);
			return label;
		}

		/// <summary>
		/// Adds the property double line.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public Label AddPropertyDoubleLine(string key, string value)
		{
			if (PageLayout == null)
				return null;
			if (key != null) {
				PageLayout.Children.Add (
					new Label {
						HorizontalOptions = LayoutOptions.Start,
						FontAttributes = FontAttributes.Bold,
						Text = key
					});
			}
			Label label = new Label {
				HorizontalOptions = LayoutOptions.Start
			};
			if (value != null) {
				label.Text = value;
			}
			PageLayout.Children.Add (
				label
			);
			return label;
		}

		/// <summary>
		/// Adds the label.
		/// </summary>
		/// <param name="value">Value.</param>
		public Label AddLabel(string value)
		{
			if (PageLayout == null || value == null)
				return null;
			Label label = new Label {
				HorizontalOptions = LayoutOptions.Start,
				Text = value
			};
			PageLayout.Children.Add (
				label
			);
			return label;
		}

		/// <summary>
		/// Adds the entity property.
		/// </summary>
		/// <param name="manager">Manager.</param>
		/// <param name="entity">Entity.</param>
		/// <param name="label">Label.</param>
		/// <param name="entityFormat">Entity format.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public ExtendedLabel AddEntityProperty<T>(NexusManager<T> manager, EntityBase entity, string label = null, string entityFormat = "{0}") where T :   EntityBase, new()
		{
			if (PageLayout == null || manager == null || entity == null)
				return null;
			if (label != null) {
				PageLayout.Children.Add (
					new Label {
						HorizontalOptions = LayoutOptions.Start,
						FontAttributes = FontAttributes.Bold,
						Text = label
					});
			}
			ExtendedLabel entityLabel = CreateEntityLabel<T> (manager, entity, entityFormat);
			PageLayout.Children.Add (entityLabel);
			return entityLabel;
		}

		/// <summary>
		/// Creates the entity label.
		/// </summary>
		/// <returns>The entity label.</returns>
		/// <param name="manager">Manager.</param>
		/// <param name="entity">Entity.</param>
		/// <param name="entityFormat">Entity format.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public ExtendedLabel CreateEntityLabel<T>(NexusManager<T> manager, EntityBase entity, string entityFormat = "{0}") where T :   EntityBase, new()
		{
			ExtendedLabel entityLabel = new ExtendedLabel {
				Text = string.Format(entityFormat, entity.ToString ()),
				TextColor = Color.Blue,
				IsUnderline = true
			};
			TapGestureRecognizer tapGesture = new TapGestureRecognizer();
			tapGesture.Command = new Command ((e) => {
				EntityPageBuilderFactory.ShowEntityPage<T>(manager,entity.Id);
			});
			entityLabel.GestureRecognizers.Add (tapGesture);
			return entityLabel;
		}

		/// <summary>
		/// Adds the help label.
		/// </summary>
		/// <param name="text">Text.</param>
		public Label AddHelpLabel(string text)
		{
			Label help = new Label {
				Text = text,
				FontSize = Device.GetNamedSize (NamedSize.Small, typeof(Label)),
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				FontAttributes = FontAttributes.Italic
			};
			PageLayout.Children.Add (help);
			return help;
		}

		/// <summary>
		/// Adds the list view.
		/// </summary>
		/// <returns>The list view.</returns>
		/// <param name="cellType">Cell type.</param>
		/// <param name="source">Source.</param>
		/// <param name="tapAction">Tap action.</param>
		public ListView AddListView(Type cellType, IEnumerable<EntityBase> source, Action<object, ItemTappedEventArgs> tapAction)
		{
			ListView listView = new ListView ();

			listView.ItemTemplate = new DataTemplate (cellType);
			listView.ItemTemplate.SetBinding (TextCell.TextProperty, "ListText");
			listView.ItemTemplate.SetBinding (TextCell.DetailProperty, "ListDetail");
			listView.ItemsSource = source;

			listView.ItemTapped += (object sender, ItemTappedEventArgs e) => {
				Log.WriteLine (Log.Layer.UI, GetType (), "Tapped: " + e.Item + "(" + e.Item.GetType() + ")");
				((ListView)sender).SelectedItem = null; // de-select the row
				tapAction(sender,e);
			};

			PageLayout.Children.Add (listView);

			return listView;
		}

		/// <summary>
		/// Adds the list view with search bar.
		/// </summary>
		/// <param name="cellType">Cell type.</param>
		/// <param name="source">Source.</param>
		/// <param name="tapAction">Tap action.</param>
		public ListView AddListViewWithSearchBar(Type cellType, IEnumerable<EntityBase> source, Action<object, ItemTappedEventArgs> tapAction)
		{
			SearchBar searchBar = new SearchBar () {
				Placeholder = "Search"
			};
			ListView listView = new ListView ();

			searchBar.TextChanged += (sender, e) => {
				IsSearching = true;
				FilterListView(listView,searchBar.Text,source);
			};
			searchBar.SearchButtonPressed += (sender, e) => {
				IsSearching = true;
				FilterListView(listView,searchBar.Text,source);
			};

			listView.ItemTemplate = new DataTemplate (cellType);
			listView.ItemTemplate.SetBinding (TextCell.TextProperty, "ListText");
			listView.ItemTemplate.SetBinding (TextCell.DetailProperty, "ListDetail");
			listView.ItemsSource = source;

			listView.ItemTapped += (object sender, ItemTappedEventArgs e) => {
				Log.WriteLine (Log.Layer.UI, GetType (), "Tapped: " + e.Item + "(" + e.Item.GetType() + ")");
				((ListView)sender).SelectedItem = null; // de-select the row
				tapAction(sender,e);
			};

			PageLayout.Children.Add (searchBar);
			PageLayout.Children.Add (listView);

			return listView;
		}

		/// <summary>
		/// Adds the logo.
		/// </summary>
		public Image AddLogo()
		{
			Image logo = new Image { Aspect = Aspect.AspectFill };
			logo.Source = ImageSource.FromFile ("logo.png");

			PageLayout.Children.Add (logo);
			return logo;
		}

		protected virtual void FilterListView(ListView listView, string filter, IEnumerable<EntityBase> source)
		{
			listView.BeginRefresh ();

			if (string.IsNullOrWhiteSpace (filter)) {
				listView.ItemsSource = source;
			} else {
				listView.ItemsSource = (source.Where (x => x.ToString ().ToLower ().Contains (filter.ToLower ())));
			}

			IsSearching = false;
			listView.EndRefresh ();
		}
	}
}


