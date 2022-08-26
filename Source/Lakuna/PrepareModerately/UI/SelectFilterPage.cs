using System.Collections.Generic;
using System.Linq;
using Lakuna.PrepareModerately.Filter;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.UI {
	[StaticConstructorOnStartup]
	public class SelectFilterPage : Page {
		private Filter.Filter currentFilter;

		private Vector2 infoScrollPosition;

		private const float FilterEntryHeight = 62;

		private Vector2 filtersScrollPosition;

		private float totalFilterListHeight;

		public override string PageTitle => "ChooseFilter".Translate();

		public override void PreOpen() {
			base.PreOpen();
			this.infoScrollPosition = Vector2.zero;
			FilterLister.MarkDirty();
			this.EnsureValidSelection();
		}

		public override void DoWindowContents(Rect rect) {
			this.DrawPageTitle(rect);

			Rect mainRect = this.GetMainRect(rect);
			Widgets.BeginGroup(mainRect);

			Rect filterSelectionListRect = new Rect(0, 0, mainRect.width * 0.35f, mainRect.height).Rounded();
			this.DoFilterSelectionList(filterSelectionListRect);

			FilterUI.DrawFilterInfo(new Rect(filterSelectionListRect.xMax + 17, 0, mainRect.width - filterSelectionListRect.width - 17, mainRect.height).Rounded(), this.currentFilter, ref this.infoScrollPosition);

			Widgets.EndGroup();

			this.DoBottomButtons(rect, null, "FilterEditor".Translate(), this.GoToFilterEditor);
		}

		private bool CanEditFilter(Filter.Filter filter) {
			return filter.Category == FilterCategory.CustomLocal;
		}

		private void GoToFilterEditor() {
			FilterEditorPage filterEditorPage = new FilterEditorPage(this.CanEditFilter(this.currentFilter) ? this.currentFilter : this.currentFilter.CopyForEditing());
			filterEditorPage.prev = this;
			Find.WindowStack.Add(filterEditorPage);
			this.Close();
		}

		private void DoFilterSelectionList(Rect rect) {
			rect.xMax += 2;

			Rect scrollViewRect = new Rect(0, 0, rect.width - 16 - 2, this.totalFilterListHeight + 250);
			Widgets.BeginScrollView(rect, ref this.filtersScrollPosition, scrollViewRect);

			Rect listingRect = scrollViewRect.AtZero();
			listingRect.height = 999999;
			Listing_Standard listing = new Listing_Standard();
			listing.ColumnWidth = scrollViewRect.width;
			listing.Begin(listingRect);

			Text.Font = GameFont.Small;
			this.ListFiltersOnListing(listing, FilterLister.FiltersInCategory(FilterCategory.FromDef));

			listing.Gap();

			Text.Font = GameFont.Small;
			listing.Label("FiltersCustom".Translate());
			this.ListFiltersOnListing(listing, FilterLister.FiltersInCategory(FilterCategory.CustomLocal));

			listing.End();

			this.totalFilterListHeight = listing.CurHeight;

			Widgets.EndScrollView();
		}

		private void ListFiltersOnListing(Listing_Standard listing, IEnumerable<Filter.Filter> filters) {
			bool flag = false;
			foreach (Filter.Filter filter in filters) {
				if (filter.showInUI) {
					if (flag) { listing.Gap(); }
					Filter.Filter filter2 = filter;
					Rect rect = listing.GetRect(62);
					this.DoFilterListEntry(rect, filter2);
					flag = true;
				}
			}
			if (!flag) {
				GUI.color = new Color(1, 1, 1, 0.5f);
				listing.Label("(" + "NoneLower".Translate() + ")");
				GUI.color = Color.white;
			}
		}

		private void DoFilterListEntry(Rect rect, Filter.Filter filter) {
			bool flag = this.currentFilter == filter;

			Widgets.DrawOptionBackground(rect, flag);

			MouseoverSounds.DoRegion(rect);

			Rect rect2 = rect.ContractedBy(4);

			Text.Font = GameFont.Small;
			Rect filterNameRect = rect2;
			filterNameRect.height = Text.CalcHeight(filter.name, filterNameRect.width);
			Widgets.Label(filterNameRect, filter.name);

			Text.Font = GameFont.Tiny;
			Rect filterSummaryRect = rect2;
			filterSummaryRect.yMin = filterNameRect.yMax;
			if (!Text.TinyFontSupported) {
				filterSummaryRect.yMin -= 2;
				filterSummaryRect.height += 2;
			}
			Widgets.Label(filterSummaryRect, filter.GetSummary());

			if (!filter.enabled) { return; }

			WidgetRow widgetRow = new WidgetRow(rect.xMax, rect.y, UIDirection.LeftThenDown);

			if (filter.Category == FilterCategory.CustomLocal && widgetRow.ButtonIcon(TexButton.DeleteX, "Delete".Translate(), GenUI.SubtleMouseoverColor)) {
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(filter.File.Name), delegate {
					filter.File.Delete();
					FilterLister.MarkDirty();
				}, true));
			}

			if (!flag && Widgets.ButtonInvisible(rect)) {
				this.currentFilter = filter;
				SoundDefOf.Click.PlayOneShotOnCamera();
			}
		}

		protected override bool CanDoNext() {
			if (!base.CanDoNext()) { return false; }
			if (this.currentFilter == null) { return false; }
			SelectFilterPage.BeginFilterConfiguration(this.currentFilter, this);
			return true;
		}

		public static void BeginFilterConfiguration(Filter.Filter filter, Page originPage) {
			Filter.Filter.currentFilter = filter;
		}

		private void EnsureValidSelection() {
			if (this.currentFilter == null || !FilterLister.FilterIsListedAnywhere(this.currentFilter)) {
				this.currentFilter = FilterLister.FiltersInCategory(FilterCategory.FromDef).FirstOrDefault();
			}
		}

		internal void NotifyFilterListChanged() {
			this.currentFilter = FilterLister.AllFilters().FirstOrDefault();
			this.EnsureValidSelection();
		}

		public SelectFilterPage() {
			this.infoScrollPosition = Vector2.zero;
			this.filtersScrollPosition = Vector2.zero;
		}
	}
}
