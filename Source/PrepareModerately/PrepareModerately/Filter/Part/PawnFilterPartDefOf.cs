using RimWorld;

namespace Lakuna.PrepareModerately.Filter.Part {
	[DefOf]
	public static class PawnFilterPartDefOf {
		public static readonly PawnFilterPartDef HasAdulthood;

		public static readonly PawnFilterPartDef HasAnyAddiction;

		public static readonly PawnFilterPartDef HasAnyBodyModification;

		public static readonly PawnFilterPartDef HasAnyPermanentMedicalCondition;

		public static readonly PawnFilterPartDef HasAnyRelation;

		public static readonly PawnFilterPartDef HasAnyScar;

		public static readonly PawnFilterPartDef HasChildhood;

		public static readonly PawnFilterPartDef HasPassion;

		public static readonly PawnFilterPartDef HasPassionsAtLevel;

		public static readonly PawnFilterPartDef HasRelation;

		public static readonly PawnFilterPartDef HasSkill;

		public static readonly PawnFilterPartDef HasSkillsAtLevel;

		public static readonly PawnFilterPartDef HasTrait;

		public static readonly PawnFilterPartDef IsAge;

		public static readonly PawnFilterPartDef IsCapableOf;

		public static readonly PawnFilterPartDef IsCapableOfEverything;

		public static readonly PawnFilterPartDef IsGender;

		public static readonly PawnFilterPartDef IsSpecies;

		public static readonly PawnFilterPartDef LogicGate;

		public static readonly PawnFilterPartDef NameMatches;

#if !V1_0
		public static readonly PawnFilterPartDef HasMeditationFocus;
#endif

#if !(V1_0 || V1_1 || V1_2)
		public static readonly PawnFilterPartDef HasFavoriteColor;
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		public static readonly PawnFilterPartDef HasPossession;
#endif

		static PawnFilterPartDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterPartDefOf));
	}
}
