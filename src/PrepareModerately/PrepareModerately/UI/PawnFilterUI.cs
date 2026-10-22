using System.Collections.Generic;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;
using Lakuna.PrepareModerately.Utility;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `RimWorld.ScenarioUI`.
	public static class PawnFilterUi {
		private static float _editViewHeight;

		private const float _extraScrollHeight = 100;

		private const float _filterNameLabelHeight = 30;

		private const float _listingRectHeight = 99999; // Arbitrary large number.

		private const float _viewRectHorizontalPadding = 16;

		private const float _viewRectVerticalPadding = 30;

		public static void DrawEditInterface(Rect rect, PawnFilter filter, ref Vector2 infoScrollPosition) {
			if (filter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw edit interface with null filter.");
				return;
			}

			Widgets.DrawMenuSection(rect);
			Rect innerRect = rect.GetInnerRect().Rounded();
			Rect viewRect = new Rect(0, 0, innerRect.width - _viewRectHorizontalPadding, _editViewHeight).Rounded();
			Widgets.BeginScrollView(innerRect, ref infoScrollPosition, viewRect);

			Rect listingRect = new Rect(0, 0, viewRect.width, _listingRectHeight).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			PawnFilterEditListing listing = new PawnFilterEditListing(filter) {
#else
			PawnFilterEditListing listing = new(filter) {
#endif
				ColumnWidth = listingRect.width
			};
			listing.Begin(listingRect);

#if V1_0
			listing.Label("PM.Title".Translate().CapitalizeFirst());
#else
			_ = listing.Label("PM.Title".Translate().CapitalizeFirst());
#endif
			filter.Name = listing.TextEntry(filter.Name)?.TrimmedToLength(PawnFilter.NameMaxLength) ?? "";

#if V1_0
			listing.Label("PM.Summary".Translate().CapitalizeFirst());
#else
			_ = listing.Label("PM.Summary".Translate().CapitalizeFirst());
#endif
			filter.Summary = listing.TextEntry(filter.Summary, 2)?.TrimmedToLength(PawnFilter.SummaryMaxLength) ?? "";

#if V1_0
			listing.Label("PM.Description".Translate().CapitalizeFirst());
#else
			_ = listing.Label("PM.Description".Translate().CapitalizeFirst());
#endif
			filter.Description = listing.TextEntry(filter.Description, 4)?.TrimmedToLength(PawnFilter.DescriptionMaxLength) ?? "";

			listing.Gap();

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			IEnumerable<PawnFilterPart> parts = filter.Parts is null ? new List<PawnFilterPart>() : new List<PawnFilterPart>(filter.Parts); // Necessary so that deleting a pawn filter part doesn't cause an error.
#else
			IEnumerable<PawnFilterPart> parts = filter.Parts is null ? [] : [.. filter.Parts]; // Necessary so that deleting a pawn filter part doesn't cause an error.
#endif
			foreach (PawnFilterPart part in parts) {
				part.DoEditInterface(listing);
			}

			listing.End();
			_editViewHeight = listing.CurHeight + _extraScrollHeight;
			Widgets.EndScrollView();
		}

		public static void DrawInfo(Rect rect, PawnFilter filter, ref Vector2 infoScrollPosition) {
			if (filter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw information of null filter.");
				return;
			}

			Widgets.DrawMenuSection(rect);
			Rect innerRect = rect.GetInnerRect().Rounded();

			Text.Font = GameFont.Small;
			float width = innerRect.width - _viewRectHorizontalPadding;
			float height = _viewRectVerticalPadding + Text.CalcHeight(filter.FullInformationText, width) + _extraScrollHeight;

			Rect viewRect = new Rect(0, 0, width, height).Rounded();
			Widgets.BeginScrollView(innerRect, ref infoScrollPosition, viewRect);

			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0, 0, viewRect.width, _filterNameLabelHeight).Rounded(), filter.Name ?? "null");

			Text.Font = GameFont.Small;
			Widgets.Label(new Rect(0, _filterNameLabelHeight, viewRect.width, viewRect.height - _filterNameLabelHeight).Rounded(), filter.FullInformationText);

			Widgets.EndScrollView();
		}
	}
}
