using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.UI {
	// Based on `RimWorld.Page_ScenarioEditor`.
	public class PawnFilterEditorPage : Page {
		public PawnFilter Filter { get; private set; }

		private Vector2 infoScrollPosition;

		private string seed;

		private bool seedIsValid;

		private bool editMode;

		private const float ConfigControlsScreenShare = 0.35f;

		private const float GapBetweenColumns = 17;

		private const float ConfigControlsColumnWidth = 200;

		private static readonly float InvalidSeedGap = Text.LineHeight * 2 + 2;

		public override string PageTitle => "FilterEditor".Translate().CapitalizeFirst();

		public PawnFilterEditorPage(PawnFilter filter) {
			this.infoScrollPosition = Vector2.zero;
			this.seedIsValid = filter == null;
			if (!this.seedIsValid) {
				this.Filter = filter;
			} else {
				this.RandomizeSeedAndFilter();
			}
		}

		public override void PreOpen() {
			base.PreOpen();
			this.infoScrollPosition = Vector2.zero;
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

		private void RandomizeSeedAndFilter() {
			this.seed = GenText.RandomSeedString();
			this.Filter = PawnFilterMaker.Random(this.seed);
		}

		private void DoConfigControls(Rect inRect) {
			Listing_Standard listing = new Listing_Standard() { ColumnWidth = ConfigControlsColumnWidth };
			listing.Begin(inRect);

			if (listing.ButtonText("Load".Translate().CapitalizeFirst())) {
				Find.WindowStack.Add(new PawnFilterListLoadDialog((filter) => {
					this.Filter = filter;
					this.seedIsValid = false;
				}));
			}

			if (listing.ButtonText("Save".Translate().CapitalizeFirst()) && CheckAllPartsCompatible(this.Filter)) {
				Find.WindowStack.Add(new PawnFilterListSaveDialog(this.Filter));
			}

			if (listing.ButtonText("RandomizeSeed".Translate().CapitalizeFirst())) {
				SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
				this.RandomizeSeedAndFilter();
				this.seedIsValid = true;
			}

			if (this.seedIsValid) {
#if V1_0
				listing.Label("Seed".Translate().CapitalizeFirst());
#else
				_ = listing.Label("Seed".Translate().CapitalizeFirst());
#endif
				string text = listing.TextEntry(this.seed);
				if (text != this.seed) {
					this.seed = text;
					this.Filter = PawnFilterMaker.Random(this.seed);
				}
			} else {
				listing.Gap(InvalidSeedGap);
			}

			listing.CheckboxLabeled("EditMode".Translate().CapitalizeFirst(), ref this.editMode);

			if (this.editMode) {
				this.seedIsValid = false;
				if (listing.ButtonText("AddPart".Translate().CapitalizeFirst())) {
					this.OpenAddFilterPartMenu();
				}
			}

			listing.End();
		}

		private static bool CheckAllPartsCompatible(PawnFilter filter) {
			foreach (PawnFilterPart part in filter.Parts) {
				int num = 0;
				foreach (PawnFilterPart part2 in filter.Parts) {
					if (part2.Def == part.Def) {
						num++;
					}

					if (num > part.Def.maxUses) {
						Messages.Message("TooManyOfFilterPart".Translate(), MessageTypeDefOf.RejectInput, false);
						return false;
					}

					if (part != part2 && !part.CanCoexistWith(part2)) {
						Messages.Message("IncompatibleFilterPart".Translate(), MessageTypeDefOf.RejectInput, false);
						return false;
					}
				}
			}

			return true;
		}

		private void OpenAddFilterPartMenu() => FloatMenuUtility.MakeMenu(
			from part in PawnFilterMaker.AddableParts(this.Filter)
			where part.category != PawnFilterPartCategory.Fixed
			orderby part.label
			select part,
			(def) => def.LabelCap,
			(def) => () => this.AddFilterPart(def));

		private void AddFilterPart(PawnFilterPartDef def) {
			PawnFilterPart part = PawnFilterMaker.MakeFilterPart(def);
			part.Randomize();
			this.Filter.AddPart(part);
		}

		protected override bool CanDoNext() {
			if (!base.CanDoNext()) {
				return false;
			}

			if (this.Filter == null) {
				return false;
			}

			if (!CheckAllPartsCompatible(this.Filter)) {
				return false;
			}

			SelectPawnFilterPage.BeginFilterConfiguration(this.Filter);
			return true;
		}
	}
}
