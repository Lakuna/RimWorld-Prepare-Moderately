using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasPassion : PawnFilterPart {
		private SkillDef skill;

		private Passion passion;

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static bool PassionIsPossibleWithModifier(Passion passion, PassionMod.PassionModType modifier) => modifier == PassionMod.PassionModType.AddOneLevel
			? passion != Passion.None
			: modifier != PassionMod.PassionModType.DropAll || passion == Passion.None;
#endif

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.skills.GetSkill(this.skill).passion == this.passion
#if !(V1_0 || V1_1 || V1_2 || V1_3)
			|| pawn.genes.GenesListForReading.Any((gene) =>
				gene.def.passionMod != null
				&& gene.def.passionMod.skill == this.skill
				&& !PassionIsPossibleWithModifier(this.passion, gene.def.passionMod.modType))
#endif
			;

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		// Override NOT gate functionality to disregard the gene override.
		public override bool NotMatches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.skills.GetSkill(this.skill).passion != this.passion
			|| this.passion == Passion.None
			&& pawn.genes.GenesListForReading.Any((gene) =>
				gene.def.passionMod != null
				&& gene.def.passionMod.skill == this.skill
				&& !PassionIsPossibleWithModifier(Passion.Minor, gene.def.passionMod.modType));
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2, out totalAddedListHeight);

			Rect skillRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(skillRect, this.skill.LabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<SkillDef>.AllDefsListForReading, (def) => def.LabelCap, (def) => () => this.skill = def);
			}

			Rect passionRect = new Rect(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(passionRect, this.passion.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((Passion[])Enum.GetValues(typeof(Passion)), (passion) => passion.ToString().CapitalizeFirst(), (passion) => () => this.passion = passion);
			}
		}

		public override string Summary(PawnFilter filter) => "HasPassionForSkill".Translate(this.passion.ToString(), this.skill.ToString());

		public override void Randomize() {
			this.passion = GetRandomOfEnum(new Passion());
			this.skill = DefDatabase<SkillDef>.AllDefsListForReading.RandomElement();
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.passion, nameof(this.passion));
			Scribe_Defs.Look(ref this.skill, nameof(this.skill));
		}
	}
}
