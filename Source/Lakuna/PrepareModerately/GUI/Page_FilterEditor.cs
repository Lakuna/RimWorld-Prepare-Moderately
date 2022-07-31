using Lakuna.PrepareModerately.PawnFilter;
using Lakuna.PrepareModerately.PawnFilter.Parts;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.GUI {
	// Based on RimWorld.Page_ScenarioEditor.
	public class Page_FilterEditor : Page {
		private Filter currentFilter;

		private Vector2 infoScrollPosition = Vector2.zero;

		private string seed;

		private bool seedIsValid = true;

		private bool editMode;

		public override string PageTitle => "FilterEditor".Translate();

		public Filter EditingFilter => this.currentFilter;

		public Page_FilterEditor(Filter filter) {
			if (filter != null) {
				this.currentFilter = filter;
				this.seedIsValid = false;
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
			Widgets.BeginGroup(mainRect);


			Rect rect2 = new Rect(0, 0, mainRect.width * 0.35f, mainRect.height).Rounded();
			this.DoConfigControls(rect2);

			Rect rect3 = new Rect(rect2.xMax + 17, 0, mainRect.width - rect2.width - 17, mainRect.height).Rounded();
			if (!this.editMode) {
				FilterUI.DrawFilterInfo(rect3, this.currentFilter, ref this.infoScrollPosition);
			} else {
				FilterUI.DrawFilterEditInterface(rect3, this.currentFilter, ref this.infoScrollPosition);
			}

			Widgets.EndGroup();

			this.DoBottomButtons(inRect);
		}

		private void RandomizeSeedAndFilter() {
			this.seed = GenText.RandomSeedString();
			this.currentFilter = FilterMaker.GenerateNewRandomFilter(this.seed);
		}

		private void DoConfigControls(Rect inRect) {
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.ColumnWidth = 200;
			listing_Standard.Begin(inRect);

			if (listing_Standard.ButtonText("Load".Translate())) {
				Find.WindowStack.Add(new Dialog_FilterList_Load(delegate (Filter loadedFilter) {
					this.currentFilter = loadedFilter;
					this.seedIsValid = false;
				}));
			}

			if (listing_Standard.ButtonText("Save".Translate()) && Page_FilterEditor.CheckAllPartsCompatible(this.currentFilter)) {
				Find.WindowStack.Add(new Dialog_FilterList_Save(this.currentFilter));
			}

			if (listing_Standard.ButtonText("RandomizeSeed".Translate())) {
				SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
				this.RandomizeSeedAndFilter();
				this.seedIsValid = true;
			}

			if (this.seedIsValid) {
				listing_Standard.Label("Seed".Translate().CapitalizeFirst());
				string text = listing_Standard.TextEntry(this.seed);
				if (text != this.seed) {
					this.seed = text;
					this.currentFilter = FilterMaker.GenerateNewRandomFilter(this.seed);
				}
			} else {
				listing_Standard.Gap(Text.LineHeight + Text.LineHeight + 2);
			}

			listing_Standard.CheckboxLabeled("EditMode".Translate().CapitalizeFirst(), ref this.editMode);

			if (this.editMode) {
				this.seedIsValid = false;

				if (listing_Standard.ButtonText("AddPart".Translate())) {
					this.OpenAddFilterPartMenu();
				}
			}

			listing_Standard.End();
		}

		private static bool CheckAllPartsCompatible(Filter filter) {
			foreach (FilterPart allPart in filter.AllParts) {
				int num = 0;
				foreach (FilterPart allPart2 in filter.AllParts) {
					if (allPart2.def == allPart.def) { num++; }

					if (num > allPart.def.maxUses) {
						Messages.Message("TooMany".Translate(allPart.def.maxUses) + ": " + allPart.def.label, MessageTypeDefOf.RejectInput, historical: false);
						return false;
					}

					if (allPart != allPart2 && !allPart.CanCoexistWith(allPart2)) {
						Messages.Message("Incompatible".Translate() + ": " + allPart.def.label + ", " + allPart2.def.label, MessageTypeDefOf.RejectInput, historical: false);
						return false;
					}
				}
			}

			return true;
		}

		private void OpenAddFilterPartMenu() {
			FloatMenuUtility.MakeMenu(
				from p in FilterMaker.AddableParts(this.currentFilter) where p.category != FilterPartCategory.Fixed orderby p.label select p,
				(FilterPartDef p) => p.LabelCap,
				(FilterPartDef p) => delegate { this.AddFilterPart(p); });
		}

		private void AddFilterPart(FilterPartDef def) {
			FilterPart filterPart = FilterMaker.MakeFilterPart(def);
			filterPart.Randomize();
			this.currentFilter.parts.Add(filterPart);
		}

		protected override bool CanDoNext() {
			if (!base.CanDoNext()) { return false; }
			if (this.currentFilter == null) { return false; }
			if (!Page_FilterEditor.CheckAllPartsCompatible(this.currentFilter)) { return false; }
			Page_SelectFilter.BeginFilterConfiguration(this.currentFilter, this);
			return true;
		}
	}
}
