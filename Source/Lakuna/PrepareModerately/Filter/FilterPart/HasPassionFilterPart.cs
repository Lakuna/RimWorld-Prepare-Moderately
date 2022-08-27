using System;
using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasPassionFilterPart : FilterPart {
		public SkillDef skill;

		public Passion passion;

		public override bool Matches(Pawn pawn) {
			return pawn.skills.GetSkill(this.skill).passion == this.passion;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight * 2);

			Rect skillRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(skillRect, this.skill.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(DefDatabase<SkillDef>.AllDefsListForReading,
					(SkillDef def) => def.ToString().CapitalizeFirst(),
					(SkillDef def) => () => this.skill = def);
			}

			Rect passionRect = new Rect(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(passionRect, this.passion.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((Passion[])Enum.GetValues(typeof(Passion)),
					(Passion passion) => passion.ToString().CapitalizeFirst(),
					(Passion passion) => () => this.passion = passion);
			}
		}

		public override string Summary(Filter filter) {
			return "Has a " + this.passion.ToString() + " passion for " + this.skill.ToString() + ".";
		}

		public override void Randomize() {
			switch (Rand.RangeInclusive(0, 2)) {
				case 0:
					this.passion = Passion.None;
					break;
				case 1:
					this.passion = Passion.Minor;
					break;
				case 2:
					this.passion = Passion.Major;
					break;
			}

			this.skill = DefDatabase<SkillDef>.AllDefsListForReading[Rand.RangeInclusive(0, DefDatabase<SkillDef>.AllDefsListForReading.Count - 1)];
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.passion, nameof(this.passion));
			Scribe_Defs.Look(ref this.skill, nameof(this.skill));
		}
	}
}
