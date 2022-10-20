using RimWorld;

namespace Lakuna.PrepareModerately.Filter.Part {
	[DefOf]
	public static class PawnFilterPartDefOf {
		public static PawnFilterPartDef HasAdulthood { get; }

		public static PawnFilterPartDef HasAnyAddiction { get; }

		public static PawnFilterPartDef HasAnyBodyModification { get; }

		public static PawnFilterPartDef HasAnyPermanentMedicalCondition { get; }

		public static PawnFilterPartDef HasAnyRelation { get; }

		public static PawnFilterPartDef HasChildhood { get; }

		public static PawnFilterPartDef HasPassion { get; }

		public static PawnFilterPartDef HasPassionsAtLevel { get; }

		public static PawnFilterPartDef HasRelation { get; }

		public static PawnFilterPartDef HasSkill { get; }

		public static PawnFilterPartDef HasSkillsAtLevel { get; }

		public static PawnFilterPartDef HasTrait { get; }

		public static PawnFilterPartDef IsAge { get; }

		public static PawnFilterPartDef IsCapableOfEverything { get; }

		public static PawnFilterPartDef IsCapableOf { get; }

		public static PawnFilterPartDef IsGender { get; }

		public static PawnFilterPartDef IsSpecies { get; }

		public static PawnFilterPartDef LogicGate { get; }

		public static PawnFilterPartDef NameMatches { get; }

		static PawnFilterPartDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterPartDefOf));
	}
}
