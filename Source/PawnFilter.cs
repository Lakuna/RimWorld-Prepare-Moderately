using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PrepareModerately {
	public class PawnFilter {
		public static List<SkillDef> allSkills = DefDatabase<SkillDef>.AllDefsListForReading;
		public static List<PawnRelationDef> allRelations = DefDatabase<PawnRelationDef>.AllDefsListForReading;
		public static List<TraitDef> allTraits = DefDatabase<TraitDef>.AllDefsListForReading;
		public static List<WorkTypeDef> allWorkTypes = DefDatabase<WorkTypeDef>.AllDefsListForReading;
		public static List<StatDef> allStats = DefDatabase<StatDef>.AllDefsListForReading;
		public static List<PawnFilterPartDef> allFilterParts = DefDatabase<PawnFilterPartDef>.AllDefsListForReading;

		public readonly List<PawnFilterPart> parts;

		public PawnFilter() => this.parts = new List<PawnFilterPart>();

		public bool Matches(Pawn pawn) {
			foreach (PawnFilterPart part in this.parts) {
				if (!part.Matches(pawn)) { return false; }
			}
			return true;
		}
	}
}
