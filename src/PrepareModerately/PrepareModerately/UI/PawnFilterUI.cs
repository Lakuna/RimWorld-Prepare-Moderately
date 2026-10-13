using System;
using System.Collections.Generic;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.UI {
	// Based on `RimWorld.ScenarioUI`.
	static internal class PawnFilterUi {
		private static float EditViewHeight;

		private const float ViewRectHorizontalPadding = 16;

		private const float ViewRectVerticalPadding = 30;

		private const float ExtraScrollHeight = 100;

		private const float FilterNameLabelHeight = 30;

		private const float ListingRectHeight = 99999; // Arbitrary large number.

		public static void DrawInfo(Rect rect, PawnFilter filter, ref Vector2 infoScrollPosition) {
			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();

			if (filter is null) {
				return;
			}

			string fullInformationText = filter.FullInformationText;

			float width = rect.width - ViewRectHorizontalPadding;
			float height = ViewRectVerticalPadding + Text.CalcHeight(fullInformationText, width) + ExtraScrollHeight;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect viewRect = new Rect(0, 0, width, height);
#else
			Rect viewRect = new(0, 0, width, height);
#endif
			Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect);

			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0, 0, viewRect.width, FilterNameLabelHeight), filter.NameP);

			Text.Font = GameFont.Small;
			Widgets.Label(new Rect(0, FilterNameLabelHeight, viewRect.width, viewRect.height - FilterNameLabelHeight), fullInformationText);

			Widgets.EndScrollView();
		}

		public static void DrawEditInterface(Rect rect, PawnFilter filter, ref Vector2 infoScrollPosition) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (filter is null) {
				throw new ArgumentNullException(nameof(filter));
			}
#else
			ArgumentNullException.ThrowIfNull(filter);
#endif

			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect viewRect = new Rect(0, 0, rect.width - ViewRectHorizontalPadding, EditViewHeight);
#else
			Rect viewRect = new(0, 0, rect.width - ViewRectHorizontalPadding, EditViewHeight);
#endif
			Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect listingRect = new Rect(0, 0, viewRect.width, ListingRectHeight);
			PawnFilterEditListing listing = new PawnFilterEditListing(filter) {
#else
			Rect listingRect = new(0, 0, viewRect.width, ListingRectHeight);
			PawnFilterEditListing listing = new(filter) {
#endif
				ColumnWidth = listingRect.width
			};
			listing.Begin(listingRect);

#if V1_0
			listing.Label("PM.Title".Translate().CapitalizeFirst());
			filter.Name = listing.TextEntry(filter.Name).TrimmedToLength(PawnFilter.NameMaxLength);

			listing.Label("PM.Summary".Translate().CapitalizeFirst());
			filter.Summary = listing.TextEntry(filter.Summary, 2).TrimmedToLength(PawnFilter.SummaryMaxLength);

			listing.Label("PM.Description".Translate().CapitalizeFirst());
			filter.Description = listing.TextEntry(filter.Description, 4).TrimmedToLength(PawnFilter.DescriptionMaxLength);
#else
			_ = listing.Label("PM.Title".Translate().CapitalizeFirst());
			filter.NameP = listing.TextEntry(filter.NameP).TrimmedToLength(PawnFilter.NameMaxLength);

			_ = listing.Label("PM.Summary".Translate().CapitalizeFirst());
			filter.SummaryP = listing.TextEntry(filter.SummaryP, 2).TrimmedToLength(PawnFilter.SummaryMaxLength);

			_ = listing.Label("PM.Description".Translate().CapitalizeFirst());
			filter.DescriptionP = listing.TextEntry(filter.DescriptionP, 4).TrimmedToLength(PawnFilter.DescriptionMaxLength);
#endif

			listing.Gap();

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			List<PawnFilterPart> parts = new List<PawnFilterPart>();
#else
			List<PawnFilterPart> parts = [];
#endif
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
			foreach (PawnFilterPart part in filter.PartsP) {
				parts.Add(part);
			}
#else
			filter.PartsP.CopyToList(parts);
#endif
			foreach (PawnFilterPart part in parts) {
				part.DoEditInterface(listing, out _);
			}

			listing.End();
			EditViewHeight = listing.CurHeight + ExtraScrollHeight;
			Widgets.EndScrollView();
		}
	}
}
