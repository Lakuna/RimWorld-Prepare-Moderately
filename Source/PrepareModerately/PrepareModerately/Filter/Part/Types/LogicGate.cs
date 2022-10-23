using Lakuna.PrepareModerately.UI;
using Lakuna.PrepareModerately.Utility;
using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class LogicGate : PawnFilterPart {
		private PawnFilter innerFilter;

		private LogicGateType type;

		private float editViewHeight;

		private const float EditViewHeightBuffer = 100;

		public LogicGate() : base() => this.innerFilter = new PawnFilter {
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
				case LogicGateType.Xor:
					bool flag = false;
					foreach (PawnFilterPart part in this.innerFilter.Parts) {
						if (part.Matches(pawn)) {
							if (flag) { return false; }
							flag = true;
						}
					}
					return flag;
				default:
					PrepareModeratelyLogger.LogErrorMessage("Invalid logic gate type.");
					return true;
			}
		}

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2 + this.editViewHeight, out totalAddedListHeight);
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
				FloatMenuUtility.MakeMenu(
					from part in PawnFilterMaker.AddableParts(this.innerFilter)
					where part.category != PawnFilterPartCategory.Fixed
					orderby part.label
					select part,
					(PawnFilterPartDef def) => def.LabelCap,
					(PawnFilterPartDef def) => delegate {
						PawnFilterPart part = PawnFilterMaker.MakeFilterPart(def);
						part.Randomize();
						this.innerFilter.AddPart(part);
					});
			}

			Rect listingRect = new Rect(rect.x, addPartRect.yMax, rect.width, this.editViewHeight);
			PawnFilterEditListing innerListing = new PawnFilterEditListing(this.innerFilter) {
				ColumnWidth = listingRect.width
			};
			innerListing.Begin(listingRect);

			bool flag = this.editViewHeight == 0;
			for (int i = 0; i < this.innerFilter.Parts.Count(); i++) {
				this.innerFilter.Parts.ElementAt(i).DoEditInterface(innerListing, out float partAddedListHeight);
				if (flag) { this.editViewHeight += partAddedListHeight; }
			}
			innerListing.End();
			if (!flag) { this.editViewHeight = innerListing.CurHeight; }
			this.editViewHeight += EditViewHeightBuffer;
		}

		public override string Summary(PawnFilter filter) {
			string output = "";

			foreach (PawnFilterPart part in this.innerFilter.Parts) {
				part.Summarized = false;
			}

			foreach (PawnFilterPart part in
				from part in this.innerFilter.Parts
				orderby part.Def.summaryPriority descending,
				part.Def.defName
				where part.Visible
				select part) {
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
					return "AtLeastOneOf".Translate(output);
				case LogicGateType.Not:
					return "NoneOf".Translate(output);
				case LogicGateType.Xor:
					return "ExactlyOneOf".Translate(output);
				default:
					PrepareModeratelyLogger.LogErrorMessage("Invalid logic gate type.");
					return output;
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.type, nameof(this.type));
			Scribe_Deep.Look(ref this.innerFilter, nameof(this.innerFilter));
		}
	}
}
