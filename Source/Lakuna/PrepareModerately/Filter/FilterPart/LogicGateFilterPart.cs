using System;
using System.Linq;
using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class LogicGateFilterPart : FilterPart {
		public Filter innerFilter;

		public LogicGateType type;

		private float editViewHeight;

		private Vector2 infoScrollPosition;

		public LogicGateFilterPart() : base() {
			this.innerFilter = new Filter();
			this.innerFilter.name = "INNER";
			this.innerFilter.description = "INNER";
			this.innerFilter.summary = "INNER";
			this.infoScrollPosition = Vector2.zero;
		}

		public override bool Matches(Pawn pawn) {
			switch (this.type) {
				case LogicGateType.AND:
					return this.innerFilter.Matches(pawn);
				case LogicGateType.OR:
					foreach (FilterPart part in this.innerFilter.AllParts) {
						if (part.Matches(pawn)) { return true; }
					}
					return false;
				case LogicGateType.NOT:
					foreach (FilterPart part in this.innerFilter.AllParts) {
						if (part.Matches(pawn)) { return false; }
					}
					return true;
				default:
					Logger.LogErrorMessage("InvalidLogicGateType".Translate());
					return true;
			}
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, (Text.LineHeight * 2) + this.editViewHeight);
			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();

			Rect typeRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(typeRect, this.type.ToString())) {
				FloatMenuUtility.MakeMenu((LogicGateType[])Enum.GetValues(typeof(LogicGateType)),
					(LogicGateType type) => type.ToString(),
					(LogicGateType type) => () => this.type = type);
			}

			Rect addPartRect = new Rect(rect.x, typeRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(addPartRect, "AddPart".Translate())) {
				FloatMenuUtility.MakeMenu(from part in FilterMaker.AddableParts(this.innerFilter) where part.category != FilterPartCategory.Fixed orderby part.label select part, (FilterPartDef def) => def.label, (FilterPartDef def) => delegate {
					FilterPart part = FilterMaker.MakeFilterPart(def);
					part.Randomize();
					this.innerFilter.parts.Add(part);
				});
			}

			Rect listingRect = new Rect(rect.x, addPartRect.yMax, rect.width - 16, this.editViewHeight);
			FilterEditListing innerListing = new FilterEditListing(this.innerFilter);
			innerListing.ColumnWidth = listingRect.width;
			innerListing.Begin(listingRect);

			foreach (FilterPart part in this.innerFilter.AllParts) { part.DoEditInterface(innerListing); }

			innerListing.End();
			this.editViewHeight = innerListing.CurHeight + 100;
		}

		public override string Summary(Filter filter) {
			string output = "";

			foreach (FilterPart part in this.innerFilter.AllParts) {
				part.summarized = false;
			}

			foreach (FilterPart part in from part in this.innerFilter.AllParts orderby part.def.summaryPriority descending, part.def.defName where part.visible select part) {
				string summary = part.Summary(this.innerFilter);
				if (!summary.NullOrEmpty()) {
					if (output.Length > 0) { output += ", "; }
					output += summary;
				}
			}

			switch (this.type) {
				case LogicGateType.AND:
					return "AllOf".Translate(output);
				case LogicGateType.OR:
					return "OneOf".Translate(output);
				case LogicGateType.NOT:
					return "NoneOf".Translate(output);
				default:
					Logger.LogErrorMessage("InvalidLogicGateType".Translate());
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
