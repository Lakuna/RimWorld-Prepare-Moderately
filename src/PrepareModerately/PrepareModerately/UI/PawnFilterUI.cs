using System.Collections.Generic;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `RimWorld.ScenarioUI`.
	static internal class PawnFilterUi {
		private static float _editViewHeight;

		private const float _extraScrollHeight = 100;

		private const float _filterNameLabelHeight = 30;

		private const float _listingRectHeight = 99999; // Arbitrary large number.

		private const float _viewRectHorizontalPadding = 16;

		private const float _viewRectVerticalPadding = 30;

		internal static void DrawEditInterface(Rect rect, PawnFilter filter, ref Vector2 infoScrollPosition) {
			Widgets.DrawMenuSection(rect);
			Rect innerRect = rect.GetInnerRect();

			if (filter is null) {
				return;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect viewRect = new Rect(0, 0, innerRect.width - _viewRectHorizontalPadding, _editViewHeight);
#else
			Rect viewRect = new(0, 0, innerRect.width - _viewRectHorizontalPadding, _editViewHeight);
#endif
			Widgets.BeginScrollView(innerRect, ref infoScrollPosition, viewRect);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect listingRect = new Rect(0, 0, viewRect.width, _listingRectHeight);
			PawnFilterEditListing listing = new PawnFilterEditListing(filter) {
				ColumnWidth = listingRect.width
			};
#else
			Rect listingRect = new(0, 0, viewRect.width, _listingRectHeight);
			PawnFilterEditListing listing = new(filter) {
				ColumnWidth = listingRect.width
			};
#endif
			listing.Begin(listingRect);

#if V1_0
			listing.Label("PM.Title".Translate().CapitalizeFirst());
			filter.name = listing.TextEntry(filter.name).TrimmedToLength(PawnFilter._nameMaxLength);

			listing.Label("PM.Summary".Translate().CapitalizeFirst());
			filter.summary = listing.TextEntry(filter.summary, 2).TrimmedToLength(PawnFilter._summaryMaxLength);

			listing.Label("PM.Description".Translate().CapitalizeFirst());
			filter.description = listing.TextEntry(filter.description, 4).TrimmedToLength(PawnFilter._descriptionMaxLength);
#else
			_ = listing.Label("PM.Title".Translate().CapitalizeFirst());
			filter.name = listing.TextEntry(filter.name).TrimmedToLength(PawnFilter._nameMaxLength);

			_ = listing.Label("PM.Summary".Translate().CapitalizeFirst());
			filter.summary = listing.TextEntry(filter.summary, 2).TrimmedToLength(PawnFilter._summaryMaxLength);

			_ = listing.Label("PM.Description".Translate().CapitalizeFirst());
			filter.description = listing.TextEntry(filter.description, 4).TrimmedToLength(PawnFilter._descriptionMaxLength);
#endif

			listing.Gap();

			IEnumerable<PawnFilterPart> parts = filter.parts.ListFullCopy(); // Necessary so that deleting a pawn filter part doesn't cause an error.
			foreach (PawnFilterPart part in parts) {
				part.DoEditInterface(listing);
			}

			listing.End();
			_editViewHeight = listing.CurHeight + _extraScrollHeight;
			Widgets.EndScrollView();
		}

		internal static void DrawInfo(Rect rect, PawnFilter filter, ref Vector2 infoScrollPosition) {
			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();

			if (filter is null) {
				return;
			}

			string fullInformationText = filter._fullInformationText;
			Text.Font = GameFont.Small;
			float width = rect.width - _viewRectHorizontalPadding;
			float height = _viewRectVerticalPadding + Text.CalcHeight(fullInformationText, width) + _extraScrollHeight;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect viewRect = new Rect(0, 0, width, height);
#else
			Rect viewRect = new(0, 0, width, height);
#endif
			Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect);

			Text.Font = GameFont.Medium;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Widgets.Label(new Rect(0, 0, viewRect.width, _filterNameLabelHeight), filter.name);
#else
			Widgets.Label(new(0, 0, viewRect.width, _filterNameLabelHeight), filter.name);
#endif

			Text.Font = GameFont.Small;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Widgets.Label(new Rect(0, _filterNameLabelHeight, viewRect.width, viewRect.height - _filterNameLabelHeight), fullInformationText);
#else
			Widgets.Label(new(0, _filterNameLabelHeight, viewRect.width, viewRect.height - _filterNameLabelHeight), fullInformationText);
#endif

			Widgets.EndScrollView();
		}
	}
}
