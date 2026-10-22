using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `RimWorld.Page_SelectScenario`.
	[StaticConstructorOnStartup]
	public sealed class SelectPawnFilterPage : Page {
		private const float _extraScrollHeight = 250;

		private const float _filterListEntryHeight = 68;

		private const float _filterListEntryPadding = 4;

		private const float _filterListGap = 6;

		private const float _filterListHeight = 999999; // Arbitrary large number.

		private const float _filterListMargin = 16;

		private const float _filterListPadding = 2;

		private const float _filterSelectionListScreenShare = 0.35f;

		private const float _gapBetweenColumns = 17;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly Color _minorTextColor = new Color(1, 1, 1, 0.5f);
#else
		private static readonly Color _minorTextColor = new(1, 1, 1, 0.5f);
#endif

		private const float _tinyFontCorrectionMargin = 6;

		private const float _warningIconMargin = 3;

		public override string PageTitle =>
#if V1_0
			"PM.ChooseFilter".Translate()?.CapitalizeFirstSafe() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.ChooseFilter".Translate().CapitalizeFirst().StringSafe();
#else
			"PM.ChooseFilter".Translate().CapitalizeFirst().StringSafe;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private PawnFilter _filter;
#else
		private PawnFilter? _filter;
#endif

		private Vector2 _filtersScrollPosition;

		private Vector2 _infoScrollPosition;

		private float _totalFilterListHeight;

		public static void BeginFilterConfiguration(PawnFilter filter) {
			if (filter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to begin filter configuration with null filter.");
				return;
			}

			PawnFilter.Current = filter;
			PawnFilter.Current.PreConfigure();
		}

		public override void DoWindowContents(Rect inRect) {
			if (this._filter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw select pawn filter page with null selected filter.");
				return;
			}

			this.DrawPageTitle(inRect);

			Rect mainRect = this.GetMainRect(inRect).Rounded();
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

#if V1_0
			this.DoBottomButtons(inRect, null, "PM.FilterEditor".Translate()?.CapitalizeFirstSafe() ?? "null", this.GoToFilterEditor);
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this.DoBottomButtons(inRect, null, "PM.FilterEditor".Translate().CapitalizeFirst().StringSafe(), this.GoToFilterEditor);
#else
			this.DoBottomButtons(inRect, null, "PM.FilterEditor".Translate().CapitalizeFirst().StringSafe, this.GoToFilterEditor);
#endif
		}

		public override void PreOpen() {
			base.PreOpen();
			this._infoScrollPosition = Vector2.zero;
			PawnFilterLister.MarkDirty();
			this.EnsureValidSelection();
		}

		public void NotifyFilterListChanged() {
			if (this._filter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Notified of filter list change with null selected filter.");
				return;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			PawnFilter filter = PawnFilterLister.All.FirstOrDefault();
#else
			PawnFilter? filter = PawnFilterLister.All.FirstOrDefault();
#endif
			if (filter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Unable to locate filter when filter list changed.");
				return;
			}

			this._filter = filter;
			this.EnsureValidSelection();
		}

		protected override bool CanDoNext() {
			if (!base.CanDoNext()) {
				return false;
			}

			if (this._filter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to check if it was possible to proceed to the next page with null selected filter.");
				return false;
			}

			BeginFilterConfiguration(this._filter);
			return true;
		}

		private void DoFilterListEntry(Rect rect, PawnFilter filter) {
			if (filter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw list entry of null filter.");
				return;
			}

			if (Find.WindowStack is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to add to null window stack.");
				return;
			}

			if (Textures.Delete is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw null delete texture.");
				return;
			}

			if (Textures.Warning is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw null warning texture.");
				return;
			}

			if (!filter.Valid) {
#if V1_0
				TooltipHandler.TipRegion(rect, "PM.FilterPartError".Translate()?.CapitalizeFirstSafe().EndWithPeriodSafe() ?? "null");
#elif V1_1 || V1_2
				TooltipHandler.TipRegion(rect, "PM.FilterPartError".Translate().CapitalizeFirst().EndWithPeriod().StringSafe().Colorize(ColorLibrary.Red) ?? "null");
#else
				TooltipHandler.TipRegion(rect, "PM.FilterPartError".Translate().CapitalizeFirst().EndWithPeriod().Colorize(ColorLibrary.RedReadable) ?? "null");
#endif
			}

			bool currentFilterIsSelected = this._filter == filter;
			Widgets.DrawOptionBackground(rect, currentFilterIsSelected);
			MouseoverSounds.DoRegion(rect);

			Rect innerRect = rect.ContractedBy(_filterListEntryPadding).Rounded();

			Text.Font = GameFont.Small;
			string name = filter.Name ?? "null";
			Rect filterNameRect = new Rect(innerRect.x, innerRect.y, innerRect.width, Text.CalcHeight(name, innerRect.width)).Rounded();
			Widgets.Label(filterNameRect, name);

			Text.Font = GameFont.Tiny;
			Rect filterSummaryRect = new Rect(innerRect.x, filterNameRect.yMax, innerRect.width, innerRect.height - filterNameRect.height).Rounded();
#if !(V1_0 || V1_1 || V1_2)
			if (!Text.TinyFontSupported) {
				filterSummaryRect.yMin -= _tinyFontCorrectionMargin;
				filterSummaryRect.height += _tinyFontCorrectionMargin;
			}
#endif
			Widgets.Label(filterSummaryRect, filter.Summary ?? "null");

			if (!filter.Enabled) {
				return;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			WidgetRow widgetRow = new WidgetRow(rect.xMax, rect.y, UIDirection.LeftThenDown);
#else
			WidgetRow widgetRow = new(rect.xMax, rect.y, UIDirection.LeftThenDown);
#endif

#if V1_0
			if (filter.CanEdit && widgetRow.ButtonIcon(Textures.Delete, "PM.Delete".Translate()?.CapitalizeFirstSafe() ?? "null", GenUI.SubtleMouseoverColor)) {
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (filter.CanEdit && widgetRow.ButtonIcon(Textures.Delete, "PM.Delete".Translate().CapitalizeFirst().StringSafe(), GenUI.SubtleMouseoverColor)) {
#else
			if (filter.CanEdit && widgetRow.ButtonIcon(Textures.Delete, "PM.Delete".Translate().CapitalizeFirst().StringSafe, GenUI.SubtleMouseoverColor)) {
#endif
				if (filter.File is null) {
					PrepareModeratelyLogger.LogErrorMessage("Attempted to delete null file.");
					return;
				}

				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("PM.ReallyDeleteFile".Translate(filter.File.Name).CapitalizeFirst(), () => {
					filter.File.Delete();
					PawnFilterLister.MarkDirty();
				}, true));
			}

			if (!filter.Valid) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				_ = widgetRow.Icon(Textures.Warning);
#else
				_ = widgetRow.Icon(Textures.Warning, null, _warningIconMargin);
#endif
			}

			if (!currentFilterIsSelected && Widgets.ButtonInvisible(rect)) {
				SoundDefOf.Click?.PlayOneShotOnCamera();
				if (filter.Valid) {
					this._filter = filter;
				} else {
					if (filter.File is null) {
						PrepareModeratelyLogger.LogErrorMessage("Attempted to load null file.");
						return;
					}

					PreLoadUtility.CheckVersionAndLoad(filter.File.FullName, ScribeMetaHeaderUtility.ScribeHeaderMode.Scenario, () => {
						this._filter = filter;
					});
				}
			}
		}

		private void DoFilterSelectionList(Rect inRect) {
			Rect rect = new Rect(inRect.x, inRect.y, inRect.width + _filterListPadding, inRect.height).Rounded();
			Rect scrollViewRect = new Rect(0, 0, rect.width - _filterListMargin - _filterListPadding, this._totalFilterListHeight + _extraScrollHeight).Rounded();
			Widgets.BeginScrollView(rect, ref this._filtersScrollPosition, scrollViewRect);

			Rect listingRect = new Rect(0, 0, scrollViewRect.width, _filterListHeight).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Listing_Standard listing = new Listing_Standard() {
#else
			Listing_Standard listing = new() {
#endif
				ColumnWidth = scrollViewRect.width
			};
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
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				PawnFilter filter = PawnFilterLister.InCategory(PawnFilterCategory.FromDef).First();
#else
				PawnFilter? filter = PawnFilterLister.InCategory(PawnFilterCategory.FromDef).First();
#endif
				if (filter is null) {
					PrepareModeratelyLogger.LogErrorMessage("Unable to find any valid filter when ensuring valid filter selection.");
					return;
				}

				this._filter = filter;
			}
		}

		private void GoToFilterEditor() {
			if (this._filter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to go to filter editor with null selected filter.");
				return;
			}

			if (Find.WindowStack is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to add filter editor to null window stack.");
				return;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			PawnFilterEditorPage pawnFilterEditorPage = new PawnFilterEditorPage(this._filter.CanEdit ? this._filter : this._filter.CopyForEditing) {
#else
			PawnFilterEditorPage pawnFilterEditorPage = new(this._filter.CanEdit ? this._filter : this._filter.CopyForEditing) {
#endif
				prev = this,
				next = this.next // Go back to the configure starting pawns page.
			};
			Find.WindowStack.Add(pawnFilterEditorPage);
			this.Close();
		}

		private void ListFiltersOnListing(Listing_Standard listing, IEnumerable<PawnFilter> filters) {
			if (listing is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to list filters on null listing.");
				return;
			}

			if (filters is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to list null filters on listing.");
				return;
			}

			bool anyFilter = false;
			foreach (PawnFilter filter in filters) {
				if (filter.ShowInUi) {
					if (anyFilter) {
						listing.Gap(_filterListGap);
					}

					this.DoFilterListEntry(listing.GetRect(_filterListEntryHeight).ContractedBy(_filterListEntryPadding).Rounded(), filter);
					anyFilter = true;
				}
			}

			if (!anyFilter) {
				GUI.color = _minorTextColor;
#if V1_0
				listing.Label($"({"PM.None".Translate().CapitalizeFirst()})");
#else
				_ = listing.Label($"({"PM.None".Translate().CapitalizeFirst()})");
#endif
				GUI.color = Color.white;
			}
		}
	}
}
