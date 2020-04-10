using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace PrepareModerately {
	public class PawnFilter {
		public static List<SkillDef> allSkills = DefDatabase<SkillDef>.AllDefsListForReading;
		public static List<PawnRelationDef> allRelations = DefDatabase<PawnRelationDef>.AllDefsListForReading;
		public static List<TraitDef> allTraits = DefDatabase<TraitDef>.AllDefsListForReading;
		public static List<WorkTypeDef> allWorkTypes = DefDatabase<WorkTypeDef>.AllDefsListForReading;
		public static List<StatDef> allStats = DefDatabase<StatDef>.AllDefsListForReading;

		private readonly List<IFilterPart> parts;

		// Able/not able to do each skill.
		// 0-20 and no/minor/major passion for each skill.
		// No relationships/has certain relationship.
		// No medical conditions.
		// Has/doesn't have specific trait.
		// Age below/above.

		public PawnFilter() => this.parts = new List<IFilterPart>();

		public void AddPart(IFilterPart part) => this.parts.Add(part);

		public bool Matches(Pawn pawn) {
			foreach (IFilterPart part in this.parts) {
				if (!part.Matches(pawn)) { return false; }
			}
			return true;
		}
	}
}
