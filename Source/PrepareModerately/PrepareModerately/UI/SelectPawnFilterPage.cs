using Lakuna.PrepareModerately.Filter;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.UI {
	[StaticConstructorOnStartup]
	public class SelectPawnFilterPage : Page {
		private PawnFilter filter;

		private Vector2 infoScrollPosition;

		private Vector2 filtersScrollPosition;

		private float totalFilterListHeight;

		private const float FilterSelectionListScreenShare = 0.35f;

		private const float FilterSelectionListMargin = 16;

		private const float GapBetweenColumns = 17;

		private const float PawnFilterSelectionListOverflow = 2;

		private const float ExtraScrollHeight = 250;

		private const float ListingRectHeight = 99999; // Arbitrary large number.

		private const float PawnFilterListingEntryHeight = 62;

		private const float PawnFilterListingEntryMargin = 4;

		private static readonly Color MinorTextColor = new Color(1, 1, 1, 0.5f);

#if !(V1_0 || V1_1 || V1_2)
		private const float TinyFontCorrectionMargin = 2;
#endif

		public override string PageTitle => "ChooseFilter".Translate().CapitalizeFirst();

		public override void PreOpen() {
			base.PreOpen();
			this.infoScrollPosition = Vector2.zero;
			PawnFilterLister.MarkDirty();
			this.EnsureValidSelection();
		}

		public override void DoWindowContents(Rect inRect) {
			this.DrawPageTitle(inRect);

			Rect mainRect = this.GetMainRect(inRect);
#if V1_0 || V1_1 || V1_2
			GUI.BeginGroup(mainRect);
#else
			Widgets.BeginGroup(mainRect);
#endif

			Rect filterSelectionListRect = new Rect(0, 0, mainRect.width * FilterSelectionListScreenShare, mainRect.height).Rounded();
			this.DoFilterSelectionList(filterSelectionListRect);

			PawnFilterUI.DrawInfo(
				new Rect(filterSelectionListRect.xMax + GapBetweenColumns, 0, mainRect.width - filterSelectionListRect.width - GapBetweenColumns, mainRect.height).Rounded(),
				this.filter,
				ref this.infoScrollPosition);

#if V1_0 || V1_1 || V1_2
			GUI.EndGroup();
#else
			Widgets.EndGroup();
#endif

			this.DoBottomButtons(inRect, null, "FilterEditor".Translate().CapitalizeFirst(), this.GoToFilterEditor);
		}

		private static bool CanEditFilter(PawnFilter filter) => filter.Category == PawnFilterCategory.CustomLocal;

		private void GoToFilterEditor() {
			PawnFilterEditorPage pawnFilterEditorPage = new PawnFilterEditorPage(CanEditFilter(this.filter) ? this.filter : this.filter.CopyForEditing) {
				prev = this
			};
			Find.WindowStack.Add(pawnFilterEditorPage);
			this.Close();
		}

		private void DoFilterSelectionList(Rect inRect) {
			inRect.xMax += PawnFilterSelectionListOverflow;

			Rect scrollViewRect = new Rect(0, 0, inRect.width - FilterSelectionListMargin - PawnFilterSelectionListOverflow, this.totalFilterListHeight + ExtraScrollHeight);
			Widgets.BeginScrollView(inRect, ref this.filtersScrollPosition, scrollViewRect);

			Rect listingRect = scrollViewRect.AtZero();
			listingRect.height = ListingRectHeight;
			Listing_Standard listing = new Listing_Standard { ColumnWidth = scrollViewRect.width };
			listing.Begin(listingRect);

			Text.Font = GameFont.Small;
			this.ListFiltersOnListing(listing, PawnFilterLister.InCategory(PawnFilterCategory.FromDef));

			listing.Gap();

			Text.Font = GameFont.Small;
#if V1_0
			listing.Label("FiltersCustom".Translate().CapitalizeFirst());
#else
			_ = listing.Label("FiltersCustom".Translate().CapitalizeFirst());
#endif
			this.ListFiltersOnListing(listing, PawnFilterLister.InCategory(PawnFilterCategory.CustomLocal));

			listing.End();
			this.totalFilterListHeight = listing.CurHeight;
			Widgets.EndScrollView();
		}

		private void ListFiltersOnListing(Listing_Standard listing, IEnumerable<PawnFilter> filters) {
			bool flag = false;
			foreach (PawnFilter filter in filters) {
				if (filter.ShowInUi) {
					if (flag) {
						listing.Gap();
					}

					PawnFilter filter2 = filter;
					Rect rect = listing.GetRect(PawnFilterListingEntryHeight);
					this.DoFilterListEntry(rect, filter2);
					flag = true;
				}
			}

			if (!flag) {
				GUI.color = MinorTextColor;
#if V1_0
				listing.Label(("(" + "NoneLower".Translate() + ")").CapitalizeFirst());
#else
				_ = listing.Label(("(" + "NoneLower".Translate() + ")").CapitalizeFirst());
#endif
				GUI.color = Color.white;
			}
		}

		private void DoFilterListEntry(Rect rect, PawnFilter filter) {
			bool flag = this.filter == filter;

			Widgets.DrawOptionBackground(rect, flag);

			MouseoverSounds.DoRegion(rect);

			Rect rect2 = rect.ContractedBy(PawnFilterListingEntryMargin);

			Text.Font = GameFont.Small;
			Rect filterNameRect = rect2;
			filterNameRect.height = Text.CalcHeight(filter.Name, filterNameRect.width);
			Widgets.Label(filterNameRect, filter.Name);

			Text.Font = GameFont.Tiny;
			Rect filterSummaryRect = rect2;
			filterSummaryRect.yMin = filterNameRect.yMax;
#if !(V1_0 || V1_1 || V1_2)
			if (!Text.TinyFontSupported) {
				filterSummaryRect.yMin -= TinyFontCorrectionMargin;
				filterSummaryRect.height += TinyFontCorrectionMargin;
			}
#endif
			Widgets.Label(filterSummaryRect, filter.Summary);

			if (!filter.Enabled) {
				return;
			}

			WidgetRow widgetRow = new WidgetRow(rect.xMax, rect.y, UIDirection.LeftThenDown);

			if (filter.Category == PawnFilterCategory.CustomLocal && widgetRow.ButtonIcon(Textures.DeleteX, "Delete".Translate().CapitalizeFirst(), GenUI.SubtleMouseoverColor)) {
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(filter.File.Name).CapitalizeFirst(), () => {
					filter.File.Delete();
					PawnFilterLister.MarkDirty();
				}, true));
			}

			if (!flag && Widgets.ButtonInvisible(rect)) {
				this.filter = filter;
				SoundDefOf.Click.PlayOneShotOnCamera();
			}
		}

		protected override bool CanDoNext() {
			if (!base.CanDoNext()) {
				return false;
			}

			if (this.filter == null) {
				return false;
			}

			BeginFilterConfiguration(this.filter);
			return true;
		}

		public static void BeginFilterConfiguration(PawnFilter filter) => PawnFilter.Current = filter;

		private void EnsureValidSelection() {
			if (this.filter == null || !PawnFilterLister.FilterIsListedAnywhere(this.filter)) {
				this.filter = PawnFilterLister.InCategory(PawnFilterCategory.FromDef).FirstOrDefault();
			}
		}

		public void NotifyFilterListChanged() {
			this.filter = PawnFilterLister.All().FirstOrDefault();
			this.EnsureValidSelection();
		}

		public SelectPawnFilterPage() {
			this.infoScrollPosition = Vector2.zero;
			this.filtersScrollPosition = Vector2.zero;
		}
	}
}
