//
// NotificationPageBuilder.cs
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

namespace PhoenixImperator.Pages.Entities
{
	/// <summary>
	/// Notification page builder.
	/// </summary>
	public class NotificationPageBuilder : BaseEntityPageBuilder<Notification>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.Entities.NotificationPageBuilder"/> class.
		/// </summary>
		public NotificationPageBuilder ()
		{
		}

		/// <summary>
		/// Displaies the entity.
		/// </summary>
		/// <param name="item">Item.</param>
		protected override void DisplayEntity (Notification item)
		{
			entityPage.Title = item.TypeDescription;

			string dateLabel;
			if (item.DaysAgo > 0) {
				if (item.DaysAgo == 1) {
					dateLabel = ("Yesterday");
				} else {
					dateLabel = (item.DaysAgo + " days ago");
				}
			} else {
				dateLabel = ("Today");
			}

			AddContentTab (dateLabel, "icon_notifications.png");
			currentTab.AddHeading (item.ListText);

			if (item.Type == Notification.NotificationType.ComplexChange) {
				currentTab.AddLabel (item.ChangeType.ToString ());
			}

			if (item.Type == Notification.NotificationType.RegisteredBaseActivity) {
				currentTab.AddLabel (item.BaseActivityType.ToString ());
			}

			// AddProperty ("Priority", item.Priority.ToString ());


			if (item.Position != null) {
				currentTab.AddEntityProperty (Phoenix.Application.PositionManager, item.Position, "Position");
			} else if(!string.IsNullOrWhiteSpace(item.PositionName)){
				currentTab.AddPropertyDoubleLine ("Position", item.PositionName);
			}

			if (item.StarSystem != null) {
				currentTab.AddEntityProperty (Phoenix.Application.StarSystemManager, item.StarSystem, "Star System");
			} else if(!string.IsNullOrWhiteSpace(item.StarSystemName)){
				currentTab.AddPropertyDoubleLine ("Star System", item.StarSystemName);
			}

			if(!string.IsNullOrWhiteSpace(item.CelestialBodyName)){
				currentTab.AddPropertyDoubleLine ("Celestial Body", item.CelestialBodyName);
			}

			if (item.Order != null) {
				currentTab.AddEntityProperty (Phoenix.Application.OrderTypeManager, item.Order, "Order");
			} else if(!string.IsNullOrWhiteSpace(item.OrderName)){
				currentTab.AddPropertyDoubleLine ("Order", item.OrderName);
			}

			if (item.PositionType != Notification.NotificationPositionType.None) {
				currentTab.AddProperty ("Position Type", item.PositionType.ToString ());
			}

			if(!string.IsNullOrWhiteSpace(item.SquadronName)){
				currentTab.AddPropertyDoubleLine ("Squadron", item.SquadronName);
			}

			if(!string.IsNullOrWhiteSpace(item.Location)){
				currentTab.AddPropertyDoubleLine ("Location", item.Location);
			}

			if (item.NumberOfShips > 0) {
				currentTab.AddProperty ("Ships", item.NumberOfShips.ToString ());
			}

			if (item.Quantity > 0) {
				currentTab.AddProperty ("Quantity", item.Quantity.ToString ());
			}

			if (item.Item != null) {
				currentTab.AddEntityProperty (Phoenix.Application.ItemManager, item.Item, "Item");
			} else if(!string.IsNullOrWhiteSpace(item.ItemName)){
				currentTab.AddPropertyDoubleLine ("Item", item.ItemName);
			}

			if (item.Base != null) {
				currentTab.AddEntityProperty (Phoenix.Application.PositionManager, item.Base, "Base");
			} else if(!string.IsNullOrWhiteSpace(item.BaseName)){
				currentTab.AddPropertyDoubleLine ("Base", item.BaseName);
			}

			if (item.DeliverTo != null) {
				currentTab.AddEntityProperty (Phoenix.Application.PositionManager, item.DeliverTo, "Delivered To");
			} else if(!string.IsNullOrWhiteSpace(item.DeliverToName)){
				currentTab.AddPropertyDoubleLine ("Deliver To", item.DeliverToName);
			}

			if (item.PickedUpFrom != null) {
				currentTab.AddEntityProperty (Phoenix.Application.PositionManager, item.PickedUpFrom, "Picked Up From");
			} else if(!string.IsNullOrWhiteSpace(item.PickedUpFromName)){
				currentTab.AddPropertyDoubleLine ("Picked Up From", item.PickedUpFromName);
			}

			if (item.BoughtFrom != null) {
				currentTab.AddEntityProperty (Phoenix.Application.PositionManager, item.BoughtFrom, "Bought From");
			} else if(!string.IsNullOrWhiteSpace(item.BoughtFromName)){
				currentTab.AddPropertyDoubleLine ("Bought From", item.BoughtFromName);
			}

			if (item.SoldTo != null) {
				currentTab.AddEntityProperty (Phoenix.Application.PositionManager, item.SoldTo, "Sold To");
			} else if(!string.IsNullOrWhiteSpace(item.SoldToName)){
				currentTab.AddPropertyDoubleLine ("Sold To", item.SoldToName);
			}

			if (item.ByPosition != null) {
				currentTab.AddEntityProperty (Phoenix.Application.PositionManager, item.ByPosition, "By");
			} else if(!string.IsNullOrWhiteSpace(item.ByPositionName)){
				currentTab.AddPropertyDoubleLine ("By", item.ByPositionName);
			}

			if (item.Stellars > 0) {
				currentTab.AddProperty ("Stellars", item.Stellars.ToString ());
			}

			if (!string.IsNullOrWhiteSpace (item.AffiliationName)) {
				currentTab.AddPropertyDoubleLine ("Affiliation", item.AffiliationName);
			}

			if (!string.IsNullOrWhiteSpace (item.Status)) {
				currentTab.AddPropertyDoubleLine ("Status", item.Status);
			}

			if (item.Type == Notification.NotificationType.PlanetarySales) {
				currentTab.AddProperty ("Planetary Sales", item.TradeType.ToString ());
			}

			if (!string.IsNullOrWhiteSpace (item.Subject)) {
				currentTab.AddPropertyDoubleLine ("Subject", item.Subject);
			}

			if (!string.IsNullOrWhiteSpace (item.Message)) {
				currentTab.AddPropertyDoubleLine ("Message", item.Message);
			}

			if (!string.IsNullOrWhiteSpace (item.ErrorCode)) {
				currentTab.AddProperty ("Error Code", item.ErrorCode);
			}

			if (!string.IsNullOrWhiteSpace (item.ErrorMessage)) {
				currentTab.AddPropertyDoubleLine ("Message", item.ErrorMessage);
			}

			if (!string.IsNullOrWhiteSpace (item.WarningCode)) {
				currentTab.AddProperty ("Warning Code", item.WarningCode);
			}

			if (!string.IsNullOrWhiteSpace (item.WarningMessage)) {
				currentTab.AddPropertyDoubleLine ("Message", item.WarningMessage);
			}

			if (item.ComplexItem != null) {
				currentTab.AddEntityProperty (Phoenix.Application.ItemManager, item.ComplexItem, "Complex");
			} else if(!string.IsNullOrWhiteSpace(item.ComplexTypeName)){
				currentTab.AddPropertyDoubleLine ("Complex", item.ComplexTypeName);
			}
		}
	}
}

