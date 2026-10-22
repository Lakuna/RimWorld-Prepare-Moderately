using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class LogicGate : PawnFilterPart {
		private const float _collapseWidgetRowMaxWidth = 24;

		private const float _innerPadding = 5;

		private static IEnumerable<LogicGateType> _legalLogicGateTypes =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(LogicGateType)).OfType<LogicGateType>();
#else
			Enum.GetValues<LogicGateType>().OfType<LogicGateType>();
#endif

		private const float _padding = 20;

		public override IEnumerable<string> ConfigErrors {
			get {
				foreach (string item in base.ConfigErrors) {
					yield return item;
				}

				if (this._innerFilter is null) {
					yield return "Logic gate filter part inner filter is null.";
				}
			}
		}

		public override bool Valid =>
			base.Valid
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			&& this._innerFilter != null;
#else
			&& this._innerFilter is not null;
#endif

		private bool _collapsed;

		private float _editViewHeight;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private PawnFilter _innerFilter = new PawnFilter() {
#else
		private PawnFilter? _innerFilter = new() {
#endif
			Name = "Inner Filter",
			Description = "This filter exists inside of a logic gate. It is part of another filter.",
			Summary = "If you are seeing this summary in the UI, something has gone wrong."
		};

		private LogicGateType _type;

		private static int IndentationLevel(PawnFilter filter, LogicGate gate) =>
			IndentationLevel(filter, gate, out _);

		private static int IndentationLevel(PawnFilter filter, LogicGate gate, out bool filterContainsGate) {
			filterContainsGate = false;

			// If the filter can't contain anything, short circuit.
			if (filter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Filter was null when determining logic gate indentation level.", PrepareModeratelyLoggerCategory.LogicGateIndentation);
				return 0;
			}

			if (filter.Parts is null) {
				PrepareModeratelyLogger.LogErrorMessage("Filter had null parts when determining logic gate indentation level.", PrepareModeratelyLoggerCategory.LogicGateIndentation);
				return 0;
			}

			if (gate is null) {
				PrepareModeratelyLogger.LogErrorMessage("Logic gate was null when determining its indentation level.", PrepareModeratelyLoggerCategory.LogicGateIndentation);
				return 0;
			}

			// If the filter contains the logic gate directly, it has an indentation level of zero.
			if (filter.Parts.Any((part) => part == gate)) {
				filterContainsGate = true;
				return 0;
			}

			// Otherwise, check nested logic gates to see if they contain the logic gate and add one indentation level.
			foreach (PawnFilterPart part in filter.Parts) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				if (!(part is LogicGate gate2)) {
#else
				if (part is not LogicGate gate2) {
#endif
					continue;
				}

				if (gate2._innerFilter is null) {
					PrepareModeratelyLogger.LogErrorMessage("Logic gate had descendent with null inner filter when determining its indentation level.", PrepareModeratelyLoggerCategory.LogicGateIndentation);
					continue;
				}

				int descendentIndentationLevel = IndentationLevel(gate2._innerFilter, gate, out bool descendentContainsGate);
				if (!descendentContainsGate) {
					continue;
				}

				filterContainsGate = true;
				return descendentIndentationLevel + 1;
			}

			// If neither the filter nor any of its descendents contain the logic gate, just use an indentation level of zero.
			PrepareModeratelyLogger.LogErrorMessage("Attempted to determine the indentation level of a logic gate within a filter that did not contain it.", PrepareModeratelyLoggerCategory.LogicGateIndentation);
			return 0;
		}

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			// Container.
			Rect rect = this._collapsed
				? listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect collapseRect).Rounded()
				: listing.GetPawnFilterPartRect(this, Text.LineHeight * 2 + this._editViewHeight + _padding, out totalAddedListHeight, out collapseRect).Rounded();

			if (this._innerFilter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw edit interface for logic gate with null inner filter.");
				return;
			}

			if (this._innerFilter.Parts is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw edit interface for logic gate with inner filter with null parts.");
				return;
			}

			if (Textures.ReorderUp is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw null reorder up texture.");
				return;
			}

			if (Textures.ReorderDown is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw null reorder down texture.");
				return;
			}

			Widgets.DrawMenuSection(rect);
			Rect innerRect = rect.ContractedBy(_innerPadding).Rounded(); // Use `ContractedBy` instead of `GetInnerRect` so that nested logic gates don't get too small. `GetInnerRect` passes a value of `17f` to `ContractedBy`.

			// Collapse widget.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			WidgetRow collapseWidgetRow = new WidgetRow(collapseRect.x, collapseRect.y, UIDirection.RightThenDown, _collapseWidgetRowMaxWidth, 0);
