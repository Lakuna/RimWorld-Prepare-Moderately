using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasPassion : PawnFilterPart {
#pragma warning restore CA1812
		private static IEnumerable<Passion> _legalPassions =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(Passion)).OfType<Passion>();
#else
			Enum.GetValues<Passion>().OfType<Passion>();
#endif

		private static IEnumerable<SkillDef> _legalSkills =>
			DefDatabase<SkillDef>.AllDefs;

#pragma warning disable IDE1006 // `Def` relies on static fields with names matching XML definitions.
		private Passion passion;

		private SkillDef skill;
#pragma warning restore IDE1006

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static bool PassionIsPossibleWithModifier(Passion passion, PassionMod.PassionModType modifier) =>
			modifier == PassionMod.PassionModType.AddOneLevel ? passion != Passion.None
			: modifier != PassionMod.PassionModType.DropAll || passion == Passion.None;
#endif

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.passion, nameof(this.passion));
			Scribe_Defs.Look(ref this.skill, nameof(this.skill));
		}

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
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
				FloatMenuUtility.MakeMenu(_legalSkills.OrderBy((def) => def.label),
					(def) => def.LabelCap,
					(def) => () => this.skill = def);
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect passionRect = new Rect(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
#else
			Rect passionRect = new(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
#endif
			if (Widgets.ButtonText(passionRect, this.passion.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(_legalPassions.OrderBy((passion) => passion.ToString()),
					(passion) => passion.ToString().CapitalizeFirst(),
					(passion) => () => this.passion = passion);
			}
		}

		internal override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
#if V1_0 || V1_1 || V1_2 || V1_3
			: pawn.skills.GetSkill(this.skill).passion == this.passion;
#else
			: pawn.skills.GetSkill(this.skill).passion == this.passion
				|| pawn.genes.GenesListForReading.Any((gene) => gene.def.passionMod != null && gene.def.passionMod.skill == this.skill && !PassionIsPossibleWithModifier(this.passion, gene.def.passionMod.modType));
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		// Override NOT gate functionality to disregard the gene override.
		internal override bool NotMatches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
			: pawn.skills.GetSkill(this.skill).passion != this.passion
				|| this.passion == Passion.None && pawn.genes.GenesListForReading.Any((gene) => gene.def.passionMod != null && gene.def.passionMod.skill == this.skill && !PassionIsPossibleWithModifier(Passion.Minor, gene.def.passionMod.modType));
#endif

		internal override void Randomize() {
			this.passion = _legalPassions.RandomElement();
			this.skill = _legalSkills.RandomElement();
		}

		internal override string Summary(PawnFilter filter) =>
			"PM.HasPassionForSkill".Translate(this.passion.ToString(), this.skill.ToString());
	}
}
