using System.Collections.Generic;
using Lakuna.PrepareModerately.PawnFilter;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.GUI {
	// Based on RimWorld.Page_SelectScenario.
	public class Page_SelectFilter : Page {
		private Filter currentFilter;

		private Vector2 infoScrollPosition = Vector2.zero;

		private const float FilterEntryHeight = 62;

		private Vector2 filtersScrollPosition = Vector2.zero;

		private float totalFilterListHeight;

		public override string PageTitle => "ChooseFilter".Translate();

		public override void PreOpen() {
			base.PreOpen();
			this.infoScrollPosition = Vector2.zero;
			FilterLister.MarkDirty();
			this.EnsureValidSelection();
		}

		public override void DoWindowContents(Rect inRect) {
			this.DrawPageTitle(inRect);

			Rect mainRect = this.GetMainRect(inRect);
			Widgets.BeginGroup(mainRect);

			Rect rect2 = new Rect(0, 0, mainRect.width * 0.35f, mainRect.height).Rounded();
			this.DoFilterSelectionList(rect2);
			FilterUI.DrawFilterInfo(new Rect(rect2.xMax + 17, 0, mainRect.width - rect2.width - 17, mainRect.height).Rounded(), this.currentFilter, ref this.infoScrollPosition);

			Widgets.EndGroup();

			this.DoBottomButtons(inRect, null, "FilterEditor".Translate(), this.GoToFilterEditor);
		}

		private bool CanEditFilter(Filter filter) {
			return filter.Category == FilterCategory.CustomLocal;
		}

		private void GoToFilterEditor() {
			Page_FilterEditor page_FilterEditor = new Page_FilterEditor(this.CanEditFilter(this.currentFilter) ? this.currentFilter : this.currentFilter.CopyForEditing());
			page_FilterEditor.prev = this;
			Find.WindowStack.Add(page_FilterEditor);
			this.Close();
		}

		private void DoFilterSelectionList(Rect inRect) {
			inRect.xMax += 2;

			Rect rect2 = new Rect(0, 0, inRect.width - 16 - 2, this.totalFilterListHeight + 250);

			Widgets.BeginScrollView(inRect, ref this.filtersScrollPosition, rect2);

			Rect rect3 = rect2.AtZero();
			rect3.height = 999999;

			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.ColumnWidth = rect2.width;
			listing_Standard.Begin(rect3);

			Text.Font = GameFont.Small;
			this.ListFiltersOnListing(listing_Standard, FilterLister.FiltersInCategory(FilterCategory.FromDef));
			listing_Standard.Gap();

			Text.Font = GameFont.Small;
			listing_Standard.Label("FiltersCustom".Translate());
			this.ListFiltersOnListing(listing_Standard, FilterLister.FiltersInCategory(FilterCategory.CustomLocal));

			listing_Standard.End();

			this.totalFilterListHeight = listing_Standard.CurHeight;

			Widgets.EndScrollView();
		}

		private void ListFiltersOnListing(Listing_Standard listing, IEnumerable<Filter> filters) {
			bool flag = false;

			foreach (Filter filter in filters) {
				if (filter.showInUI) {
					if (flag) { listing.Gap(); }

					Filter filter2 = filter;
					Rect rect = listing.GetRect(62);
					this.DoFilterListEntry(rect, filter2);

					flag = true;
				}
			}

			if (!flag) {
				UnityEngine.GUI.color = new Color(1, 1, 1, 0.5f);
				listing.Label("(" + "NoneLower".Translate() + ")");
				UnityEngine.GUI.color = Color.white;
			}
		}

		private void DoFilterListEntry(Rect inRect, Filter filter) {
			bool flag = this.currentFilter == filter;

			Widgets.DrawOptionBackground(inRect, flag);

			MouseoverSounds.DoRegion(inRect);

			Rect rect2 = inRect.ContractedBy(4);

			Text.Font = GameFont.Small;
			Rect rect3 = rect2;
			rect3.height = Text.CalcHeight(filter.name, rect3.width);
			Widgets.Label(rect3, filter.name);

			Text.Font = GameFont.Tiny;
			Rect rect4 = rect2;
			rect4.yMin = rect3.yMax;
			if (!Text.TinyFontSupported) {
				rect4.yMin -= 2;
				rect4.height += 2;
			}
			Widgets.Label(rect4, filter.GetSummary());

			if (!filter.enabled) { return; }

			WidgetRow widgetRow = new WidgetRow(inRect.xMax, inRect.y, UIDirection.LeftThenDown);

			if (filter.Category == FilterCategory.CustomLocal && widgetRow.ButtonIcon(TexButton.DeleteX, "Delete".Translate(), GenUI.SubtleMouseoverColor)) {
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(filter.File.Name), delegate {
					filter.File.Delete();
					FilterLister.MarkDirty();
				}, destructive: true));
			}

			if (!flag && Widgets.ButtonInvisible(inRect)) {
				this.currentFilter = filter;
				SoundDefOf.Click.PlayOneShotOnCamera();
			}
		}

		// TODO: Review differences between Page_SelectScenario.CanDoNext and Page_SelectFilter.CanDoNext.
		protected override bool CanDoNext() {
			if (!base.CanDoNext()) { return false; }
			if (this.currentFilter == null) { return false; }
			Page_SelectFilter.BeginFilterConfiguration(this.currentFilter, this);
			return true;
		}

		public static void BeginFilterConfiguration(Filter filter, Page originPage) {
			PrepareModeratelyMod.currentFilter = filter;
			PrepareModeratelyMod.currentFilter.PreConfigure();
			originPage.next = PrepareModeratelyMod.page_ConfigureStartingPawns;
		}

		private void EnsureValidSelection() {
			if (this.currentFilter == null || !FilterLister.FilterIsListedAnywhere(this.currentFilter)) {
				this.currentFilter = FilterLister.FiltersInCategory(FilterCategory.FromDef).FirstOrDefault();
			}
		}
	}
}
