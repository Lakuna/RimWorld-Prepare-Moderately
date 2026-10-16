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
		private const float ConfigControlsColumnWidth = 200;

		private const float ConfigControlsScreenShare = 0.35f;

		private const float GapBetweenColumns = 17;

		public bool FilterEdited =>
			this.editMode && this.Filter.GetHashCode() != this.initialHash;

		public override string PageTitle =>
			$"{"PM.FilterEditor".Translate().CapitalizeFirst()}{(this.FilterEdited ? "*" : "")}";

		internal PawnFilter Filter {
			get; private set;
		}

		private bool editMode;

		private Vector2 infoScrollPosition;

		private readonly int initialHash;

		private bool needsSave;

		private string seed;

		private bool seedIsValid;

		internal PawnFilterEditorPage(PawnFilter filter) {
			this.infoScrollPosition = Vector2.zero;
			this.needsSave = true;
			this.seedIsValid = filter is null;
			if (filter is null) {
				this.RandomizeSeedAndFilter();
			} else {
				this.Filter = filter;
			}

			this.initialHash = this.Filter.GetHashCode();
		}

		private static bool CheckAllPartsCompatible(PawnFilter filter) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (filter is null) {
				throw new ArgumentNullException(nameof(filter));
			}
#else
			ArgumentNullException.ThrowIfNull(filter);
#endif

			foreach (PawnFilterPart part in filter.PParts) {
				int num = 0;
				foreach (PawnFilterPart part2 in filter.PParts) {
					if (part2.PDef == part.PDef) {
						num++;
					}

					if (num > part.PDef.PMaxUses) {
						Messages.Message("PM.TooManyOfFilterPart".Translate(), MessageTypeDefOf.RejectInput, false);
						return false;
					}

					if (part != part2 && !part.canCoexistWith(part2)) {
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

			Rect configControlsRect = new Rect(0, 0, mainRect.width * ConfigControlsScreenShare, mainRect.height).Rounded();
			this.DoConfigControls(configControlsRect);

			Rect filterEditRect = new Rect(configControlsRect.xMax + GapBetweenColumns, 0, mainRect.width - configControlsRect.width - GapBetweenColumns, mainRect.height).Rounded();
			if (this.editMode) {
				PawnFilterUi.DrawEditInterface(filterEditRect, this.Filter, ref this.infoScrollPosition);
			} else {
				PawnFilterUi.DrawInfo(filterEditRect, this.Filter, ref this.infoScrollPosition);
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
			this.infoScrollPosition = Vector2.zero;
			this.needsSave = true;
		}

		protected override bool CanDoNext() {
			if (!base.CanDoNext() || this.Filter is null || !CheckAllPartsCompatible(this.Filter)) {
				return false;
			}

			if (this.FilterEdited && this.needsSave) {
				Find.WindowStack.Add(
					new Dialog_MessageBox(
						"PM.FilterSavePrompt".Translate().CapitalizeFirst(),
						"Yes".Translate().CapitalizeFirst(),
						() => {
							Find.WindowStack.Add(new PawnFilterListSaveDialog(this.Filter, () => {
								SelectPawnFilterPage.BeginFilterConfiguration(this.Filter);
								this.needsSave = false;
								this.DoNext();
							}));
						},
						"No".Translate().CapitalizeFirst(),
						() => {
							SelectPawnFilterPage.BeginFilterConfiguration(this.Filter);
							this.needsSave = false;
							this.DoNext();
						}
					)
				);
				return false;
			}

			SelectPawnFilterPage.BeginFilterConfiguration(this.Filter);
			return true;
		}

		private void AddFilterPart(PawnFilterPartDef def) {
			PawnFilterPart part = PawnFilterMaker.MakeFilterPart(def);
			part.randomize();
			this.Filter.AddPart(part);
		}

		private void DoConfigControls(Rect inRect) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Listing_Standard listing = new Listing_Standard() {
				ColumnWidth = ConfigControlsColumnWidth
			};
#else
			Listing_Standard listing = new() {
				ColumnWidth = ConfigControlsColumnWidth
			};
#endif
			listing.Begin(inRect);

			if (listing.ButtonText("PM.Load".Translate().CapitalizeFirst())) {
				Find.WindowStack.Add(new PawnFilterListLoadDialog((filter) => {
					this.Filter = filter;
					this.seedIsValid = false;
				}));
			}

			if (listing.ButtonText("PM.Save".Translate().CapitalizeFirst()) && CheckAllPartsCompatible(this.Filter)) {
				Find.WindowStack.Add(new PawnFilterListSaveDialog(this.Filter));
			}

			if (listing.ButtonText("PM.RandomizeSeed".Translate().CapitalizeFirst())) {
				SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
				this.RandomizeSeedAndFilter();
				this.seedIsValid = true;
			}

			if (this.seedIsValid) {
#if V1_0
				listing.Label("PM.Seed".Translate().CapitalizeFirst());
#else
				_ = listing.Label("PM.Seed".Translate().CapitalizeFirst());
#endif
				string text = listing.TextEntry(this.seed);
				if (text != this.seed) {
					this.seed = text;
					this.Filter = PawnFilterMaker.Random(this.seed);
				}
			} else {
				listing.Gap(Text.LineHeight * 2 + 2);
			}

			listing.CheckboxLabeled("PM.EditMode".Translate().CapitalizeFirst(), ref this.editMode);

			if (this.editMode) {
				this.seedIsValid = false;
				if (listing.ButtonText("PM.AddPart".Translate().CapitalizeFirst())) {
					this.OpenAddFilterPartMenu();
				}
			}

			listing.End();
		}

		private void OpenAddFilterPartMenu() =>
			FloatMenuUtility.MakeMenu(
				from part in PawnFilterMaker.AddableParts(this.Filter)
				where part.PlayerAddRemovable
				orderby part.label
				select part,
				(def) => def.LabelCap,
				(def) => () => this.AddFilterPart(def));

		private void RandomizeSeedAndFilter() {
			this.seed = GenText.RandomSeedString();
			this.Filter = PawnFilterMaker.Random(this.seed);
		}
	}
}
