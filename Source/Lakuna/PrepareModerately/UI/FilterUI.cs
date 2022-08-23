using Lakuna.PrepareModerately.Filter.FilterPart;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.UI {
	public static class FilterUI {
		private static float editViewHeight;

		public static void DrawFilterInfo(Rect rect, Filter.Filter filter, ref Vector2 infoScrollPosition) {
			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();

			if (filter == null) { return; }

			string fullInformationText = filter.GetFullInformationText();

			float width = rect.width - 16;
			float height = 30 + Text.CalcHeight(fullInformationText, width) + 100;

			Rect viewRect = new Rect(0, 0, width, height);
			Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect);

			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0, 0, viewRect.width, 30), filter.name);

			Text.Font = GameFont.Small;
			Widgets.Label(new Rect(0, 30, viewRect.width, viewRect.height - 30), fullInformationText);

			Widgets.EndScrollView();
		}

		public static void DrawFilterEditInterface(Rect rect, Filter.Filter filter, ref Vector2 infoScrollPosition) {
			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();

			if (filter == null) { return; }

			Rect viewRect = new Rect(0, 0, rect.width - 16, FilterUI.editViewHeight);
			Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect);

			Rect listingRect = new Rect(0, 0, viewRect.width, 99999);
			FilterEditListing listing = new FilterEditListing(filter);
			listing.ColumnWidth = listingRect.width;
			listing.Begin(listingRect);

			listing.Label("Title".Translate());
			filter.name = listing.TextEntry(filter.name).TrimmedToLength(55);

			listing.Label("Summary".Translate());
			filter.summary = listing.TextEntry(filter.summary, 2).TrimmedToLength(300);

			listing.Label("Description".Translate());
			filter.description = listing.TextEntry(filter.description, 4).TrimmedToLength(1000);

			listing.Gap();

			foreach (FilterPart part in filter.AllParts) { part.DoEditInterface(listing); }

			listing.End();

			FilterUI.editViewHeight = listing.CurHeight + 100;

			Widgets.EndScrollView();
		}
	}
}
