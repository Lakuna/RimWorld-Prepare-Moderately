using System.Collections.Generic;
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
			$"{"PM.FilterEditor".TranslateFull(CapitalizeType.First)}{(this._filterEdited ? "*" : "")}";

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
				List<PawnFilterPart> empty = new List<PawnFilterPart>();
#else
				List<PawnFilterPart> empty = [];
#endif
				foreach (PawnFilterPart part2 in filter.Parts ?? empty) {
					if (part2.Def == part.Def) {
						num++;
					}

					if (num > (part.Def?.MaxUses ?? 0)) {
						Messages.Message("PM.TooManyOfFilterPart".TranslateFull(CapitalizeType.First, true), MessageTypeDefOf.RejectInput, false);
						return false;
					}

					if (part != part2 && !part.CanCoexistWith(part2)) {
						Messages.Message("PM.IncompatibleFilterPart".TranslateFull(CapitalizeType.First, true), MessageTypeDefOf.RejectInput, false);
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
					"PM.FilterSavePrompt".TranslateFull(CapitalizeType.Sentences),
					"Yes".TranslateFull(CapitalizeType.First),
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
					"No".TranslateFull(CapitalizeType.First),
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
			Listing_Standard listing = new Listing_Standard()
#else
			Listing_Standard listing = new()
#endif
			{
				ColumnWidth = _configControlsColumnWidth
			};
			listing.Begin(inRect);

			if (Find.WindowStack is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to add window to null stack.");
				return;
			}

			if (listing.ButtonText("PM.Load".TranslateFull(CapitalizeType.First))) {
				Find.WindowStack.Add(new PawnFilterListLoadDialog((filter) => {
					this._filter = filter;
					this._seedIsValid = false;
				}));
			}

			if (listing.ButtonText("PM.Save".TranslateFull(CapitalizeType.First))
				&& CheckAllPartsCompatible(this._filter)) {
				Find.WindowStack.Add(new PawnFilterListSaveDialog(this._filter));
			}

			if (listing.ButtonText("PM.RandomizeSeed".TranslateFull(CapitalizeType.First))) {
				SoundDefOf.Tick_Tiny?.PlayOneShotOnCamera();
				this.RandomizeSeedAndFilter();
				this._seedIsValid = true;
			}

			if (this._seedIsValid) {
				string label = "PM.Seed".TranslateFull(CapitalizeType.First);
#if V1_0
				listing.Label(label);
#else
				_ = listing.Label(label);
#endif
				string text = listing.TextEntry(this._seed) ?? "";
				if (text != this._seed) {
					this._seed = text;
					this._filter = PawnFilterMaker.Random(this._seed);
				}
			} else {
				listing.Gap(Text.LineHeight * 2 + 2);
			}

			listing.CheckboxLabeled("PM.EditMode".TranslateFull(CapitalizeType.First), ref this._editMode);

			if (this._editMode) {
				this._seedIsValid = false;
				if (listing.ButtonText("PM.AddPart".TranslateFull(CapitalizeType.First))) {
					this.OpenAddFilterPartMenu();
				}
			}

			listing.End();
		}

		private void OpenAddFilterPartMenu() =>
			FloatMenuUtility.MakeMenu(
				PawnFilterMaker.AddableParts(this._filter).Where((part) => part.PlayerAddRemovable && part.Category != PawnFilterPartCategory.Obsolete).OrderBy((part) => part.label),
				(def) => def.LabelCap.Finish(),
				(def) => () => this.AddFilterPart(def));

		private void RandomizeSeedAndFilter() {
			this._seed = GenText.RandomSeedString() ?? "";
			this._filter = PawnFilterMaker.Random(this._seed);
		}
	}
}
