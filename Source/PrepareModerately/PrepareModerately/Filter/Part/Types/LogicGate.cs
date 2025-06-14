using Lakuna.PrepareModerately.UI;
using Lakuna.PrepareModerately.Utility;
using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class LogicGate : PawnFilterPart {
		private const float CollapseWidgetRowMaxWidth = 24;

		private PawnFilter innerFilter;

		private bool collapsed;

		private LogicGateType type;

		private float editViewHeight;

		private const float Padding = 20;

		public LogicGate() : base() {
			this.innerFilter = new PawnFilter {
				Name = "Inner filter",
				Description = "This filter exists inside of a logic gate. It is part of another filter.",
				Summary = "If you are seeing this summary in the UI, something has gone wrong."
			};
			this.collapsed = false;
		}

		public override bool Matches(Pawn pawn) {
			switch (this.type) {
				case LogicGateType.And:
					return this.innerFilter.Matches(pawn);
				case LogicGateType.Or:
					foreach (PawnFilterPart part in this.innerFilter.Parts) {
						if (part.Matches(pawn)) {
							return true;
						}
					}

					return false;
				case LogicGateType.Not:
					foreach (PawnFilterPart part in this.innerFilter.Parts) {
						if (!part.NotMatches(pawn)) {
							return false;
						}
					}

					return true;
				case LogicGateType.Xor:
					bool flag = false;
					foreach (PawnFilterPart part in this.innerFilter.Parts) {
						if (part.Matches(pawn)) {
							if (flag) {
								return false;
							}

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

			// Container.
			Rect rect = this.collapsed
				? listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect collapseRect)
				: listing.GetPawnFilterPartRect(this, Text.LineHeight * 2 + this.editViewHeight + Padding, out totalAddedListHeight, out collapseRect);
			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();

			// Collapse widget.
			WidgetRow collapseWidgetRow = new WidgetRow(collapseRect.x, collapseRect.y, UIDirection.RightThenDown, CollapseWidgetRowMaxWidth, 0);
			if (collapseWidgetRow.ButtonIcon(this.collapsed ? Textures.ReorderDown : Textures.ReorderUp, (this.collapsed ? "Expand" : "Collapse").Translate().CapitalizeFirst(), GenUI.SubtleMouseoverColor)) {
				this.collapsed = !this.collapsed;
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}

			// Don't draw the rest of the UI if collapsed.
			if (this.collapsed) {
				return;
			}

			// Type button.
			Rect typeRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(typeRect, this.type.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((LogicGateType[])Enum.GetValues(typeof(LogicGateType)),
					(type) => type.ToString().CapitalizeFirst(),
					(type) => () => this.type = type);
			}

			// Add part button.
			Rect addPartRect = new Rect(rect.x, typeRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(addPartRect, "AddPart".Translate().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(
					from part
					in PawnFilterMaker.AddableParts(this.innerFilter)
					where part.category != PawnFilterPartCategory.Fixed
					orderby part.label
					select part,
					(def) => def.LabelCap,
					(def) => () => {
						PawnFilterPart part = PawnFilterMaker.MakeFilterPart(def);
						part.Randomize();
						this.innerFilter.AddPart(part, true);
					}
				);
			}

			// Part listing.
			Rect listingRect = new Rect(rect.x, addPartRect.yMax, rect.width, this.editViewHeight);
			PawnFilterEditListing innerListing = new PawnFilterEditListing(this.innerFilter) {
				ColumnWidth = listingRect.width
			};
			innerListing.Begin(listingRect);

			// Fill part listing.
			float listingHeight = 0;
			for (int i = 0; i < this.innerFilter.Parts.Count(); i++) { // Can't use `foreach` or the mouse stack will overflow when removing elements.
				PawnFilterPart part = this.innerFilter.Parts.ElementAt(i);
				part.DoEditInterface(innerListing, out float partAddedListHeight);
				listingHeight += partAddedListHeight;
			}

			// End part listing.
			innerListing.End();
			this.editViewHeight = Math.Max(innerListing.CurHeight, listingHeight);
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
					if (output.Length > 0) {
						output += ", ";
					}

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
