using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasPassion : PawnFilterPart {
		private static IEnumerable<Passion> _allPassions =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(Passion)).OfType<Passion>();
#else
			Enum.GetValues<Passion>().OfType<Passion>();
#endif

		private static IEnumerable<SkillDef> _allSkills =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			DefDatabase<SkillDef>.AllDefs ?? new List<SkillDef>();
#else
			DefDatabase<SkillDef>.AllDefs ?? [];
#endif

		public override IEnumerable<string> ConfigErrors {
			get {
				foreach (string item in base.ConfigErrors) {
					yield return item;
				}

				if (this._skill is null) {
					yield return "Has skill filter part skill is null.";
				}
			}
		}

		public override bool HasNullDefs =>
			base.HasNullDefs || this._skill is null;

		private Passion _passion;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private SkillDef _skill;
#else
		private SkillDef? _skill;
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static bool PassionIsPossibleWithModifier(Passion passion, PassionMod.PassionModType modifier) =>
			(modifier == PassionMod.PassionModType.AddOneLevel) ? (passion != Passion.None)
			: (modifier != PassionMod.PassionModType.DropAll || passion == Passion.None);
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2, out totalAddedListHeight).Rounded();

			Rect skillRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight).Rounded();
			if (Widgets.ButtonText(skillRect, this._skill?.LabelCap.Finish() ?? "null")) {
				FloatMenuUtility.MakeMenu(
					_allSkills.OrderBy((def) => def.label),
					(def) => def.LabelCap.Finish(),
					(def) => () => this._skill = def);
			}

			if (Widgets.ButtonText(new Rect(rect.x, skillRect.yMax, rect.width, Text.LineHeight).Rounded(), this._passion.ToString().Finish(CapitalizeType.First))) {
				FloatMenuUtility.MakeMenu(
					_allPassions.OrderBy((passion) => passion.ToString()),
					(passion) => passion.ToString().Finish(CapitalizeType.First),
					(passion) => () => this._passion = passion);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._passion, "passion");
			Scribe_Defs.Look(ref this._skill, "skill");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ this._passion.GetHashCode() ^ (this._skill?.GetHashCode() ?? 0);

#if V1_0 || V1_1 || V1_2 || V1_3
#elif V1_4 || V1_5 || V1_6
		public override bool IgnoreFor(Pawn pawn) =>
			base.IgnoreFor(pawn)
			|| this._skill is null
			|| (pawn.genes?.GenesListForReading?.Any((gene) =>
				!(gene?.def?.passionMod is null)
				&& gene.def.passionMod.skill == this._skill
				&& !PassionIsPossibleWithModifier(this._passion, gene.def.passionMod.modType)) ?? false);
#else
		public override bool IgnoreFor(Pawn pawn) =>
			base.IgnoreFor(pawn)
			|| this._skill is null
			|| (pawn.genes?.GenesListForReading?.Any((gene) =>
				gene?.def?.passionMod is not null
				&& gene.def.passionMod.skill == this._skill
				&& !PassionIsPossibleWithModifier(this._passion, gene.def.passionMod.modType)) ?? false);
#endif

		public override bool Matches(Pawn pawn) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			!(this._skill is null)
#else
			this._skill is not null
#endif
			&& pawn.skills?.GetSkill(this._skill)?.passion == this._passion;

		public override void Randomize() {
			this._passion = _allPassions.RandomElement();
			this._skill = _allSkills.RandomElement();
		}

		public override string Summary(PawnFilter filter) =>
			"PM.HasPassionForSkill".TranslateFull(this._passion.ToString(), this._skill?.label ?? "null");
	}
}
