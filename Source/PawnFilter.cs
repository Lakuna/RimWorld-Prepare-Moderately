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

		// Able/not able to do each skill.
		// 0-20 and no/minor/major passion for each skill.
		// No relationships/has certain relationship.
		// No medical conditions.
		// Has/doesn't have specific trait.

		// Constructor.

		// public bool PawnMatchesFilter(Pawn pawn) { }
	}
}
