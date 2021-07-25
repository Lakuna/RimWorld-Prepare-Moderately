using RimWorld;

namespace PrepareModerately.PawnFilter {
	[DefOf]
	public static class PawnFilterPartDefOf {
		public static PawnFilterPartDef HasAnyAddiction;
		public static PawnFilterPartDef HasAnyBodyModification;
		public static PawnFilterPartDef HasAnyPermanentMedicalCondition;
		public static PawnFilterPartDef HasAnyRelationship;
		public static PawnFilterPartDef HasMinimumPassionsAtLevel;
		public static PawnFilterPartDef HasMinimumSkillsAtLevel;
		public static PawnFilterPartDef HasPassionValue;
		public static PawnFilterPartDef HasRelationship;
		public static PawnFilterPartDef HasSkillLevelMinimum;
		public static PawnFilterPartDef HasTrait;
		public static PawnFilterPartDef IsCapableOf;
		public static PawnFilterPartDef IsCapableOfEverything;
		public static PawnFilterPartDef IsGender;
		public static PawnFilterPartDef IsInAgeRange;
		public static PawnFilterPartDef IsSpecies;
		public static PawnFilterPartDef LogicGate;
		public static PawnFilterPartDef NameContains;

		static PawnFilterPartDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterPartDefOf));
	}
}
