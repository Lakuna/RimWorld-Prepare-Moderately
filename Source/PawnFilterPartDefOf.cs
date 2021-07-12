using RimWorld;

namespace PrepareModerately {
	[DefOf]
	public static class PawnFilterPartDefOf {
		public static PawnFilterPartDef AgeRange;
		public static PawnFilterPartDef CapableOf;
		public static PawnFilterPartDef CapableOfEverything;
		public static PawnFilterPartDef Gender;
		public static PawnFilterPartDef HasRelationship;
		public static PawnFilterPartDef IsSpecies;
		public static PawnFilterPartDef LogicGate;
		public static PawnFilterPartDef MinimumInterests;
		public static PawnFilterPartDef NameContains;
		public static PawnFilterPartDef NoAddictions;
		public static PawnFilterPartDef NoPermanentMedicalConditions;
		public static PawnFilterPartDef NoRelationships;
		public static PawnFilterPartDef PassionValue;
		public static PawnFilterPartDef RequiredTrait;
		public static PawnFilterPartDef SkillLevelMinimum;
		public static PawnFilterPartDef WellRounded;

		static PawnFilterPartDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterPartDefOf));
	}
}
