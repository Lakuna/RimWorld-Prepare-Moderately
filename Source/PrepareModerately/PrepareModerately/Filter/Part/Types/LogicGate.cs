using Lakuna.PrepareModerately.UI;
using Lakuna.PrepareModerately.Utility;
using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class LogicGateFilterPart : PawnFilterPart {
		private PawnFilter innerFilter;

		private LogicGateType type;

		private float editViewHeight;

		public LogicGateFilterPart() : base() => this.innerFilter = new PawnFilter {
			Name = "Inner filter",
			Description = "This filter exists inside of a logic gate. It is part of another filter.",
			Summary = "If you are seeing this summary in the UI, something has gone wrong."
		};

		public override bool Matches(Pawn pawn) {
			switch (this.type) {
				case LogicGateType.And:
					return this.innerFilter.Matches(pawn);
				case LogicGateType.Or:
					foreach (PawnFilterPart part in this.innerFilter.Parts) {
						if (part.Matches(pawn)) { return true; }
					}
					return false;
				case LogicGateType.Not:
					foreach (PawnFilterPart part in this.innerFilter.Parts) {
						if (part.Matches(pawn)) { return false; }
					}
					return true;
				default:
					PrepareModeratelyLogger.LogErrorMessage("Invalid logic gate type.");
					return true;
			}
		}

		public override void DoEditInterface(PawnFilterEditListing listing) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2 + this.editViewHeight);
			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();

			Rect typeRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(typeRect, this.type.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((LogicGateType[])Enum.GetValues(typeof(LogicGateType)),
					(LogicGateType type) => type.ToString().CapitalizeFirst(),
					(LogicGateType type) => () => this.type = type);
			}

			Rect addPartRect = new Rect(rect.x, typeRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(addPartRect, "AddPart".Translate().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(from part in PawnFilterMaker.AddableParts(this.innerFilter)
					where part.Category != PawnFilterPartCategory.Fixed orderby part.label select part,
					(PawnFilterPartDef def) => def.LabelCap,
					(PawnFilterPartDef def) => delegate {
						PawnFilterPart part = PawnFilterMaker.MakeFilterPart(def);
						part.Randomize();
						_ = this.innerFilter.Parts.Append(part);
					});
			}

			Rect listingRect = new Rect(rect.x, addPartRect.yMax, rect.width - 16, this.editViewHeight);
			PawnFilterEditListing innerListing = new PawnFilterEditListing(this.innerFilter) {
				ColumnWidth = listingRect.width
			};
			innerListing.Begin(listingRect);

			foreach (PawnFilterPart part in this.innerFilter.Parts) { part.DoEditInterface(innerListing); }

			innerListing.End();
			this.editViewHeight = innerListing.CurHeight + 100;
		}

		public override string Summary(PawnFilter filter) {
			string output = "";

			foreach (PawnFilterPart part in this.innerFilter.Parts) {
				part.Summarized = false;
			}

			foreach (PawnFilterPart part in from part in this.innerFilter.Parts orderby part.Def.SummaryPriority descending,
				part.Def.defName where part.Visible select part) {
				string summary = part.Summary(this.innerFilter);
				if (!summary.NullOrEmpty()) {
					if (output.Length > 0) { output += ", "; }
					output += summary;
				}
			}

			switch (this.type) {
				case LogicGateType.And:
					return "AllOf".Translate(output);
				case LogicGateType.Or:
					return "OneOf".Translate(output);
				case LogicGateType.Not:
					return "NoneOf".Translate(output);
				default:
					PrepareModeratelyLogger.LogErrorMessage("Invalid logic gate type.");
					return output;
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.type, nameof(this.type));
			Scribe_Deep.Look(ref this.innerFilter, nameof(this.innerFilter));
			Scribe_Values.Look(ref this.editViewHeight, nameof(this.editViewHeight));
		}
	}
}