#else
			WidgetRow collapseWidgetRow = new(collapseRect.x, collapseRect.y, UIDirection.RightThenDown, _collapseWidgetRowMaxWidth, 0);
#endif
			if (collapseWidgetRow.ButtonIcon(
				this._collapsed ? Textures.ReorderDown : Textures.ReorderUp,
#if V1_0
				(this._collapsed ? "PM.Expand" : "PM.Collapse").Translate()?.CapitalizeFirst() ?? "null",
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				(this._collapsed ? "PM.Expand" : "PM.Collapse").Translate().CapitalizeFirst().StringSafe(),
#else
				(this._collapsed ? "PM.Expand" : "PM.Collapse").Translate().CapitalizeFirst().StringSafe,
#endif
				GenUI.SubtleMouseoverColor)) {
				SoundDefOf.Tick_Low?.PlayOneShotOnCamera();
				this._collapsed = !this._collapsed;
			}

			// Don't draw the rest of the UI if collapsed.
			if (this._collapsed) {
				return;
			}

			// Type button.
			Rect typeRect = new Rect(innerRect.x, innerRect.y, innerRect.width, Text.LineHeight).Rounded();
			if (Widgets.ButtonText(typeRect, this._type.ToString().ToUpper(CultureInfo.InvariantCulture))) {
				FloatMenuUtility.MakeMenu(
					_legalLogicGateTypes.OrderBy((type) => type.ToString()),
					(type) => type.ToString().ToUpper(CultureInfo.InvariantCulture),
					(type) => () => this._type = type);
			}

			// Add part button.
			Rect addPartRect = new Rect(innerRect.x, typeRect.yMax, innerRect.width, Text.LineHeight).Rounded();
