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
		public static PawnFilterPartDef NoMedicalConditions;

		static PawnFilterPartDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterPartDefOf));
	}
}
