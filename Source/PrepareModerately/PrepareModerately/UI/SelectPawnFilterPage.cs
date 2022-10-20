using Lakuna.PrepareModerately.Filter;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.UI {
	public class SelectPawnFilterPage : Page {
		private PawnFilter filter;

		private Vector2 infoScrollPosition;

		private Vector2 filtersScrollPosition;

		private float totalFilterListHeight;

		private const float filterSelectionListScreenShare = 0.35f;

		private const float filterSelectionListMargin = 16;

		private const float gapBetweenColumns = 17;

		private const float pawnFilterSelectionListOverflow = 2;

		private const float extraScrollHeight = 250;

		private const float listingRectHeight = 99999; // Arbitrary large number.

		private const float pawnFilterListingEntryHeight = 62;

		private const float pawnFilterListingEntryMargin = 4;

		private static readonly Color minorTextColor = new Color(1, 1, 1, 0.5f);

		private const float tinyFontCorrectionMargin = 2;

		private static readonly Texture2D deleteXTexture = ContentFinder<Texture2D>.Get("UI/Widgets/Delete"); // TODO: TexButton.DeleteX

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
			GUI.BeginGroup(mainRect); // TODO [1.?,): Widgets.BeginGroup

			Rect filterSelectionListRect = new Rect(0, 0, mainRect.width * filterSelectionListScreenShare, mainRect.height);
			this.DoFilterSelectionList(filterSelectionListRect);

			PawnFilterUI.DrawInfo(new Rect(filterSelectionListRect.xMax + gapBetweenColumns, 0,
				mainRect.width - filterSelectionListRect.width - gapBetweenColumns, mainRect.height).Rounded(),
				this.filter, ref this.infoScrollPosition);

			GUI.EndGroup(); // TODO [1.?,): Widgets.EndGroup

			this.DoBottomButtons(inRect, null, "FilterEditor".Translate().CapitalizeFirst(), this.GoToFilterEditor);
		}

		private static bool CanEditFilter(PawnFilter filter) => filter.Category == PawnFilterCategory.CustomLocal;

		private void GoToFilterEditor() {
			PawnFilterEditorPage pawnFilterEditorPage = new PawnFilterEditorPage(CanEditFilter(this.filter) ? this.filter : this.filter.CopyForEditing) { prev = this };
			Find.WindowStack.Add(pawnFilterEditorPage);
			this.Close();
		}

		private void DoFilterSelectionList(Rect inRect) {
			inRect.xMax += pawnFilterSelectionListOverflow;

			Rect scrollViewRect = new Rect(0, 0, inRect.width - filterSelectionListMargin - pawnFilterSelectionListOverflow,
				this.totalFilterListHeight + extraScrollHeight);
			Widgets.BeginScrollView(inRect, ref this.filtersScrollPosition, scrollViewRect);

			Rect listingRect = scrollViewRect.AtZero();
			listingRect.height = listingRectHeight;
			Listing_Standard listing = new Listing_Standard { ColumnWidth = scrollViewRect.width };
			listing.Begin(listingRect);

			Text.Font = GameFont.Small;
			this.ListFiltersOnListing(listing, PawnFilterLister.InCategory(PawnFilterCategory.FromDef));

			listing.Gap();

			Text.Font = GameFont.Small;
			listing.Label("FiltersCustom".Translate().CapitalizeFirst());
			this.ListFiltersOnListing(listing, PawnFilterLister.InCategory(PawnFilterCategory.CustomLocal));

			listing.End();
			this.totalFilterListHeight = listing.CurHeight;
			Widgets.EndScrollView();
		}

		private void ListFiltersOnListing(Listing_Standard listing, IEnumerable<PawnFilter> filters) {
			bool flag = false;
			foreach (PawnFilter filter in filters) {
				if (filter.ShowInUi) {
					if (flag) { listing.Gap(); }
					PawnFilter filter2 = filter;
					Rect rect = listing.GetRect(pawnFilterListingEntryHeight);
					this.DoFilterListEntry(rect, filter2);
					flag = true;
				}
			}
			if (!flag) {
				GUI.color = minorTextColor;
				listing.Label(("(" + "NoneLower".Translate() + ")").CapitalizeFirst());
				GUI.color = Color.white;
			}
		}

		private void DoFilterListEntry(Rect rect, PawnFilter filter) {
			bool flag = this.filter == filter;

			Widgets.DrawOptionBackground(rect, flag);

			MouseoverSounds.DoRegion(rect);

			Rect rect2 = rect.ContractedBy(pawnFilterListingEntryMargin);

			Text.Font = GameFont.Small;
			Rect filterNameRect = rect2;
			filterNameRect.height = Text.CalcHeight(filter.Name, filterNameRect.width);
			Widgets.Label(filterNameRect, filter.Name);

			Text.Font = GameFont.Tiny;
			Rect filterSummaryRect = rect2;
			filterSummaryRect.yMin = filterNameRect.yMax;
			// TODO [1.?,): if (Text.TinyFontSupported) {
			filterSummaryRect.yMin -= tinyFontCorrectionMargin;
			filterSummaryRect.height += tinyFontCorrectionMargin;
			// TODO [1.?,): }
			Widgets.Label(filterSummaryRect, filter.Summary);

			if (!filter.Enabled) { return; }

			WidgetRow widgetRow = new WidgetRow(rect.xMax, rect.y, UIDirection.LeftThenDown);

			if (filter.Category == PawnFilterCategory.CustomLocal && widgetRow.ButtonIcon(deleteXTexture, "Delete".Translate().CapitalizeFirst(),
				GenUI.SubtleMouseoverColor)) {
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(filter.File.Name).CapitalizeFirst(), delegate {
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
			if (!base.CanDoNext()) { return false; }
			if (this.filter == null) { return false; }
			BeginFilterConfiguration(this.filter);
			return true;
		}

		public static void BeginFilterConfiguration(PawnFilter filter) {
			PawnFilter.Current = filter;
		}

		private void EnsureValidSelection() {
			if (this.filter == null || !PawnFilterLister.FilterIsListedAnywhere(this.filter)) {
				this.filter = PawnFilterLister.InCategory(PawnFilterCategory.FromDef).FirstOrDefault();
			}
		}

		internal void NotifyFilterListChanged() {
			this.filter = PawnFilterLister.All().FirstOrDefault();
			this.EnsureValidSelection();
		}

		public SelectPawnFilterPage() {
			this.infoScrollPosition = Vector2.zero;
			this.filtersScrollPosition = Vector2.zero;
		}
	}
}
