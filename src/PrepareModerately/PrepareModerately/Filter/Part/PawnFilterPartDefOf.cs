using RimWorld;

namespace Lakuna.PrepareModerately.Filter.Part {
	// Based on `RimWorld.ScenPartDefOf`.
	[DefOf]
	internal static class PawnFilterPartDefOf {
#pragma warning disable CS0649, IDE1006 // `DefOf` relies on static fields with names matching XML definitions.
		internal static readonly PawnFilterPartDef HasAdulthood;

		internal static readonly PawnFilterPartDef HasAnyAddiction;

		internal static readonly PawnFilterPartDef HasAnyBodyModification;

		internal static readonly PawnFilterPartDef HasAnyMissingBodyPart;

		internal static readonly PawnFilterPartDef HasAnyPermanentMedicalCondition;

		internal static readonly PawnFilterPartDef HasAnyRelation;

		internal static readonly PawnFilterPartDef HasAnyScar;

		internal static readonly PawnFilterPartDef HasChildhood;

#if !(V1_0 || V1_1 || V1_2)
		internal static readonly PawnFilterPartDef HasFavoriteColor;
#endif

#if !V1_0
		internal static readonly PawnFilterPartDef HasMeditationFocus;
#endif

		internal static readonly PawnFilterPartDef HasNumberOfTraits;

		internal static readonly PawnFilterPartDef HasPassion;

		internal static readonly PawnFilterPartDef HasPassionsAtLevel;

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		internal static readonly PawnFilterPartDef HasPossession;
#endif

		internal static readonly PawnFilterPartDef HasRelation;

		internal static readonly PawnFilterPartDef HasSkill;

		internal static readonly PawnFilterPartDef HasSkillsAtLevel;

		internal static readonly PawnFilterPartDef HasSumOfSkills;

		internal static readonly PawnFilterPartDef HasTrait;

		internal static readonly PawnFilterPartDef IsAge;

		internal static readonly PawnFilterPartDef IsCapableOf;

		internal static readonly PawnFilterPartDef IsCapableOfEverything;

		internal static readonly PawnFilterPartDef IsGender;

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		internal static readonly PawnFilterPartDef IsPregnant;
#endif

		internal static readonly PawnFilterPartDef IsKind;

		internal static readonly PawnFilterPartDef IsSpecies;

		internal static readonly PawnFilterPartDef LogicGate;

		internal static readonly PawnFilterPartDef NameMatches;
#pragma warning restore CS0649, IDE1006

		static PawnFilterPartDefOf() =>
			DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterPartDefOf));
	}
}
