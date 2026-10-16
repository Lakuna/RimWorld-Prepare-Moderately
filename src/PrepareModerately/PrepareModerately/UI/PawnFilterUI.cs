using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `RimWorld.ScenarioUI`.
	static internal class PawnFilterUi {
		private static float s_editViewHeight;

		private const float ExtraScrollHeight = 100;

		private const float FilterNameLabelHeight = 30;

		private const float ListingRectHeight = 99999; // Arbitrary large number.

		private const float ViewRectHorizontalPadding = 16;

		private const float ViewRectVerticalPadding = 30;

		internal static void DrawEditInterface(Rect rect, PawnFilter filter, ref Vector2 infoScrollPosition) {
			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();

			if (filter is null) {
				return;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect viewRect = new Rect(0, 0, rect.width - ViewRectHorizontalPadding, s_editViewHeight);
#else
			Rect viewRect = new(0, 0, rect.width - ViewRectHorizontalPadding, s_editViewHeight);
#endif
			Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect listingRect = new Rect(0, 0, viewRect.width, ListingRectHeight);
			PawnFilterEditListing listing = new PawnFilterEditListing(filter) {
				ColumnWidth = listingRect.width
			};
#else
			Rect listingRect = new(0, 0, viewRect.width, ListingRectHeight);
			PawnFilterEditListing listing = new(filter) {
				ColumnWidth = listingRect.width
			};
#endif
			listing.Begin(listingRect);

#if V1_0
			listing.Label("PM.Title".Translate().CapitalizeFirst());
			filter.name = listing.TextEntry(filter.name).TrimmedToLength(PawnFilter.NameMaxLength);

			listing.Label("PM.Summary".Translate().CapitalizeFirst());
			filter.summary = listing.TextEntry(filter.summary, 2).TrimmedToLength(PawnFilter.SummaryMaxLength);

			listing.Label("PM.Description".Translate().CapitalizeFirst());
			filter.description = listing.TextEntry(filter.description, 4).TrimmedToLength(PawnFilter.DescriptionMaxLength);
#else
			_ = listing.Label("PM.Title".Translate().CapitalizeFirst());
			filter.name = listing.TextEntry(filter.name).TrimmedToLength(PawnFilter.NameMaxLength);

			_ = listing.Label("PM.Summary".Translate().CapitalizeFirst());
			filter.summary = listing.TextEntry(filter.summary, 2).TrimmedToLength(PawnFilter.SummaryMaxLength);

			_ = listing.Label("PM.Description".Translate().CapitalizeFirst());
			filter.description = listing.TextEntry(filter.description, 4).TrimmedToLength(PawnFilter.DescriptionMaxLength);
#endif

			listing.Gap();

			foreach (PawnFilterPart part in filter.parts) {
				part.DoEditInterface(listing, out _);
			}

			listing.End();
			s_editViewHeight = listing.CurHeight + ExtraScrollHeight;
			Widgets.EndScrollView();
		}

		internal static void DrawInfo(Rect rect, PawnFilter filter, ref Vector2 infoScrollPosition) {
			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();

			if (filter is null) {
				return;
			}

			string fullInformationText = filter.FullInformationText;
			Text.Font = GameFont.Small;
			float width = rect.width - ViewRectHorizontalPadding;
			float height = ViewRectVerticalPadding + Text.CalcHeight(fullInformationText, width) + ExtraScrollHeight;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect viewRect = new Rect(0, 0, width, height);
#else
			Rect viewRect = new(0, 0, width, height);
#endif
			Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect);

			Text.Font = GameFont.Medium;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Widgets.Label(new Rect(0, 0, viewRect.width, FilterNameLabelHeight), filter.name);
#else
			Widgets.Label(new(0, 0, viewRect.width, FilterNameLabelHeight), filter.name);
#endif

			Text.Font = GameFont.Small;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Widgets.Label(new Rect(0, FilterNameLabelHeight, viewRect.width, viewRect.height - FilterNameLabelHeight), fullInformationText);
#else
			Widgets.Label(new(0, FilterNameLabelHeight, viewRect.width, viewRect.height - FilterNameLabelHeight), fullInformationText);
#endif

			Widgets.EndScrollView();
		}
	}
}
