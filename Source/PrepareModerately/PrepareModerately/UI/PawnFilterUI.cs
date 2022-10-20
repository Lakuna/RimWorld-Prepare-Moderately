using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.UI {
	internal class PawnFilterUI {
		private static float editViewHeight;

		private const float viewRectHorizontalPadding = 16;

		private const float viewRectVerticalPadding = 30;

		private const float extraScrollHeight = 100;

		private const float filterNameLabelHeight = 30;

		private const float listingRectHeight = 99999; // Arbitrary large number.

		public static void DrawInfo(Rect rect, PawnFilter filter, ref Vector2 infoScrollPosition) {
			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();

			if (filter == null) { return; }

			string fullInformationText = filter.FullInformationText;

			float width = rect.width - viewRectHorizontalPadding;
			float height = viewRectVerticalPadding + Text.CalcHeight(fullInformationText, width) + extraScrollHeight;

			Rect viewRect = new Rect(0, 0, width, height);
			Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect);

			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0, 0, viewRect.width, filterNameLabelHeight), filter.Name);

			Text.Font = GameFont.Small;
			Widgets.Label(new Rect(0, filterNameLabelHeight, viewRect.width, viewRect.height - filterNameLabelHeight), fullInformationText);

			Widgets.EndScrollView();
		}

		public static void DrawEditInterface(Rect rect, PawnFilter filter, ref Vector2 infoScrollPosition) {
			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();

			if (filter == null) { return; }

			Rect viewRect = new Rect(0, 0, rect.width - viewRectHorizontalPadding, editViewHeight);
			Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect);

			Rect listingRect = new Rect(0, 0, viewRect.width, listingRectHeight);
			PawnFilterEditListing listing = new PawnFilterEditListing(filter) { ColumnWidth = listingRect.width };
			listing.Begin(listingRect);

			listing.Label("Title".Translate().CapitalizeFirst());
			filter.Name = listing.TextEntry(filter.Name).TrimmedToLength(PawnFilter.NameMaxLength);

			listing.Label("Summary".Translate().CapitalizeFirst());
			filter.Summary = listing.TextEntry(filter.Summary, 2).TrimmedToLength(PawnFilter.SummaryMaxLength);

			listing.Label("Description".Translate().CapitalizeFirst());
			filter.Description = listing.TextEntry(filter.Description, 4).TrimmedToLength(PawnFilter.DescriptionMaxLength);

			listing.Gap();

			foreach (PawnFilterPart part in filter.Parts) { part.DoEditInterface(listing); }

			listing.End();
			PawnFilterUI.editViewHeight = listing.CurHeight + extraScrollHeight;
			Widgets.EndScrollView();
		}
	}
}
