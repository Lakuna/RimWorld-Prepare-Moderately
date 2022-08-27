using System.Linq;
using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.FilterPart;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.UI {
	public class FilterEditorPage : Page {
		private Filter.Filter currentFilter;

		private Vector2 infoScrollPosition;

		private string seed;

		private bool seedIsValid;

		private bool editMode;

		public override string PageTitle => "FilterEditor".Translate();

		public Filter.Filter EditingFilter => this.currentFilter;

		public FilterEditorPage(Filter.Filter filter) {
			this.infoScrollPosition = Vector2.zero;
			this.seedIsValid = true;
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

		public override void DoWindowContents(Rect rect) {
			this.DrawPageTitle(rect);

			Rect mainRect = this.GetMainRect(rect);
			Widgets.BeginGroup(mainRect);

			Rect configControlsRect = new Rect(0, 0, mainRect.width * 0.35f, mainRect.height).Rounded();
			this.DoConfigControls(configControlsRect);

			Rect filterEditRect = new Rect(configControlsRect.xMax + 17, 0, mainRect.width - configControlsRect.width - 17, mainRect.height).Rounded();
			if (this.editMode) {
				FilterUI.DrawFilterEditInterface(filterEditRect, this.currentFilter, ref this.infoScrollPosition);
			} else {
				FilterUI.DrawFilterInfo(filterEditRect, this.currentFilter, ref this.infoScrollPosition);
			}

			Widgets.EndGroup();

			this.DoBottomButtons(rect);
		}

		private void RandomizeSeedAndFilter() {
			this.seed = GenText.RandomSeedString();
			this.currentFilter = FilterMaker.GenerateNewRandomFilter(this.seed);
		}

		private void DoConfigControls(Rect rect) {
			Listing_Standard listing = new Listing_Standard();
			listing.ColumnWidth = 200;
			listing.Begin(rect);

			if (listing.ButtonText("Load".Translate())) {
				Find.WindowStack.Add(new FilterListLoadDialog(delegate (Filter.Filter filter) {
					this.currentFilter = filter;
					this.seedIsValid = false;
				}));
			}

			if (listing.ButtonText("Save".Translate()) && FilterEditorPage.CheckAllPartsCompatible(this.currentFilter)) {
				Find.WindowStack.Add(new FilterListSaveDialog(this.currentFilter));
			}

			if (listing.ButtonText("RandomizeSeed".Translate())) {
				SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
				this.RandomizeSeedAndFilter();
				this.seedIsValid = true;
			}

			if (this.seedIsValid) {
				listing.Label("Seed".Translate());
				string text = listing.TextEntry(this.seed);
				if (text != this.seed) {
					this.seed = text;
					this.currentFilter = FilterMaker.GenerateNewRandomFilter(this.seed);
				}
			} else {
				listing.Gap(Text.LineHeight + Text.LineHeight + 2);
			}

			listing.CheckboxLabeled("EditMode".Translate(), ref this.editMode);

			if (this.editMode) {
				this.seedIsValid = false;

				if (listing.ButtonText("AddPart".Translate())) { this.OpenAddFilterPartMenu(); }
			}

			listing.End();
		}

		private static bool CheckAllPartsCompatible(Filter.Filter filter) {
			foreach (FilterPart part in filter.AllParts) {
				int num = 0;
				foreach (FilterPart part2 in filter.AllParts) {
					if (part2.def == part.def) { num++; }
					if (num > part.def.maxUses) {
						Messages.Message("TooMany".Translate(part.def.maxUses) + ": " + part.def.label, MessageTypeDefOf.RejectInput, false);
						return false;
					}
					if (part != part2 && !part.CanCoexistWith(part2)) {
						Messages.Message("Incompatible".Translate() + ": " + part.def.label + ", " + part2.def.label, MessageTypeDefOf.RejectInput, false);
						return false;
					}
				}
			}
			return true;
		}

		private void OpenAddFilterPartMenu() {
			FloatMenuUtility.MakeMenu(from part in FilterMaker.AddableParts(this.currentFilter) where part.category != FilterPartCategory.Fixed orderby part.label select part, (FilterPartDef def) => def.label, (FilterPartDef def) => delegate {
				this.AddFilterPart(def);
			});
		}

		private void AddFilterPart(FilterPartDef def) {
			FilterPart part = FilterMaker.MakeFilterPart(def);
			part.Randomize();
			this.currentFilter.parts.Add(part);
		}

		protected override bool CanDoNext() {
			if (!base.CanDoNext()) { return false; }
			if (this.currentFilter == null) { return false; }
			if (!FilterEditorPage.CheckAllPartsCompatible(this.currentFilter)) { return false; }
			SelectFilterPage.BeginFilterConfiguration(this.currentFilter, this);
			return true;
		}
	}
}
