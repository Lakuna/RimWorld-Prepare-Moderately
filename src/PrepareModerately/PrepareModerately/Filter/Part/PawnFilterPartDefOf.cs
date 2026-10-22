using RimWorld;

namespace Lakuna.PrepareModerately.Filter.Part {
	// Based on `RimWorld.ScenPartDefOf`.
	[DefOf]
	public static class PawnFilterPartDefOf {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasAdulthood;
#else
		public static readonly PawnFilterPartDef? HasAdulthood;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasAnyAddiction;
#else
		public static readonly PawnFilterPartDef? HasAnyAddiction;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasAnyBodyModification;
#else
		public static readonly PawnFilterPartDef? HasAnyBodyModification;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasAnyMissingBodyPart;
#else
		public static readonly PawnFilterPartDef? HasAnyMissingBodyPart;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasAnyPermanentMedicalCondition;
#else
		public static readonly PawnFilterPartDef? HasAnyPermanentMedicalCondition;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasAnyRelation;
#else
		public static readonly PawnFilterPartDef? HasAnyRelation;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasAnyScar;
#else
		public static readonly PawnFilterPartDef? HasAnyScar;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasChildhood;
#else
		public static readonly PawnFilterPartDef? HasChildhood;
#endif

#if V1_0 || V1_1 || V1_2
#elif V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasFavoriteColor;
#else
		public static readonly PawnFilterPartDef? HasFavoriteColor;
#endif

#if V1_0
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasMeditationFocus;
#else
		public static readonly PawnFilterPartDef? HasMeditationFocus;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasNumberOfTraits;
#else
		public static readonly PawnFilterPartDef? HasNumberOfTraits;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasPassion;
#else
		public static readonly PawnFilterPartDef? HasPassion;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasPassionsAtLevel;
#else
		public static readonly PawnFilterPartDef? HasPassionsAtLevel;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3
#elif V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasPossession;
#else
		public static readonly PawnFilterPartDef? HasPossession;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasRelation;
#else
		public static readonly PawnFilterPartDef? HasRelation;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasSkill;
#else
		public static readonly PawnFilterPartDef? HasSkill;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasSkillsAtLevel;
#else
		public static readonly PawnFilterPartDef? HasSkillsAtLevel;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasSumOfSkills;
#else
		public static readonly PawnFilterPartDef? HasSumOfSkills;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef HasTrait;
#else
		public static readonly PawnFilterPartDef? HasTrait;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef IsAge;
#else
		public static readonly PawnFilterPartDef? IsAge;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef IsCapableOf;
#else
		public static readonly PawnFilterPartDef? IsCapableOf;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef IsCapableOfEverything;
#else
		public static readonly PawnFilterPartDef? IsCapableOfEverything;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef IsGender;
#else
		public static readonly PawnFilterPartDef? IsGender;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3
#elif V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef IsPregnant;
#else
		public static readonly PawnFilterPartDef? IsPregnant;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef IsKind;
#else
		public static readonly PawnFilterPartDef? IsKind;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef IsSpecies;
#else
		public static readonly PawnFilterPartDef? IsSpecies;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef LogicGate;
#else
		public static readonly PawnFilterPartDef? LogicGate;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterPartDef NameMatches;
#else
		public static readonly PawnFilterPartDef? NameMatches;
#endif

		static PawnFilterPartDefOf() =>
			DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterPartDefOf));
	}
}
