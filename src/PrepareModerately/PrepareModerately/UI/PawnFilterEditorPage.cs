#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
using System.Collections.Generic;
#endif
using System.Linq;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `RimWorld.Page_ScenarioEditor`.
	public sealed class PawnFilterEditorPage : Page {
		private const float _configControlsColumnWidth = 200;

		private const float _configControlsScreenShare = 0.35f;

		private const float _gapBetweenColumns = 17;

		public override string PageTitle =>
			$"{"PM.FilterEditor".Translate().CapitalizeFirst()}{(this._filterEdited ? "*" : "")}";

		private bool _editMode;

		private PawnFilter _filter;

		private bool _filterEdited =>
			this._editMode && this._filter.GetHashCode() != this._initialHash;

		private Vector2 _infoScrollPosition;

		private readonly int _initialHash;

		private bool _needsSave = true;

		private string _seed = "";

		private bool _seedIsValid;

		public PawnFilterEditorPage(PawnFilter filter) {
			this._filter = filter;
			this._initialHash = this._filter.GetHashCode();
		}

		private static bool CheckAllPartsCompatible(PawnFilter filter) {
			if (filter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to check compatibility of all parts of null filter.");
				return false;
			}

			if (filter.Parts is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to check compatibility of all parts in filter with null parts.");
				return false;
			}

			if (MessageTypeDefOf.RejectInput is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to use null definition for reject input message type.");
				return false;
			}

			foreach (PawnFilterPart part in filter.Parts) {
				int num = 0;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				foreach (PawnFilterPart part2 in filter.Parts ?? new List<PawnFilterPart>()) {
#else
				foreach (PawnFilterPart part2 in filter.Parts ?? []) {
#endif
					if (part2.Def == part.Def) {
						num++;
					}

					if (num > (part.Def?.MaxUses ?? 0)) {
#if V1_0
						Messages.Message("PM.TooManyOfFilterPart".Translate()?.CapitalizeFirstSafe().EndWithPeriodSafe() ?? "null", MessageTypeDefOf.RejectInput, false);
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
						Messages.Message("PM.TooManyOfFilterPart".Translate().CapitalizeFirst().EndWithPeriod().StringSafe(), MessageTypeDefOf.RejectInput, false);
#else
						Messages.Message("PM.TooManyOfFilterPart".Translate().CapitalizeFirst().EndWithPeriod().StringSafe, MessageTypeDefOf.RejectInput, false);
#endif
						return false;
					}

					if (part != part2 && !part.CanCoexistWith(part2)) {
#if V1_0
						Messages.Message("PM.IncompatibleFilterPart".Translate()?.CapitalizeFirstSafe().EndWithPeriodSafe() ?? "null", MessageTypeDefOf.RejectInput, false);
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
						Messages.Message("PM.IncompatibleFilterPart".Translate().CapitalizeFirst().EndWithPeriod().StringSafe(), MessageTypeDefOf.RejectInput, false);
#else
						Messages.Message("PM.IncompatibleFilterPart".Translate().CapitalizeFirst().EndWithPeriod().StringSafe, MessageTypeDefOf.RejectInput, false);
#endif
						return false;
					}
				}
			}

			return true;
		}

		public override void DoWindowContents(Rect inRect) {
			this.DrawPageTitle(inRect);

			Rect mainRect = this.GetMainRect(inRect).Rounded();
#if V1_0 || V1_1 || V1_2
			GUI.BeginGroup(mainRect);
#else
			Widgets.BeginGroup(mainRect);
#endif

			Rect configControlsRect = new Rect(0, 0, mainRect.width * _configControlsScreenShare, mainRect.height).Rounded();
			this.DoConfigControls(configControlsRect);

			Rect filterEditRect = new Rect(configControlsRect.xMax + _gapBetweenColumns, 0, mainRect.width - configControlsRect.width - _gapBetweenColumns, mainRect.height).Rounded();
			if (this._editMode) {
				PawnFilterUi.DrawEditInterface(filterEditRect, this._filter, ref this._infoScrollPosition);
			} else {
				PawnFilterUi.DrawInfo(filterEditRect, this._filter, ref this._infoScrollPosition);
			}

#if V1_0 || V1_1 || V1_2
			GUI.EndGroup();
#else
			Widgets.EndGroup();
#endif

			this.DoBottomButtons(inRect);
		}

		public override void PreOpen() {
			base.PreOpen();
			this._infoScrollPosition = Vector2.zero;
			this._needsSave = true;
		}

		protected override bool CanDoNext() {
			if (!base.CanDoNext() || !CheckAllPartsCompatible(this._filter)) {
				return false;
			}

			if (Find.WindowStack is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to add window to null stack.");
				return false;
			}

			if (this._filterEdited && this._needsSave) {
				Find.WindowStack.Add(new Dialog_MessageBox(
#if V1_0
					"PM.FilterSavePrompt".Translate()?.CapitalizeSentencesSafe() ?? "null",
					"Yes".Translate()?.CapitalizeFirstSafe() ?? "null",
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					"PM.FilterSavePrompt".Translate().StringSafe().CapitalizeSentencesSafe(),
					"Yes".Translate().CapitalizeFirst().StringSafe(),
#else
					"PM.FilterSavePrompt".Translate().StringSafe.CapitalizeSentencesSafe,
					"Yes".Translate().CapitalizeFirst().StringSafe,
#endif
					() => {
						if (Find.WindowStack is null) {
							PrepareModeratelyLogger.LogErrorMessage("Attempted to add window to null stack.");
							return;
						}

						Find.WindowStack.Add(new PawnFilterListSaveDialog(this._filter, () => {
							SelectPawnFilterPage.BeginFilterConfiguration(this._filter);
							this._needsSave = false;
							this.DoNext();
						}));
					},
#if V1_0
					"No".Translate()?.CapitalizeFirstSafe() ?? "null",
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					"No".Translate().CapitalizeFirst().StringSafe(),
#else
					"No".Translate().CapitalizeFirst().StringSafe,
#endif
					() => {
						SelectPawnFilterPage.BeginFilterConfiguration(this._filter);
						this._needsSave = false;
						this.DoNext();
					}
				));
				return false;
			}

			SelectPawnFilterPage.BeginFilterConfiguration(this._filter);
			return true;
		}

		private void AddFilterPart(PawnFilterPartDef def) {
			PawnFilterPart part = PawnFilterMaker.MakeFilterPart(def);
			part.Randomize();
			this._filter.AddPart(part);
		}

		private void DoConfigControls(Rect inRect) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Listing_Standard listing = new Listing_Standard() {
#else
			Listing_Standard listing = new() {
#endif
				ColumnWidth = _configControlsColumnWidth
			};
			listing.Begin(inRect);

			if (Find.WindowStack is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to add window to null stack.");
				return;
			}

#if V1_0
			if (listing.ButtonText("PM.Load".Translate()?.CapitalizeFirstSafe() ?? "null")) {
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing.ButtonText("PM.Load".Translate().CapitalizeFirst().StringSafe())) {
#else
			if (listing.ButtonText("PM.Load".Translate().CapitalizeFirst().StringSafe)) {
#endif
				Find.WindowStack.Add(new PawnFilterListLoadDialog((filter) => {
					this._filter = filter;
					this._seedIsValid = false;
				}));
			}

#if V1_0
			if (listing.ButtonText("PM.Save".Translate()?.CapitalizeFirstSafe() ?? "null")
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing.ButtonText("PM.Save".Translate().CapitalizeFirst().StringSafe())
#else
			if (listing.ButtonText("PM.Save".Translate().CapitalizeFirst().StringSafe)
#endif
				&& CheckAllPartsCompatible(this._filter)) {
				Find.WindowStack.Add(new PawnFilterListSaveDialog(this._filter));
			}

#if V1_0
			if (listing.ButtonText("PM.RandomizeSeed".Translate()?.CapitalizeFirstSafe() ?? "null")) {
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing.ButtonText("PM.RandomizeSeed".Translate().CapitalizeFirst().StringSafe())) {
#else
			if (listing.ButtonText("PM.RandomizeSeed".Translate().CapitalizeFirst().StringSafe)) {
#endif
				SoundDefOf.Tick_Tiny?.PlayOneShotOnCamera();
				this.RandomizeSeedAndFilter();
				this._seedIsValid = true;
			}

			if (this._seedIsValid) {
#if V1_0
				listing.Label("PM.Seed".Translate().CapitalizeFirst());
#else
				_ = listing.Label("PM.Seed".Translate().CapitalizeFirst());
#endif
				string text = listing.TextEntry(this._seed) ?? "";
				if (text != this._seed) {
					this._seed = text;
					this._filter = PawnFilterMaker.Random(this._seed);
				}
			} else {
				listing.Gap(Text.LineHeight * 2 + 2);
			}

#if V1_0
			listing.CheckboxLabeled("PM.EditMode".Translate()?.CapitalizeFirstSafe() ?? "null", ref this._editMode);
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			listing.CheckboxLabeled("PM.EditMode".Translate().CapitalizeFirst().StringSafe(), ref this._editMode);
#else
			listing.CheckboxLabeled("PM.EditMode".Translate().CapitalizeFirst().StringSafe, ref this._editMode);
#endif

			if (this._editMode) {
				this._seedIsValid = false;
#if V1_0
				if (listing.ButtonText("PM.AddPart".Translate()?.CapitalizeFirstSafe() ?? "null")) {
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				if (listing.ButtonText("PM.AddPart".Translate().CapitalizeFirst().StringSafe())) {
#else
				if (listing.ButtonText("PM.AddPart".Translate().CapitalizeFirst().StringSafe)) {
#endif
					this.OpenAddFilterPartMenu();
				}
			}

			listing.End();
		}

		private void OpenAddFilterPartMenu() =>
			FloatMenuUtility.MakeMenu(
				PawnFilterMaker.AddableParts(this._filter).Where((part) => part.PlayerAddRemovable && part.Category != PawnFilterPartCategory.Obsolete).OrderBy((part) => part.label),
#if V1_0
				(def) => def.LabelCap ?? "null",
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				(def) => def.LabelCap.StringSafe(),
#else
				(def) => def.LabelCap.StringSafe,
#endif
				(def) => () => this.AddFilterPart(def));

		private void RandomizeSeedAndFilter() {
			this._seed = GenText.RandomSeedString() ?? "";
			this._filter = PawnFilterMaker.Random(this._seed);
		}
	}
}
