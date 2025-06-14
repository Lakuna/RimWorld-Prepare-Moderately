using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;
using System;
using System.Linq;
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

			if (filter == null) {
				return;
			}

			string fullInformationText = filter.FullInformationText;

			float width = rect.width - ViewRectHorizontalPadding;
			float height = ViewRectVerticalPadding + Text.CalcHeight(fullInformationText, width) + ExtraScrollHeight;

			Rect viewRect = new Rect(0, 0, width, height);
			Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect);

			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0, 0, viewRect.width, FilterNameLabelHeight), filter.Name);

			Text.Font = GameFont.Small;
			Widgets.Label(new Rect(0, FilterNameLabelHeight, viewRect.width, viewRect.height - FilterNameLabelHeight), fullInformationText);

			Widgets.EndScrollView();
		}

		public static void DrawEditInterface(Rect rect, PawnFilter filter, ref Vector2 infoScrollPosition) {
			if (filter == null) {
				throw new ArgumentNullException(nameof(filter));
			}

			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();

			Rect viewRect = new Rect(0, 0, rect.width - ViewRectHorizontalPadding, EditViewHeight);
			Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect);

			Rect listingRect = new Rect(0, 0, viewRect.width, ListingRectHeight);
			PawnFilterEditListing listing = new PawnFilterEditListing(filter) { ColumnWidth = listingRect.width };
			listing.Begin(listingRect);

#if V1_0
			listing.Label("Title".Translate().CapitalizeFirst());
			filter.Name = listing.TextEntry(filter.Name).TrimmedToLength(PawnFilter.NameMaxLength);

			listing.Label("Summary".Translate().CapitalizeFirst());
			filter.Summary = listing.TextEntry(filter.Summary, 2).TrimmedToLength(PawnFilter.SummaryMaxLength);

			listing.Label("Description".Translate().CapitalizeFirst());
			filter.Description = listing.TextEntry(filter.Description, 4).TrimmedToLength(PawnFilter.DescriptionMaxLength);
#else
			_ = listing.Label("Title".Translate().CapitalizeFirst());
			filter.Name = listing.TextEntry(filter.Name).TrimmedToLength(PawnFilter.NameMaxLength);

			_ = listing.Label("Summary".Translate().CapitalizeFirst());
			filter.Summary = listing.TextEntry(filter.Summary, 2).TrimmedToLength(PawnFilter.SummaryMaxLength);

			_ = listing.Label("Description".Translate().CapitalizeFirst());
			filter.Description = listing.TextEntry(filter.Description, 4).TrimmedToLength(PawnFilter.DescriptionMaxLength);
#endif

			listing.Gap();

			foreach (PawnFilterPart part in filter.Parts) {
				part.DoEditInterface(listing, out _);
			}

			listing.End();
			EditViewHeight = listing.CurHeight + ExtraScrollHeight;
			Widgets.EndScrollView();
		}
	}
}
