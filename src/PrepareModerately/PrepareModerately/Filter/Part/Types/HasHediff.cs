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
	public sealed class HasHediff : PawnFilterPart {
		private const int _labelGap = 4;

		private static IEnumerable<BodyPartDef> _legalBodyParts =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			DefDatabase<BodyPartDef>.AllDefs ?? new List<BodyPartDef>();
#else
			DefDatabase<BodyPartDef>.AllDefs ?? [];
#endif

		private static IEnumerable<ChemicalDef> _legalChemicals =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			DefDatabase<ChemicalDef>.AllDefs ?? new List<ChemicalDef>();
#else
			DefDatabase<ChemicalDef>.AllDefs ?? [];
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static Dictionary<string, Func<Hediff, bool>> _legalConditions =>
			new Dictionary<string, Func<Hediff, bool>>() {
#else
		private static Dictionary<string, Func<Hediff, bool>?> _legalConditions =>
			new() {
#endif
#if !V1_0
				["PM.Chronic"] = (hediff) => hediff.def.chronic,
#endif
#if !(V1_0 || V1_1)
				["PM.CountsAsAddedPartOrImplant"] = (hediff) => hediff.def.countsAsAddedPartOrImplant,
#endif
				["PM.IsAddiction"] = (hediff) => hediff.def.IsAddiction,
				["PM.IsBad"] = (hediff) => hediff.def.isBad,
				["PM.IsPermanent"] = (hediff) => hediff.IsPermanent(),
				["PM.MakesSickThought"] = (hediff) => hediff.def.makesSickThought,
#if !(V1_0 || V1_1 || V1_2 || V1_3)
				["PM.Pregnant"] = (hediff) => hediff.def.pregnant,
				["PM.PreventsPregnancy"] = (hediff) => hediff.def.preventsPregnancy,
#endif
				["PM.Tendable"] = (hediff) => hediff.def.tendable
			};

		private static IEnumerable<HediffDef> _legalHediffs =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			DefDatabase<HediffDef>.AllDefs ?? new List<HediffDef>();
#else
			DefDatabase<HediffDef>.AllDefs ?? [];
#endif

		private const int _widgetRowMaxWidth = 24;

		public override IEnumerable<string> ConfigErrors {
			get {
				foreach (string item in base.ConfigErrors) {
					yield return item;
				}

				if (this._conditions is null) {
					yield return "Has hediff filter part keys is null.";
				}
			}
		}

		public override bool Valid =>
			base.Valid
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			&& this._conditions != null;
#else
			&& this._conditions is not null;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private BodyPartDef _bodyPart;
#else
		private BodyPartDef? _bodyPart;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private ChemicalDef _chemical;
#else
		private ChemicalDef? _chemical;
#endif

#pragma warning disable CA1002 // `Scribe_Collections.Look` must be used with a `List`.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private List<string> _conditions = new List<string>();
#else
		private List<string>? _conditions = [];
#endif
#pragma warning restore CA1002

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private HediffDef _hediff;
#else
		private HediffDef? _hediff;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static string GetUniqueLabelFor(BodyPartDef def) =>
#else
		private static string GetUniqueLabelFor(BodyPartDef? def) =>
#endif
			def is null ? "null"
			: (def.label is null || def.label.NullOrEmpty()) ? (def.defName ?? "null")
			: _legalBodyParts.Count((def2) => def.label == def2.label) > 1 ? $"{def.label} ({def.defName ?? "null"})"
			: def.label;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static string GetUniqueLabelFor(HediffDef def) =>
#else
		private static string GetUniqueLabelFor(HediffDef? def) =>
#endif
			def is null ? "null"
			: (def.label is null || def.label.NullOrEmpty()) ? (def.defName ?? "null")
			: _legalHediffs.Count((def2) => def.label == def2.label) > 1 ? $"{def.label} ({def.defName ?? "null"})"
			: def.label;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * (3 + (this._conditions?.Count ?? 0)), out totalAddedListHeight, out Rect headerRemainderRect).Rounded();

			if (this._conditions is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw edit interface of has hediff filter part with null conditions.");
				return;
			}

			if (Textures.Delete is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw null delete texture.");
				return;
			}

#if V1_0
			if (Widgets.ButtonText(headerRemainderRect, this._hediff is null ? ("PM.Any".Translate()?.CapitalizeFirstSafe() ?? "null") : GetUniqueLabelFor(this._hediff).CapitalizeFirstSafe())) {
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(headerRemainderRect, this._hediff is null ? "PM.Any".Translate().CapitalizeFirst().StringSafe() : GetUniqueLabelFor(this._hediff).CapitalizeFirstSafe())) {
#else
			if (Widgets.ButtonText(headerRemainderRect, this._hediff is null ? "PM.Any".Translate().CapitalizeFirst().StringSafe : GetUniqueLabelFor(this._hediff).CapitalizeFirstSafe)) {
#endif
				FloatMenuUtility.MakeMenu(
#if V1_0
					new List<HediffDef> { null }.Concat(_legalHediffs.OrderBy((def) => def.label)),
#else
					_legalHediffs.OrderBy((def) => def.label).Prepend(null),
#endif
#if V1_0
					(def) => def is null ? ("PM.Any".Translate()?.CapitalizeFirstSafe() ?? "null") : GetUniqueLabelFor(def).CapitalizeFirstSafe(),
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(def) => def is null ? "PM.Any".Translate().CapitalizeFirst().StringSafe() : GetUniqueLabelFor(def).CapitalizeFirstSafe(),
#else
					(def) => def is null ? "PM.Any".Translate().CapitalizeFirst().StringSafe : GetUniqueLabelFor(def).CapitalizeFirstSafe,
#endif
					(def) => () => this._hediff = def);
			}

			Rect bodyPartRect = new Rect(headerRemainderRect.x, rect.y, headerRemainderRect.width, Text.LineHeight).Rounded();
#if V1_0
			string bodyPartLabel = "PM.BodyPart".Translate()?.CapitalizeFirstSafe() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			string bodyPartLabel = "PM.BodyPart".Translate().CapitalizeFirst().StringSafe();
#else
			string bodyPartLabel = "PM.BodyPart".Translate().CapitalizeFirst().StringSafe;
#endif
			float bodyPartLabelWidth = Text.CalcSize(bodyPartLabel).x;
			Widgets.Label(new Rect(bodyPartRect.x - bodyPartLabelWidth - _labelGap, bodyPartRect.y, bodyPartLabelWidth, bodyPartRect.height).Rounded(), bodyPartLabel);
#if V1_0
			if (Widgets.ButtonText(bodyPartRect, this._bodyPart is null ? ("PM.Any".Translate()?.CapitalizeFirstSafe() ?? "null") : GetUniqueLabelFor(this._bodyPart).CapitalizeFirstSafe())) {
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(bodyPartRect, this._bodyPart is null ? "PM.Any".Translate().CapitalizeFirst().StringSafe() : GetUniqueLabelFor(this._bodyPart).CapitalizeFirstSafe())) {
#else
			if (Widgets.ButtonText(bodyPartRect, this._bodyPart is null ? "PM.Any".Translate().CapitalizeFirst().StringSafe : GetUniqueLabelFor(this._bodyPart).CapitalizeFirstSafe)) {
#endif
				FloatMenuUtility.MakeMenu(
#if V1_0
					new List<BodyPartDef> { null }.Concat(_legalBodyParts.OrderBy((def) => def.label)),
#else
					_legalBodyParts.OrderBy((def) => def.label).Prepend(null),
#endif
#if V1_0
					(def) => def is null ? ("PM.Any".Translate()?.CapitalizeFirstSafe() ?? "null") : GetUniqueLabelFor(def).CapitalizeFirstSafe(),
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(def) => def is null ? "PM.Any".Translate().CapitalizeFirst().StringSafe() : GetUniqueLabelFor(def).CapitalizeFirstSafe(),
#else
					(def) => def is null ? "PM.Any".Translate().CapitalizeFirst().StringSafe : GetUniqueLabelFor(def).CapitalizeFirstSafe,
#endif
					(def) => () => this._bodyPart = def);
			}

			Rect chemicalRect = new Rect(bodyPartRect.x, bodyPartRect.yMax, bodyPartRect.width, Text.LineHeight).Rounded();
#if V1_0
			string chemicalLabel = "PM.Chemical".Translate()?.CapitalizeFirstSafe() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			string chemicalLabel = "PM.Chemical".Translate().CapitalizeFirst().StringSafe();
#else
			string chemicalLabel = "PM.Chemical".Translate().CapitalizeFirst().StringSafe;
#endif
			float chemicalLabelWidth = Text.CalcSize(chemicalLabel).x;
			Widgets.Label(new Rect(chemicalRect.x - chemicalLabelWidth - _labelGap, chemicalRect.y, chemicalLabelWidth, chemicalRect.height).Rounded(), chemicalLabel);
#if V1_0
			if (Widgets.ButtonText(chemicalRect, (this._chemical?.LabelCap ?? "PM.Any".Translate()?.CapitalizeFirstSafe()) ?? "any")) {
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(chemicalRect, (this._chemical?.LabelCap ?? "PM.Any".Translate().CapitalizeFirst()).StringSafe())) {
#else
			if (Widgets.ButtonText(chemicalRect, (this._chemical?.LabelCap ?? "PM.Any".Translate().CapitalizeFirst()).StringSafe)) {
#endif
				FloatMenuUtility.MakeMenu(
#if V1_0
					new List<ChemicalDef> { null }.Concat(_legalChemicals.OrderBy((def) => def.label)),
#else
					_legalChemicals.OrderBy((def) => def.label).Prepend(null),
#endif
#if V1_0
					(def) => (def?.LabelCap ?? "PM.Any".Translate()?.CapitalizeFirstSafe()) ?? "any",
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(def) => (def?.LabelCap ?? "PM.Any".Translate().CapitalizeFirst()).StringSafe(),
#else
					(def) => (def?.LabelCap ?? "PM.Any".Translate().CapitalizeFirst()).StringSafe,
#endif
					(def) => () => this._chemical = def);
			}

			Rect addConditionRect = new Rect(rect.x, chemicalRect.yMax, rect.width, Text.LineHeight).Rounded();
#if V1_0
			if (Widgets.ButtonText(addConditionRect, "PM.AddCondition".Translate()?.CapitalizeFirstSafe() ?? "null")) {
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(addConditionRect, "PM.AddCondition".Translate().CapitalizeFirst().StringSafe())) {
#else
			if (Widgets.ButtonText(addConditionRect, "PM.AddCondition".Translate().CapitalizeFirst().StringSafe)) {
#endif
				FloatMenuUtility.MakeMenu(
					_legalConditions.Keys.OrderBy((key) => key),
#if V1_0
					(key) => key.Translate()?.CapitalizeFirstSafe() ?? "null",
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(key) => key.Translate().CapitalizeFirst().StringSafe(),
#else
					(key) => key.Translate().CapitalizeFirst().StringSafe,
#endif
					(key) => () => this._conditions.Add(key));
			}

			// List added conditions.
			float y = addConditionRect.yMax;
			foreach (string key in this._conditions.OrderBy((key) => key)) {
				Rect conditionRect = new Rect(rect.x, y, rect.width, Text.LineHeight).Rounded();
				y = conditionRect.yMax; // Increment `y` for next condition.

				// "Remove" button.
				Rect widgetRect = new Rect(conditionRect.x, conditionRect.y, _widgetRowMaxWidth, conditionRect.height).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				WidgetRow widgetRow = new WidgetRow(widgetRect.x, widgetRect.y, UIDirection.RightThenDown, widgetRect.width, 0);
#else
				WidgetRow widgetRow = new(widgetRect.x, widgetRect.y, UIDirection.RightThenDown, widgetRect.width, 0);
#endif
				if (widgetRow.ButtonIcon(Textures.Delete, null, GenUI.SubtleMouseoverColor)) {
					SoundDefOf.Click?.PlayOneShotOnCamera();
					_ = this._conditions.Remove(key);
				}

				// Label.
#if V1_0
				string label = key.Translate()?.CapitalizeFirstSafe() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				string label = key.Translate().CapitalizeFirst().StringSafe();
#else
				string label = key.Translate().CapitalizeFirst().StringSafe;
#endif
				Widgets.Label(new Rect(widgetRect.xMax, conditionRect.y, Text.CalcSize(label).x, conditionRect.height).Rounded(), label);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this._bodyPart, "bodyPart");
			Scribe_Defs.Look(ref this._chemical, "chemical");
			Scribe_Collections.Look(ref this._conditions, "conditions");
			Scribe_Defs.Look(ref this._hediff, "hediff");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ (this._conditions?.GetHashCode() ?? 0);

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		public override bool IgnoreFor(Pawn pawn) {
			if (base.IgnoreFor(pawn)) {
				return true;
			}

#if V1_4 || V1_5 || V1_6
			if (this._chemical != null
#else
			if (this._chemical is not null
#endif
				&& (pawn.genes?.GenesListForReading?.Any((gene) => gene?.def?.chemical == this._chemical) ?? false)) {
				return true;
			}

			if ((this._conditions?.Any((condition) => condition == "PM.IsAddiction") ?? false)
#if V1_4 || V1_5 || V1_6
				&& (pawn.genes?.GenesListForReading.Any((gene) => gene?.def?.chemical != null) ?? false)) {
#else
				&& (pawn.genes?.GenesListForReading.Any((gene) => gene?.def?.chemical is not null) ?? false)) {
#endif
				return true;
			}

#if V1_4 || V1_5 || V1_6
			if (this._hediff != null
#else
			if (this._hediff is not null
#endif
				&& (pawn.genes?.GenesListForReading?.Any((gene) =>
				(gene?.def?.makeImmuneTo?.Any((hediff) => this._hediff == hediff) ?? false)
				|| (gene?.def?.hediffGiversCannotGive?.Any((hediff) => this._hediff == hediff) ?? false)) ?? false)) {
				return true;
			}

			return false;
		}
#endif

		public override bool Matches(Pawn pawn) =>
			pawn.health?.hediffSet?.hediffs?.Any((hediff) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				hediff != null
#else
				hediff is not null
#endif
				&& (this._bodyPart is null || hediff?.Part?.def == this._bodyPart)
				&& (this._chemical is null
#if !(V1_0 || V1_1 || V1_2 || V1_3)
					|| hediff is Hediff_ChemicalDependency dependency && dependency.chemical == this._chemical
#endif
					|| hediff is Hediff_Addiction addiction && addiction.Chemical == this._chemical)
				&& (this._conditions?.All((condition) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					condition != null
#else
					condition is not null
#endif
					&& (_legalConditions.TryGetValue(condition)?.Invoke(hediff) ?? false)) ?? false)
				&& (this._hediff is null || hediff.def == this._hediff)) ?? false;

		public override void Randomize() {
			if (this._conditions is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to randomize has hediff filter part with null conditions.");
				return;
			}

			// Only set one type of condition when randomizing so that it always generates a filter part that can match.
			switch (Rand.Range(0, 3)) {
				case 0:
					this._bodyPart = _legalBodyParts.RandomElement();
					break;
				case 1:
					this._chemical = _legalChemicals.RandomElement();
					break;
				case 2:
					this._conditions.Clear();
					this._conditions.Add(_legalConditions.Keys.RandomElement());
					break;
				case 3:
					this._hediff = _legalHediffs.RandomElement();
					break;
				default:
					// Pure "has any hediff" filter part.
					break;
			}
		}

		public override string Summary(PawnFilter filter) =>
			(this._hediff is null ?
					this._bodyPart is null ?
						(this._conditions is null || this._conditions.Count < 1) ?
							this._chemical is null ? "PM.HasHediff".Translate()
							: "PM.HasHediffToChemical".Translate(this._chemical.label ?? "null")
						: this._chemical is null ? "PM.HasHediffWithConditions".Translate(string.Join(", ", this._conditions.Select((condition) => condition.Translate()).ToArray()))
						: "PM.HasHediffWithConditionsToChemical".Translate(string.Join(", ", this._conditions.Select((condition) => condition.Translate()).ToArray()), this._chemical.label ?? "null")
					: (this._conditions is null || this._conditions.Count < 1) ?
						this._chemical is null ? "PM.HasHediffOnPart".Translate(GetUniqueLabelFor(this._bodyPart))
						: "PM.HasHediffOnPartToChemical".Translate(GetUniqueLabelFor(this._bodyPart), this._chemical.label ?? "null")
					: this._chemical is null ? "PM.HasHediffOnPartWithConditions".Translate(GetUniqueLabelFor(this._bodyPart), string.Join(", ", this._conditions.Select((condition) => condition.Translate()).ToArray()))
					: "PM.HasHediffOnPartWithConditionsToChemical".Translate(GetUniqueLabelFor(this._bodyPart), string.Join(", ", this._conditions.Select((condition) => condition.Translate()).ToArray()), this._chemical.label ?? "null")
				: this._bodyPart is null ?
						(this._conditions is null || this._conditions.Count < 1) ?
							this._chemical is null ? "PM.HasTypeHediff".Translate(GetUniqueLabelFor(this._hediff))
							: "PM.HasTypeHediffToChemical".Translate(GetUniqueLabelFor(this._hediff), this._chemical.label ?? "null")
						: this._chemical is null ? "PM.HasTypeHediffWithConditions".Translate(GetUniqueLabelFor(this._hediff), string.Join(", ", this._conditions.Select((condition) => condition.Translate()).ToArray()))
						: "PM.HasTypeHediffWithConditionsToChemical".Translate(GetUniqueLabelFor(this._hediff), string.Join(", ", this._conditions.Select((condition) => condition.Translate()).ToArray()), this._chemical.label ?? "null")
					: (this._conditions is null || this._conditions.Count < 1) ?
						this._chemical is null ? "PM.HasTypeHediffOnPart".Translate(GetUniqueLabelFor(this._hediff), GetUniqueLabelFor(this._bodyPart))
						: "PM.HasTypeHediffOnPartToChemical".Translate(GetUniqueLabelFor(this._hediff), GetUniqueLabelFor(this._bodyPart), this._chemical.label ?? "null")
					: this._chemical is null ? "PM.HasTypeHediffOnPartWithConditions".Translate(GetUniqueLabelFor(this._hediff), GetUniqueLabelFor(this._bodyPart), string.Join(", ", this._conditions.Select((condition) => condition.Translate()).ToArray()))
					: "PM.HasTypeHediffOnPartWithConditionsToChemical".Translate(GetUniqueLabelFor(this._hediff), GetUniqueLabelFor(this._bodyPart), string.Join(", ", this._conditions.Select((condition) => condition.Translate()).ToArray()), this._chemical.label ?? "null"))
#if V1_0
				?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				.StringSafe();
#else
				.StringSafe;
#endif
	}
}
