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
		private static List<ThingDef> allHumanlikeDefs;

		public static List<ThingDef> AllHumanlikeDefs {
			get {
				if (allHumanlikeDefs == null) {
					Log.Message("Getting humanlikes list.");
					allHumanlikeDefs = new List<ThingDef>();
					foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading) {
						if (def.race != null && def.race.Humanlike) {
							allHumanlikeDefs.Add(def);
							Log.Message(def.LabelCap + " is humanlike.");
						}
					}
				}
				return allHumanlikeDefs;
			}
		}

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
