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

		private static IEnumerable<BodyPartDef> _legalBodyParts {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				List<BodyPartDef> empty = new List<BodyPartDef>();
#else
				List<BodyPartDef> empty = [];
#endif
				return DefDatabase<BodyPartDef>.AllDefs ?? empty;
			}
		}

		private static IEnumerable<ChemicalDef> _legalChemicals {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				List<ChemicalDef> empty = new List<ChemicalDef>();
#else
				List<ChemicalDef> empty = [];
#endif
				return DefDatabase<ChemicalDef>.AllDefs ?? empty;
			}
		}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static Dictionary<string, Func<Hediff, bool>> _legalConditions =>
#else
		private static Dictionary<string, Func<Hediff, bool>?> _legalConditions =>
#endif
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new Dictionary<string, Func<Hediff, bool>>()
#else
			new()
#endif
			{
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

		private static IEnumerable<HediffDef> _legalHediffs {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				List<HediffDef> empty = new List<HediffDef>();
#else
				List<HediffDef> empty = [];
#endif
				return DefDatabase<HediffDef>.AllDefs ?? empty;
			}
		}

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
			&& !(this._conditions is null);
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

		private static string GetUniqueLabelFor(
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			BodyPartDef def
#else
			BodyPartDef? def
#endif
		) =>
			def is null ? "null"
			: (def.label is null || def.label.NullOrEmpty()) ? (def.defName ?? "null")
			: _legalBodyParts.Count((def2) => def.label == def2.label) > 1 ? $"{def.label} ({def.defName ?? "null"})"
			: def.label;

		private static string GetUniqueLabelFor(
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			HediffDef def
#else
			HediffDef? def
#endif
		) =>
			def is null ? "null"
			: (def.label is null || def.label.NullOrEmpty()) ? (def.defName ?? "null")
			: _legalHediffs.Count((def2) => def.label == def2.label) > 1 ? $"{def.label} ({def.defName ?? "null"})"
			: def.label;

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static bool IgnoreForHediff(Hediff hediff) =>
			hediff?.pawn?.genes?.GenesListForReading?.Any((gene) => gene is Gene_ChemicalDependency dependency && dependency.LinkedHediff == hediff) ?? false;
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * (3 + (this._conditions?.Count ?? 0)), out totalAddedListHeight, out Rect headerRemainderRect).Rounded();

			if (this._conditions is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw edit interface of has hediff filter part with null conditions.");
				return;
			}

			if (PrepareModeratelyTextures.Delete is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw null delete texture.");
				return;
			}

			if (Widgets.ButtonText(headerRemainderRect, this._hediff is null ? "PM.Any".TranslateFull(CapitalizeType.First) : GetUniqueLabelFor(this._hediff).Finish(CapitalizeType.First))) {
				FloatMenuUtility.MakeMenu(
#if V1_0
					new List<HediffDef> { null }.Concat(_legalHediffs.OrderBy((def) => def.label)),
#else
					_legalHediffs.OrderBy((def) => def.label).Prepend(null),
#endif
					(def) => def is null ? "PM.Any".TranslateFull(CapitalizeType.First) : GetUniqueLabelFor(def).Finish(CapitalizeType.First),
					(def) => () => this._hediff = def);
			}

			Rect bodyPartRect = new Rect(headerRemainderRect.x, rect.y, headerRemainderRect.width, Text.LineHeight).Rounded();
			string bodyPartLabel = "PM.BodyPart".TranslateFull(CapitalizeType.First);
			float bodyPartLabelWidth = Text.CalcSize(bodyPartLabel).x;
			Widgets.Label(new Rect(bodyPartRect.x - bodyPartLabelWidth - _labelGap, bodyPartRect.y, bodyPartLabelWidth, bodyPartRect.height).Rounded(), bodyPartLabel);
			if (Widgets.ButtonText(bodyPartRect, this._bodyPart is null ? "PM.Any".TranslateFull(CapitalizeType.First) : GetUniqueLabelFor(this._bodyPart).Finish(CapitalizeType.First))) {
				FloatMenuUtility.MakeMenu(
#if V1_0
					new List<BodyPartDef> { null }.Concat(_legalBodyParts.OrderBy((def) => def.label)),
#else
					_legalBodyParts.OrderBy((def) => def.label).Prepend(null),
#endif
					(def) => def is null ? "PM.Any".TranslateFull(CapitalizeType.First) : GetUniqueLabelFor(def).Finish(CapitalizeType.First),
					(def) => () => this._bodyPart = def);
			}

			Rect chemicalRect = new Rect(bodyPartRect.x, bodyPartRect.yMax, bodyPartRect.width, Text.LineHeight).Rounded();
			string chemicalLabel = "PM.Chemical".TranslateFull(CapitalizeType.First);
			float chemicalLabelWidth = Text.CalcSize(chemicalLabel).x;
			Widgets.Label(new Rect(chemicalRect.x - chemicalLabelWidth - _labelGap, chemicalRect.y, chemicalLabelWidth, chemicalRect.height).Rounded(), chemicalLabel);
			if (Widgets.ButtonText(chemicalRect, this._chemical is null ? "PM.Any".TranslateFull(CapitalizeType.First) : this._chemical.LabelCap.Finish())) {
				FloatMenuUtility.MakeMenu(
#if V1_0
					new List<ChemicalDef> { null }.Concat(_legalChemicals.OrderBy((def) => def.label)),
#else
					_legalChemicals.OrderBy((def) => def.label).Prepend(null),
#endif
					(def) => def is null ? "PM.Any".TranslateFull(CapitalizeType.First) : def.LabelCap.Finish(),
					(def) => () => this._chemical = def);
			}

			Rect addConditionRect = new Rect(rect.x, chemicalRect.yMax, rect.width, Text.LineHeight).Rounded();
			if (Widgets.ButtonText(addConditionRect, "PM.AddCondition".TranslateFull(CapitalizeType.First))) {
				FloatMenuUtility.MakeMenu(
					_legalConditions.Keys.OrderBy((key) => key),
					(key) => key.TranslateFull(CapitalizeType.First),
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
				if (widgetRow.ButtonIcon(PrepareModeratelyTextures.Delete, null, GenUI.SubtleMouseoverColor)) {
					SoundDefOf.Click?.PlayOneShotOnCamera();
					_ = this._conditions.Remove(key);
				}

				// Label.
				string label = key.TranslateFull(CapitalizeType.First);
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
		public override bool IgnoreFor(Pawn pawn) =>
			base.IgnoreFor(pawn)
			|| (pawn.health?.hediffSet?.hediffs?.Where(this.MatchesHediff).All(IgnoreForHediff) ?? false);
#endif

		public override bool Matches(Pawn pawn) =>
			pawn.health?.hediffSet?.hediffs?.Any(this.MatchesHediff) ?? false;

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
			this._hediff is null ?
				this._bodyPart is null ?
					(this._conditions is null || this._conditions.Count < 1) ?
						this._chemical is null ? "PM.HasHediff".TranslateFull()
						: "PM.HasHediffToChemical".TranslateFull(this._chemical.label ?? "null")
					: this._chemical is null ? "PM.HasHediffWithConditions".TranslateFull(string.Join(", ", this._conditions.Select((condition) => condition.TranslateFull()).ToArray()))
					: "PM.HasHediffWithConditionsToChemical".TranslateFull(string.Join(", ", this._conditions.Select((condition) => condition.TranslateFull()).ToArray()), this._chemical.label ?? "null")
				: (this._conditions is null || this._conditions.Count < 1) ?
					this._chemical is null ? "PM.HasHediffOnPart".TranslateFull(GetUniqueLabelFor(this._bodyPart))
					: "PM.HasHediffOnPartToChemical".TranslateFull(GetUniqueLabelFor(this._bodyPart), this._chemical.label ?? "null")
				: this._chemical is null ? "PM.HasHediffOnPartWithConditions".TranslateFull(GetUniqueLabelFor(this._bodyPart), string.Join(", ", this._conditions.Select((condition) => condition.TranslateFull()).ToArray()))
				: "PM.HasHediffOnPartWithConditionsToChemical".TranslateFull(GetUniqueLabelFor(this._bodyPart), string.Join(", ", this._conditions.Select((condition) => condition.TranslateFull()).ToArray()), this._chemical.label ?? "null")
			: this._bodyPart is null ?
					(this._conditions is null || this._conditions.Count < 1) ?
						this._chemical is null ? "PM.HasTypeHediff".TranslateFull(GetUniqueLabelFor(this._hediff))
						: "PM.HasTypeHediffToChemical".TranslateFull(GetUniqueLabelFor(this._hediff), this._chemical.label ?? "null")
					: this._chemical is null ? "PM.HasTypeHediffWithConditions".TranslateFull(GetUniqueLabelFor(this._hediff), string.Join(", ", this._conditions.Select((condition) => condition.TranslateFull()).ToArray()))
					: "PM.HasTypeHediffWithConditionsToChemical".TranslateFull(GetUniqueLabelFor(this._hediff), string.Join(", ", this._conditions.Select((condition) => condition.TranslateFull()).ToArray()), this._chemical.label ?? "null")
				: (this._conditions is null || this._conditions.Count < 1) ?
					this._chemical is null ? "PM.HasTypeHediffOnPart".TranslateFull(GetUniqueLabelFor(this._hediff), GetUniqueLabelFor(this._bodyPart))
					: "PM.HasTypeHediffOnPartToChemical".TranslateFull(GetUniqueLabelFor(this._hediff), GetUniqueLabelFor(this._bodyPart), this._chemical.label ?? "null")
				: this._chemical is null ? "PM.HasTypeHediffOnPartWithConditions".TranslateFull(GetUniqueLabelFor(this._hediff), GetUniqueLabelFor(this._bodyPart), string.Join(", ", this._conditions.Select((condition) => condition.TranslateFull()).ToArray()))
				: "PM.HasTypeHediffOnPartWithConditionsToChemical".TranslateFull(GetUniqueLabelFor(this._hediff), GetUniqueLabelFor(this._bodyPart), string.Join(", ", this._conditions.Select((condition) => condition.TranslateFull()).ToArray()), this._chemical.label ?? "null");

		private bool MatchesHediff(Hediff hediff) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			!(hediff is null)
#else
			hediff is not null
#endif
			&& (this._bodyPart is null || hediff.Part?.def == this._bodyPart)
			&& (this._chemical is null
#if !(V1_0 || V1_1 || V1_2 || V1_3)
				|| hediff is Hediff_ChemicalDependency dependency && dependency.chemical == this._chemical
#endif
				|| hediff is Hediff_Addiction addiction && addiction.Chemical == this._chemical)
			&& (this._conditions?.All((condition) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				!(condition is null)
#else
				condition is not null
#endif
				&& (_legalConditions.TryGetValue(condition)?.Invoke(hediff) ?? false)) ?? false)
			&& (this._hediff is null || hediff.def == this._hediff);
	}
}
