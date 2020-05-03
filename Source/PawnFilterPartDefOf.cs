using RimWorld;

namespace PrepareModerately {
	[DefOf]
	public static class PawnFilterPartDefOf {
		public static PawnFilterPartDef NameContains;
		public static PawnFilterPartDef CapableOf;
		public static PawnFilterPartDef SkillLevelMinimum;
		public static PawnFilterPartDef PassionMinimum;
		public static PawnFilterPartDef AgeMaximum;
		public static PawnFilterPartDef RequiredTrait;
		public static PawnFilterPartDef DisallowedTrait;
		public static PawnFilterPartDef NoPermanentMedicalConditions;
		public static PawnFilterPartDef NoAddictions;
		public static PawnFilterPartDef AgeMinimum;
		public static PawnFilterPartDef CapableOfEverything;
		public static PawnFilterPartDef HasRelationship;
		public static PawnFilterPartDef NotHasRelationship;
		public static PawnFilterPartDef NoRelationships;
		public static PawnFilterPartDef IsSpecies;
		public static PawnFilterPartDef MinimumInterests;
		public static PawnFilterPartDef WellRounded;

		static PawnFilterPartDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterPartDefOf));
	}
}
