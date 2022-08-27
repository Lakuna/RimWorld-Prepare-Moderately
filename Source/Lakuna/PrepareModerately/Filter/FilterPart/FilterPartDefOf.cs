using RimWorld;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	[DefOf]
	public static class FilterPartDefOf {
		public static FilterPartDef HasAnyAddiction;

		public static FilterPartDef HasAnyBodyModification;

		public static FilterPartDef HasAnyPermanentMedicalCondition;

		public static FilterPartDef HasAnyRelation;

		public static FilterPartDef HasPassion;

		public static FilterPartDef HasPassionsAtLevel;

		public static FilterPartDef HasRelation;

		public static FilterPartDef HasSkill;

		public static FilterPartDef HasSkillsAtLevel;

		public static FilterPartDef HasTrait;

		public static FilterPartDef IsAge;

		public static FilterPartDef IsCapableOfEverything;

		public static FilterPartDef IsCapableOf;

		public static FilterPartDef IsGender;

		public static FilterPartDef IsSpecies;

		public static FilterPartDef LogicGate;

		public static FilterPartDef NameMatches;

		static FilterPartDefOf() {
			DefOfHelper.EnsureInitializedInCtor(typeof(FilterPartDefOf));
		}
	}
}
