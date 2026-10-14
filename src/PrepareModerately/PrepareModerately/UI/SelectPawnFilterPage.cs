using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Filter;

using RimWorld;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `RimWorld.Page_SelectScenario`.
	[StaticConstructorOnStartup]
	internal sealed class SelectPawnFilterPage : Page {
		private const float ExtraScrollHeight = 250;

		private const float FilterListGapHeight = 6;

		private const float FilterSelectionListMargin = 16;

		private const float FilterSelectionListScreenShare = 0.35f;

		private const float GapBetweenColumns = 17;

		private const float ListingRectHeight = 999999; // Arbitrary large number.

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly Color MinorTextColor = new Color(1, 1, 1, 0.5f);
#else
		private static readonly Color MinorTextColor = new(1, 1, 1, 0.5f);
#endif

		private const float PawnFilterListingEntryHeight = 68;

		private const float PawnFilterListingEntryMargin = 4;

		private const float PawnFilterSelectionListOverflow = 2;

#if !(V1_0 || V1_1 || V1_2)
		private const float TinyFontCorrectionMargin = 6;
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
		private const float WarningIconMargin = 3;
#endif

		public override string PageTitle =>
			"PM.ChooseFilter".Translate().CapitalizeFirst();

		private PawnFilter filter;

		private Vector2 filtersScrollPosition;

		private Vector2 infoScrollPosition;

		private float totalFilterListHeight;

		internal SelectPawnFilterPage() {
			this.infoScrollPosition = Vector2.zero;
			this.filtersScrollPosition = Vector2.zero;
		}

		internal static void BeginFilterConfiguration(PawnFilter filter) =>
			PawnFilter.Current = filter;

		private static bool CanEditFilter(PawnFilter filter) =>
			filter is null ? throw new ArgumentNullException(nameof(filter))
			: filter.PCategory == PawnFilterCategory.CustomLocal;

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

			PawnFilterUi.DrawInfo(
				new Rect(filterSelectionListRect.xMax + GapBetweenColumns, 0, mainRect.width - filterSelectionListRect.width - GapBetweenColumns, mainRect.height).Rounded(),
				this.filter,
				ref this.infoScrollPosition);

#if V1_0 || V1_1 || V1_2
			GUI.EndGroup();
#else
			Widgets.EndGroup();
#endif

			this.DoBottomButtons(inRect, null, "PM.FilterEditor".Translate().CapitalizeFirst(), this.GoToFilterEditor);
		}

		public override void PreOpen() {
			base.PreOpen();
			this.infoScrollPosition = Vector2.zero;
			PawnFilterLister.MarkDirty();
			this.EnsureValidSelection();
		}

		internal void notifyFilterListChanged() {
			this.filter = PawnFilterLister.All().FirstOrDefault((filter) => filter.GetHashCode() == this.filter.GetHashCode());
			this.EnsureValidSelection();
		}

		protected override bool CanDoNext() {
			if (!base.CanDoNext() || this.filter is null) {
				return false;
			}

			BeginFilterConfiguration(this.filter);
			return true;
		}

		private void DoFilterListEntry(Rect rect, PawnFilter filter) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (filter is null) {
				throw new ArgumentNullException(nameof(filter));
			}
#else
			ArgumentNullException.ThrowIfNull(filter);
#endif

			bool isCurrentFilter = this.filter == filter;
			if (!filter.Valid) {
#if V1_0
				TooltipHandler.TipRegion(rect, $"{"PM.FilterPartError".Translate().CapitalizeFirst()}.");
#elif V1_1 || V1_2
				TooltipHandler.TipRegion(rect, $"{"PM.FilterPartError".Translate().CapitalizeFirst()}.".Colorize(ColorLibrary.Red));
#elif V1_3 || V1_4
				TooltipHandler.TipRegion(rect, $"{"PM.FilterPartError".Translate().CapitalizeFirst()}.".Colorize(ColorLibrary.RedReadable));
#else
				TooltipHandler.TipRegion(rect, "PM.FilterPartError".Translate().CapitalizeFirst().EndWithPeriod().Colorize(ColorLibrary.RedReadable));
#endif
			}

			Widgets.DrawOptionBackground(rect, isCurrentFilter);

			MouseoverSounds.DoRegion(rect);

			Rect innerRect = rect.ContractedBy(PawnFilterListingEntryMargin);

			Text.Font = GameFont.Small;
			Rect filterNameRect = innerRect;
			filterNameRect.height = Text.CalcHeight(filter.PName, filterNameRect.width);
			Widgets.Label(filterNameRect, filter.PName);

			Text.Font = GameFont.Tiny;
			Rect filterSummaryRect = innerRect;
			filterSummaryRect.yMin = filterNameRect.yMax;
#if !(V1_0 || V1_1 || V1_2)
			if (!Text.TinyFontSupported) {
				filterSummaryRect.yMin -= TinyFontCorrectionMargin;
				filterSummaryRect.height += TinyFontCorrectionMargin;
			}
