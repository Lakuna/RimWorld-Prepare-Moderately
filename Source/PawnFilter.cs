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
		// List of traits.
		// 0-20 and no/minor/major passion for each skill.
		// List of childhood/adulthoods.
		// No relationships.
		// No medical differences.

		// Constructor.

		// public bool PawnMatchesFilter(Pawn pawn) { }
	}
}
