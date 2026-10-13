using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasPassion : PawnFilterPart {
		private static IEnumerable<SkillDef> LegalSkills => DefDatabase<SkillDef>.AllDefs;

		private static IEnumerable<Passion> LegalPassions =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(Passion)).OfType<Passion>();
#else
			Enum.GetValues<Passion>().OfType<Passion>();
#endif

		private SkillDef skill;

		private Passion passion;

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static bool PassionIsPossibleWithModifier(Passion passion, PassionMod.PassionModType modifier) => modifier == PassionMod.PassionModType.AddOneLevel
			? passion != Passion.None
			: modifier != PassionMod.PassionModType.DropAll || passion == Passion.None;
#endif

		public override bool Matches(Pawn pawn) => pawn is null
			? throw new ArgumentNullException(nameof(pawn))
#if V1_0 || V1_1 || V1_2 || V1_3
			: pawn.skills.GetSkill(this.skill).passion == this.passion;
#else
			: pawn.skills.GetSkill(this.skill).passion == this.passion
			|| pawn.genes.GenesListForReading.Any((gene) =>
				gene.def.passionMod != null
				&& gene.def.passionMod.skill == this.skill
				&& !PassionIsPossibleWithModifier(this.passion, gene.def.passionMod.modType));
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		// Override NOT gate functionality to disregard the gene override.
		public override bool NotMatches(Pawn pawn) => pawn is null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.skills.GetSkill(this.skill).passion != this.passion
			|| this.passion == Passion.None
			&& pawn.genes.GenesListForReading.Any((gene) =>
				gene.def.passionMod != null
				&& gene.def.passionMod.skill == this.skill
				&& !PassionIsPossibleWithModifier(Passion.Minor, gene.def.passionMod.modType));
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2, out totalAddedListHeight);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect skillRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
#else
			Rect skillRect = new(rect.x, rect.y, rect.width, Text.LineHeight);
#endif
			if (Widgets.ButtonText(skillRect, this.skill.LabelCap)) {
				FloatMenuUtility.MakeMenu(LegalSkills.OrderBy((def) => def.label),
					(def) => def.LabelCap,
					(def) => () => this.skill = def);
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect passionRect = new Rect(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
#else
			Rect passionRect = new(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
#endif
			if (Widgets.ButtonText(passionRect, this.passion.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(LegalPassions.OrderBy((passion) => passion.ToString()),
					(passion) => passion.ToString().CapitalizeFirst(),
					(passion) => () => this.passion = passion);
			}
		}

		public override string Summary(PawnFilter filter) => "PM.HasPassionForSkill".Translate(this.passion.ToString(), this.skill.ToString());

		public override void Randomize() {
			this.passion = LegalPassions.RandomElement();
			this.skill = LegalSkills.RandomElement();
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.passion, nameof(this.passion));
			Scribe_Defs.Look(ref this.skill, nameof(this.skill));
		}
	}
}