#endif
			Widgets.Label(filterSummaryRect, filter.PSummary);

			if (!filter.Enabled) {
				return;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			WidgetRow widgetRow = new WidgetRow(rect.xMax, rect.y, UIDirection.LeftThenDown);
#else
			WidgetRow widgetRow = new(rect.xMax, rect.y, UIDirection.LeftThenDown);
#endif

			if (filter.PCategory == PawnFilterCategory.CustomLocal && widgetRow.ButtonIcon(Textures.DeleteX, "PM.Delete".Translate().CapitalizeFirst(), GenUI.SubtleMouseoverColor)) {
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("PM.ReallyDeleteFile".Translate(filter.File.Name).CapitalizeFirst(), () => {
					filter.File.Delete();
					PawnFilterLister.MarkDirty();
				}, true));
			}

			if (!filter.Valid) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				widgetRow.Icon(Textures.Warning);
#else
				widgetRow.Icon(Textures.Warning, null, WarningIconMargin);
#endif
			}

			if (!isCurrentFilter && Widgets.ButtonInvisible(rect)) {
				SoundDefOf.Click.PlayOneShotOnCamera();
				if (filter.Valid) {
					this.filter = filter;
				} else {
					PreLoadUtility.CheckVersionAndLoad(filter.File.FullName, ScribeMetaHeaderUtility.ScribeHeaderMode.Scenario, () => {
						this.filter = filter;
					});
				}
			}
		}

		private void DoFilterSelectionList(Rect inRect) {
			inRect.xMax += PawnFilterSelectionListOverflow;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect scrollViewRect = new Rect(0, 0, inRect.width - FilterSelectionListMargin - PawnFilterSelectionListOverflow, this.totalFilterListHeight + ExtraScrollHeight);
#else
			Rect scrollViewRect = new(0, 0, inRect.width - FilterSelectionListMargin - PawnFilterSelectionListOverflow, this.totalFilterListHeight + ExtraScrollHeight);
#endif

			Widgets.BeginScrollView(inRect, ref this.filtersScrollPosition, scrollViewRect);

			Rect listingRect = scrollViewRect.AtZero();
			listingRect.height = ListingRectHeight;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Listing_Standard listing = new Listing_Standard {
				ColumnWidth = scrollViewRect.width
			};
#else
			Listing_Standard listing = new() {
				ColumnWidth = scrollViewRect.width
			};
#endif
			listing.Begin(listingRect);

			Text.Font = GameFont.Small;
			this.ListFiltersOnListing(listing, PawnFilterLister.InCategory(PawnFilterCategory.FromDef));

			listing.Gap();

			Text.Font = GameFont.Small;
#if V1_0
			listing.Label("PM.FiltersCustom".Translate().CapitalizeFirst());
#else
			_ = listing.Label("PM.FiltersCustom".Translate().CapitalizeFirst());
#endif
			this.ListFiltersOnListing(listing, PawnFilterLister.InCategory(PawnFilterCategory.CustomLocal));

			listing.End();
			this.totalFilterListHeight = listing.CurHeight;
			Widgets.EndScrollView();
		}

		private void EnsureValidSelection() {
			if (this.filter is null || !PawnFilterLister.FilterIsListedAnywhere(this.filter)) {
				this.filter = PawnFilterLister.InCategory(PawnFilterCategory.FromDef).FirstOrDefault();
			}
		}

		private void GoToFilterEditor() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			PawnFilterEditorPage pawnFilterEditorPage = new PawnFilterEditorPage(CanEditFilter(this.filter) ? this.filter : this.filter.CopyForEditing) {
				prev = this
			};
#else
			PawnFilterEditorPage pawnFilterEditorPage = new(CanEditFilter(this.filter) ? this.filter : this.filter.CopyForEditing) {
				prev = this
			};
#endif
			Find.WindowStack.Add(pawnFilterEditorPage);
			this.Close();
		}

		private void ListFiltersOnListing(Listing_Standard listing, IEnumerable<PawnFilter> filters) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}

			if (filters is null) {
				throw new ArgumentNullException(nameof(filters));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
			ArgumentNullException.ThrowIfNull(filters);
#endif

			bool anyFilter = false;
			foreach (PawnFilter filter in filters) {
				if (filter.ShowInUi) {
					if (anyFilter) {
						listing.Gap(FilterListGapHeight);
					}

					this.DoFilterListEntry(listing.GetRect(PawnFilterListingEntryHeight).ContractedBy(PawnFilterListingEntryMargin), filter);
					anyFilter = true;
				}
			}

			if (!anyFilter) {
				GUI.color = MinorTextColor;
#if V1_0
				listing.Label($"({"NoneLower".Translate()})".CapitalizeFirst());
#else
				_ = listing.Label($"({"NoneLower".Translate()})".CapitalizeFirst());
#endif
				GUI.color = Color.white;
			}
		}
	}
}
