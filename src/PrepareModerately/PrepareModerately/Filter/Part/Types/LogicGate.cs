using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class LogicGate : PawnFilterPart {
#pragma warning restore CA1812
		private const float CollapseWidgetRowMaxWidth = 24;

		private const float Padding = 20;

		private static IEnumerable<LogicGateType> LegalLogicGateTypes =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(LogicGateType)).OfType<LogicGateType>();
#else
			Enum.GetValues<LogicGateType>().OfType<LogicGateType>();
#endif

		private float _editViewHeight;

		private bool _collapsed;

#pragma warning disable CA1051, IDE1006 // `Def` relies on static fields with names matching XML definitions.
		private PawnFilter innerFilter;

		private LogicGateType type;
#pragma warning restore CA1051, IDE1006

		// Must be public.
		public LogicGate() : base() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this.innerFilter = new PawnFilter {
				name = "Inner filter",
				description = "This filter exists inside of a logic gate. It is part of another filter.",
				summary = "If you are seeing this summary in the UI, something has gone wrong."
			};
#else
			this.innerFilter = new() {
				name = "Inner filter",
				description = "This filter exists inside of a logic gate. It is part of another filter.",
				summary = "If you are seeing this summary in the UI, something has gone wrong."
			};
#endif
			this._collapsed = false;
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.type, nameof(this.type));
			Scribe_Deep.Look(ref this.innerFilter, nameof(this.innerFilter));
		}

		public override bool Matches(Pawn pawn) {
			switch (this.type) {
				case LogicGateType.And:
					return this.innerFilter.Matches(pawn);
				case LogicGateType.Or:
					foreach (PawnFilterPart part in this.innerFilter.parts) {
						if (part.Matches(pawn)) {
							return true;
						}
					}

					return false;
				case LogicGateType.Not:
					foreach (PawnFilterPart part in this.innerFilter.parts) {
						if (!part.NotMatches(pawn)) {
							return false;
						}
					}

					return true;
				case LogicGateType.Xor:
					bool flag = false;
					foreach (PawnFilterPart part in this.innerFilter.parts) {
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

		public override string Summary(PawnFilter filter) {
			string output = "";

			foreach (PawnFilterPart part in this.innerFilter.parts) {
				part.Summarized = false;
			}

			foreach (PawnFilterPart part in
				from part in this.innerFilter.parts
				orderby part.def.summaryPriority descending,
				part.def.defName
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
					return "PM.AllOf".Translate(output);
				case LogicGateType.Or:
					return "PM.AtLeastOneOf".Translate(output);
				case LogicGateType.Not:
					return "PM.NoneOf".Translate(output);
				case LogicGateType.Xor:
					return "PM.ExactlyOneOf".Translate(output);
				default:
					PrepareModeratelyLogger.LogErrorMessage("Invalid logic gate type.");
					return output;
			}
		}

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			// Container.
			Rect rect = this._collapsed
				? listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect collapseRect)
				: listing.GetPawnFilterPartRect(this, Text.LineHeight * 2 + this._editViewHeight + Padding, out totalAddedListHeight, out collapseRect);
			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();

			// Collapse widget.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			WidgetRow collapseWidgetRow = new WidgetRow(collapseRect.x, collapseRect.y, UIDirection.RightThenDown, CollapseWidgetRowMaxWidth, 0);
#else
			WidgetRow collapseWidgetRow = new(collapseRect.x, collapseRect.y, UIDirection.RightThenDown, CollapseWidgetRowMaxWidth, 0);
#endif
			if (collapseWidgetRow.ButtonIcon(this._collapsed ? Textures.s_reorderDown : Textures.s_reorderUp, (this._collapsed ? "Expand" : "Collapse").Translate().CapitalizeFirst(), GenUI.SubtleMouseoverColor)) {
				this._collapsed = !this._collapsed;
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}

			// Don't draw the rest of the UI if collapsed.
			if (this._collapsed) {
				return;
			}

			// Type button.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect typeRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
#else
			Rect typeRect = new(rect.x, rect.y, rect.width, Text.LineHeight);
#endif
			if (Widgets.ButtonText(typeRect, this.type.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(LegalLogicGateTypes.OrderBy((type) => type.ToString()),
					(type) => type.ToString().CapitalizeFirst(),
					(type) => () => this.type = type);
			}

			// Add part button.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect addPartRect = new Rect(rect.x, typeRect.yMax, rect.width, Text.LineHeight);
#else
			Rect addPartRect = new(rect.x, typeRect.yMax, rect.width, Text.LineHeight);
#endif
			if (Widgets.ButtonText(addPartRect, "PM.AddPart".Translate().CapitalizeFirst())) {
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
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect listingRect = new Rect(rect.x, addPartRect.yMax, rect.width, this._editViewHeight);
			PawnFilterEditListing innerListing = new PawnFilterEditListing(this.innerFilter) {
				ColumnWidth = listingRect.width
			};
#else
			Rect listingRect = new(rect.x, addPartRect.yMax, rect.width, this._editViewHeight);
			PawnFilterEditListing innerListing = new(this.innerFilter) {
				ColumnWidth = listingRect.width
			};
#endif
			innerListing.Begin(listingRect);

			// Fill part listing.
			float listingHeight = 0;
			for (int i = 0; i < this.innerFilter.parts.Count; i++) { // Can't use `foreach` or the mouse stack will overflow when removing elements.
				PawnFilterPart part = this.innerFilter.parts.ElementAt(i);
				part.DoEditInterface(innerListing, out float partAddedListHeight);
				listingHeight += partAddedListHeight;
			}

			// End part listing.
			innerListing.End();
			this._editViewHeight = Math.Max(innerListing.CurHeight, listingHeight);
		}
	}
}
