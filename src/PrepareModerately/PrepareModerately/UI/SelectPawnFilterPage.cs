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
		private const float _extraScrollHeight = 250;

		private const float _filterListGapHeight = 6;

		private const float _filterSelectionListMargin = 16;

		private const float _filterSelectionListScreenShare = 0.35f;

		private const float _gapBetweenColumns = 17;

		private const float _listingRectHeight = 999999; // Arbitrary large number.

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly Color _minorTextColor = new Color(1, 1, 1, 0.5f);
#else
		private static readonly Color _minorTextColor = new(1, 1, 1, 0.5f);
#endif

		private const float _pawnFilterListingEntryHeight = 68;

		private const float _pawnFilterListingEntryMargin = 4;

		private const float _pawnFilterSelectionListOverflow = 2;

#if !(V1_0 || V1_1 || V1_2)
		private const float _tinyFontCorrectionMargin = 6;
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
		private const float _warningIconMargin = 3;
#endif

		public override string PageTitle =>
			"PM.ChooseFilter".Translate().CapitalizeFirst();

		private PawnFilter _filter {
			get; set;
		}

		private float _totalFilterListHeight {
			get; set;
		}

		private Vector2 _filtersScrollPosition;

		private Vector2 _infoScrollPosition;

		internal SelectPawnFilterPage() {
			this._infoScrollPosition = Vector2.zero;
			this._filtersScrollPosition = Vector2.zero;
		}

		public override void DoWindowContents(Rect inRect) {
			this.DrawPageTitle(inRect);

			Rect mainRect = this.GetMainRect(inRect);
#if V1_0 || V1_1 || V1_2
			GUI.BeginGroup(mainRect);
#else
			Widgets.BeginGroup(mainRect);
#endif

			Rect filterSelectionListRect = new Rect(0, 0, mainRect.width * _filterSelectionListScreenShare, mainRect.height).Rounded();
			this.DoFilterSelectionList(filterSelectionListRect);

			PawnFilterUi.DrawInfo(
				new Rect(filterSelectionListRect.xMax + _gapBetweenColumns, 0, mainRect.width - filterSelectionListRect.width - _gapBetweenColumns, mainRect.height).Rounded(),
				this._filter,
				ref this._infoScrollPosition);

#if V1_0 || V1_1 || V1_2
			GUI.EndGroup();
#else
			Widgets.EndGroup();
#endif

			this.DoBottomButtons(inRect, null, "PM.FilterEditor".Translate().CapitalizeFirst(), this.GoToFilterEditor);
		}

		public override void PreOpen() {
			base.PreOpen();
			this._infoScrollPosition = Vector2.zero;
			PawnFilterLister.MarkDirty();
			this.EnsureValidSelection();
		}

		internal static void BeginFilterConfiguration(PawnFilter filter) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (filter is null) {
				throw new ArgumentNullException(nameof(filter));
			}
#else
			ArgumentNullException.ThrowIfNull(filter);
#endif

			PawnFilter._current = filter;
			PawnFilter._current.PreConfigure();
		}

		internal void NotifyFilterListChanged() {
			this._filter = PawnFilterLister.All().FirstOrDefault((filter) => filter.GetHashCode() == this._filter.GetHashCode());
			this.EnsureValidSelection();
		}

		protected override bool CanDoNext() {
			if (!base.CanDoNext() || this._filter is null) {
				return false;
			}

			BeginFilterConfiguration(this._filter);
			return true;
		}

		private static bool CanEditFilter(PawnFilter filter) =>
			filter is null ? throw new ArgumentNullException(nameof(filter))
			: filter._category == PawnFilterCategory.CustomLocal;

		private void DoFilterListEntry(Rect rect, PawnFilter filter) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (filter is null) {
				throw new ArgumentNullException(nameof(filter));
			}
#else
			ArgumentNullException.ThrowIfNull(filter);
