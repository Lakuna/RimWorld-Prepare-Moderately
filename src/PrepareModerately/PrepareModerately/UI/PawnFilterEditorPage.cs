using System;
using System.Linq;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;

using RimWorld;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `RimWorld.Page_ScenarioEditor`.
	internal sealed class PawnFilterEditorPage : Page {
		private const float _configControlsColumnWidth = 200;

		private const float _configControlsScreenShare = 0.35f;

		private const float _gapBetweenColumns = 17;

		private bool _filterEdited =>
			this._editMode && this._filter.GetHashCode() != this._initialHash;

		public override string PageTitle =>
			$"{"PM.FilterEditor".Translate().CapitalizeFirst()}{(this._filterEdited ? "*" : "")}";

		private PawnFilter _filter {
			get; set;
		}

		private bool _editMode;

		private Vector2 _infoScrollPosition;

		private readonly int _initialHash;

		private bool _needsSave;

		private string _seed;

		private bool _seedIsValid;

		internal PawnFilterEditorPage(PawnFilter filter) {
			this._infoScrollPosition = Vector2.zero;
			this._needsSave = true;
			this._seedIsValid = filter is null;
			if (filter is null) {
				this.RandomizeSeedAndFilter();
			} else {
				this._filter = filter;
			}

			this._initialHash = this._filter.GetHashCode();
		}

		private static bool CheckAllPartsCompatible(PawnFilter filter) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (filter is null) {
				throw new ArgumentNullException(nameof(filter));
			}
#else
			ArgumentNullException.ThrowIfNull(filter);
#endif

			foreach (PawnFilterPart part in filter.parts) {
				int num = 0;
				foreach (PawnFilterPart part2 in filter.parts) {
					if (part2.def == part.def) {
						num++;
					}

					if (num > part.def.maxUses) {
						Messages.Message("PM.TooManyOfFilterPart".Translate(), MessageTypeDefOf.RejectInput, false);
						return false;
					}

					if (part != part2 && !part.CanCoexistWith(part2)) {
						Messages.Message("PM.IncompatibleFilterPart".Translate(), MessageTypeDefOf.RejectInput, false);
						return false;
					}
				}
			}

			return true;
		}

		public override void DoWindowContents(Rect inRect) {
			this.DrawPageTitle(inRect);

			Rect mainRect = this.GetMainRect(inRect);
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
			if (!base.CanDoNext() || this._filter is null || !CheckAllPartsCompatible(this._filter)) {
				return false;
			}

			if (this._filterEdited && this._needsSave) {
				Find.WindowStack.Add(
					new Dialog_MessageBox(
						"PM.FilterSavePrompt".Translate().CapitalizeFirst(),
						"Yes".Translate().CapitalizeFirst(),
						() => {
							Find.WindowStack.Add(new PawnFilterListSaveDialog(this._filter, () => {
								SelectPawnFilterPage.BeginFilterConfiguration(this._filter);
								this._needsSave = false;
								this.DoNext();
							}));
						},
						"No".Translate().CapitalizeFirst(),
						() => {
							SelectPawnFilterPage.BeginFilterConfiguration(this._filter);
							this._needsSave = false;
							this.DoNext();
						}
					)
				);
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
				ColumnWidth = _configControlsColumnWidth
			};
#else
			Listing_Standard listing = new() {
				ColumnWidth = _configControlsColumnWidth
			};
#endif
			listing.Begin(inRect);

			if (listing.ButtonText("PM.Load".Translate().CapitalizeFirst())) {
				Find.WindowStack.Add(new PawnFilterListLoadDialog((filter) => {
					this._filter = filter;
					this._seedIsValid = false;
				}));
			}

			if (listing.ButtonText("PM.Save".Translate().CapitalizeFirst()) && CheckAllPartsCompatible(this._filter)) {
				Find.WindowStack.Add(new PawnFilterListSaveDialog(this._filter));
			}

			if (listing.ButtonText("PM.RandomizeSeed".Translate().CapitalizeFirst())) {
				SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
				this.RandomizeSeedAndFilter();
				this._seedIsValid = true;
			}

			if (this._seedIsValid) {
#if V1_0
				listing.Label("PM.Seed".Translate().CapitalizeFirst());
#else
				_ = listing.Label("PM.Seed".Translate().CapitalizeFirst());
#endif
				string text = listing.TextEntry(this._seed);
				if (text != this._seed) {
					this._seed = text;
					this._filter = PawnFilterMaker.Random(this._seed);
				}
			} else {
				listing.Gap(Text.LineHeight * 2 + 2);
			}

			listing.CheckboxLabeled("PM.EditMode".Translate().CapitalizeFirst(), ref this._editMode);

			if (this._editMode) {
				this._seedIsValid = false;
				if (listing.ButtonText("PM.AddPart".Translate().CapitalizeFirst())) {
					this.OpenAddFilterPartMenu();
				}
			}

			listing.End();
		}

		private void OpenAddFilterPartMenu() =>
			FloatMenuUtility.MakeMenu(
				from part in PawnFilterMaker.AddableParts(this._filter)
				where part._playerAddRemovable
				orderby part.label
				select part,
				(def) => def.LabelCap,
				(def) => () => this.AddFilterPart(def));

		private void RandomizeSeedAndFilter() {
			this._seed = GenText.RandomSeedString();
			this._filter = PawnFilterMaker.Random(this._seed);
		}
	}
}