#if V1_0
			if (Widgets.ButtonText(addPartRect, "PM.AddPart".Translate()?.CapitalizeFirst() ?? "null")) {
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(addPartRect, "PM.AddPart".Translate().CapitalizeFirst().StringSafe())) {
#else
			if (Widgets.ButtonText(addPartRect, "PM.AddPart".Translate().CapitalizeFirst().StringSafe)) {
#endif
				FloatMenuUtility.MakeMenu(
					PawnFilterMaker.AddableParts(this._innerFilter).Where((part) => part.Category != PawnFilterPartCategory.Fixed && part.Category != PawnFilterPartCategory.Obsolete).OrderBy((part) => part.label),
#if V1_0
					(def) => def.LabelCap ?? "null",
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(def) => def.LabelCap.StringSafe(),
#else
					(def) => def.LabelCap.StringSafe,
#endif
					(def) => () => {
						PawnFilterPart part = PawnFilterMaker.MakeFilterPart(def);
						part.Randomize();
						this._innerFilter.AddPart(part, true);
					}
				);
			}

			// Part listing.
			Rect listingRect = new Rect(innerRect.x, addPartRect.yMax, innerRect.width, this._editViewHeight).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			PawnFilterEditListing innerListing = new PawnFilterEditListing(this._innerFilter) {
#else
			PawnFilterEditListing innerListing = new(this._innerFilter) {
#endif
				ColumnWidth = listingRect.width
			};
			innerListing.Begin(listingRect);

			// Fill part listing.
			float listingHeight = 0;
			for (int i = 0; i < this._innerFilter.Parts.Count; i++) { // Can't use `foreach` or the mouse stack will overflow when removing elements.
				PawnFilterPart part = this._innerFilter.Parts[i];
				part.DoEditInterface(innerListing, out float partAddedListHeight);
				listingHeight += partAddedListHeight;
			}

			// End part listing.
			innerListing.End();
			this._editViewHeight = Math.Max(innerListing.CurHeight, listingHeight);
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._type, "type");
			Scribe_Deep.Look(ref this._innerFilter, "innerFilter");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ (this._innerFilter?.GetHashCode() ?? 0) ^ this._type.GetHashCode();

		public override bool Matches(Pawn pawn) {
			if (this._innerFilter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to match pawn with logic gate with null inner filter.");
				return false;
			}

			if (this._innerFilter.Parts is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to match pawn with logic gate with inner filter with null parts.");
				return false;
			}

			switch (this._type) {
				case LogicGateType.And:
					return this._innerFilter.Parts.All((part) => part.IgnoreFor(pawn) || part.Matches(pawn));
				case LogicGateType.Or:
					return this._innerFilter.Parts.Any((part) => !part.IgnoreFor(pawn) && part.Matches(pawn));
				case LogicGateType.Not:
				case LogicGateType.Nor:
					return this._innerFilter.Parts.All((part) => part.IgnoreFor(pawn) || !part.Matches(pawn));
				case LogicGateType.Xor:
					return this._innerFilter.Parts.Where((part) => !part.IgnoreFor(pawn)).Count((part) => part.Matches(pawn)) == 1;
				case LogicGateType.Nand:
					return this._innerFilter.Parts.Any((part) => !part.IgnoreFor(pawn) && !part.Matches(pawn));
				default:
					PrepareModeratelyLogger.LogErrorMessage("Invalid logic gate type.");
					return false;
			}
		}

		public override string Summary(PawnFilter filter) {
			if (this._innerFilter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to match pawn with logic gate with null inner filter.");
				return "null";
			}

			if (this._innerFilter.Parts is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to match pawn with logic gate with inner filter with null parts.");
				return "null";
			}

			// Logic gates with no inner filter parts don't do anything, so don't add them to the summary.
			if (this._innerFilter.Parts.Count < 1) {
				return "";
			}

#if V1_0
			string output = "";
			switch (this._type) {
				case LogicGateType.And:
					output = "PM.AllOf".Translate() ?? "null";
					break;
				case LogicGateType.Or:
					output = "PM.AtLeastOneOf".Translate() ?? "null";
					break;
				case LogicGateType.Not:
				case LogicGateType.Nor:
					output = "PM.NoneOf".Translate() ?? "null";
					break;
				case LogicGateType.Xor:
					output = "PM.ExactlyOneOf".Translate() ?? "null";
					break;
				case LogicGateType.Nand:
					output = "PM.NotAllOf".Translate() ?? "null";
					break;
				default:
					throw new NotImplementedException();
			}
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			string output = "";
			switch (this._type) {
				case LogicGateType.And:
					output = "PM.AllOf".Translate().StringSafe();
					break;
				case LogicGateType.Or:
					output = "PM.AtLeastOneOf".Translate().StringSafe();
					break;
				case LogicGateType.Not:
				case LogicGateType.Nor:
					output = "PM.NoneOf".Translate().StringSafe();
					break;
				case LogicGateType.Xor:
					output = "PM.ExactlyOneOf".Translate().StringSafe();
					break;
				case LogicGateType.Nand:
					output = "PM.NotAllOf".Translate().StringSafe();
					break;
				default:
					throw new NotImplementedException();
			}
#else
			string output = this._type switch {
				LogicGateType.And => "PM.AllOf".Translate().StringSafe,
				LogicGateType.Or => "PM.AtLeastOneOf".Translate().StringSafe,
				LogicGateType.Not or LogicGateType.Nor => "PM.NoneOf".Translate().StringSafe,
				LogicGateType.Xor => "PM.ExactlyOneOf".Translate().StringSafe,
				LogicGateType.Nand => "PM.NotAllOf".Translate().StringSafe,
				_ => throw new NotImplementedException(),
			};
#endif

			string indentation = "\t";
			for (int i = 0; i < IndentationLevel(filter, this); i++) {
				indentation += "\t";
			}

			bool empty = true;
			foreach (PawnFilterPart part in this._innerFilter.Parts.OrderByDescending((part) => part.Def?.SummaryPriority).ThenBy((part) => part.Def?.defName).Where((part) => part.Visible)) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				string summary = part.Summary(filter).CapitalizeFirstSafe().EndWithPeriodSafe(); // Don't use the inner filter - needs broader context to properly determine nested indentation levels.
#else
				string summary = part.Summary(filter).CapitalizeFirstSafe.EndWithPeriodSafe; // Don't use the inner filter - needs broader context to properly determine nested indentation levels.
#endif
				if (summary.NullOrEmpty()) {
					continue;
				}

				output += $"\n{indentation}{summary}";
				empty = false;
			}

			// If the logic gate only contains empty logic gates, it still doesn't do anything so still don't add it to the summary.
			if (empty) {
				return "";
			}

			return output;
		}
	}
}