#endif

			bool isCurrentFilter = this._filter == filter;
			if (!filter._valid) {
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

			Rect innerRect = rect.ContractedBy(_pawnFilterListingEntryMargin);

			Text.Font = GameFont.Small;
			Rect filterNameRect = innerRect;
			filterNameRect.height = Text.CalcHeight(filter.name, filterNameRect.width);
			Widgets.Label(filterNameRect, filter.name);

			Text.Font = GameFont.Tiny;
			Rect filterSummaryRect = innerRect;
			filterSummaryRect.yMin = filterNameRect.yMax;
#if !(V1_0 || V1_1 || V1_2)
			if (!Text.TinyFontSupported) {
				filterSummaryRect.yMin -= _tinyFontCorrectionMargin;
				filterSummaryRect.height += _tinyFontCorrectionMargin;
			}
#endif
			Widgets.Label(filterSummaryRect, filter.summary);

			if (!filter._enabled) {
				return;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			WidgetRow widgetRow = new WidgetRow(rect.xMax, rect.y, UIDirection.LeftThenDown);
#else
			WidgetRow widgetRow = new(rect.xMax, rect.y, UIDirection.LeftThenDown);
#endif

			if (filter._category == PawnFilterCategory.CustomLocal && widgetRow.ButtonIcon(Textures._delete, "PM.Delete".Translate().CapitalizeFirst(), GenUI.SubtleMouseoverColor)) {
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("PM.ReallyDeleteFile".Translate(filter._file.Name).CapitalizeFirst(), () => {
					filter._file.Delete();
					PawnFilterLister.MarkDirty();
				}, true));
			}

			if (!filter._valid) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				widgetRow.Icon(Textures._warning);
#else
				widgetRow.Icon(Textures._warning, null, _warningIconMargin);
#endif
			}

			if (!isCurrentFilter && Widgets.ButtonInvisible(rect)) {
				SoundDefOf.Click.PlayOneShotOnCamera();
				if (filter._valid) {
					this._filter = filter;
				} else {
					PreLoadUtility.CheckVersionAndLoad(filter._file.FullName, ScribeMetaHeaderUtility.ScribeHeaderMode.Scenario, () => {
						this._filter = filter;
					});
				}
			}
		}

		private void DoFilterSelectionList(Rect inRect) {
			inRect.xMax += _pawnFilterSelectionListOverflow;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect scrollViewRect = new Rect(0, 0, inRect.width - _filterSelectionListMargin - _pawnFilterSelectionListOverflow, this._totalFilterListHeight + _extraScrollHeight);
#else
			Rect scrollViewRect = new(0, 0, inRect.width - _filterSelectionListMargin - _pawnFilterSelectionListOverflow, this._totalFilterListHeight + _extraScrollHeight);
#endif

			Widgets.BeginScrollView(inRect, ref this._filtersScrollPosition, scrollViewRect);

			Rect listingRect = scrollViewRect.AtZero();
			listingRect.height = _listingRectHeight;
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
			this._totalFilterListHeight = listing.CurHeight;
			Widgets.EndScrollView();
		}

		private void EnsureValidSelection() {
			if (this._filter is null || !PawnFilterLister.FilterIsListedAnywhere(this._filter)) {
				this._filter = PawnFilterLister.InCategory(PawnFilterCategory.FromDef).FirstOrDefault();
			}
		}

		private void GoToFilterEditor() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			PawnFilterEditorPage pawnFilterEditorPage = new PawnFilterEditorPage(CanEditFilter(this._filter) ? this._filter : this._filter._copyForEditing) {
				prev = this,
				next = this.next // Go back to the configure starting pawns page.
			};
#else
			PawnFilterEditorPage pawnFilterEditorPage = new(CanEditFilter(this._filter) ? this._filter : this._filter._copyForEditing) {
				prev = this,
				next = this.next // Go back to the configure starting pawns page.
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
				if (filter._showInUi) {
					if (anyFilter) {
						listing.Gap(_filterListGapHeight);
					}

					this.DoFilterListEntry(listing.GetRect(_pawnFilterListingEntryHeight).ContractedBy(_pawnFilterListingEntryMargin), filter);
					anyFilter = true;
				}
			}

			if (!anyFilter) {
				GUI.color = _minorTextColor;
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
