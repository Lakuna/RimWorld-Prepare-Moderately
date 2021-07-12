using PrepareModerately.Filter.Filters;
using PrepareModerately.GUI;
using System;
using System.Xml.Serialization;
using Verse;

namespace PrepareModerately.Filter {
	public abstract class PawnFilterPart : IExposable {
		[Serializable]
		[XmlInclude(typeof(HasAnyAddiction.HasAnyAddictionSerializable))]
		[XmlInclude(typeof(HasAnyPermanentMedicalCondition.HasAnyPermanentMedicalConditionSerializable))]
		[XmlInclude(typeof(HasAnyRelationship.HasAnyRelationshipSerializable))]
		[XmlInclude(typeof(HasMinimumInterestsAtLevel.HasMinimumInterestsAtLevelSerializable))]
		[XmlInclude(typeof(HasMinimumSkillsAtLevel.HasMinimumSkillsAtLevelSerializable))]
		[XmlInclude(typeof(HasPassionValue.HasPassionValueSerializable))]
		[XmlInclude(typeof(HasRelationship.HasRelationshipSerializable))]
		[XmlInclude(typeof(HasSkillLevelMinimum.HasSkillLevelMinimumSerializable))]
		[XmlInclude(typeof(HasTrait.HasTraitSerializable))]
		[XmlInclude(typeof(InAgeRange.InAgeRangeSerializable))]
		[XmlInclude(typeof(IsCapableOf.IsCapableOfSerializable))]
		[XmlInclude(typeof(IsCapableOfEverything.IsCapableOfEverythingSerializable))]
		[XmlInclude(typeof(IsGender.IsGenderSerializable))]
		[XmlInclude(typeof(IsSpecies.IsSpeciesSerializable))]
		[XmlInclude(typeof(LogicGate.LogicGateSerializable))]
		[XmlInclude(typeof(NameContains.NameContainsSerializable))]
		public abstract class PawnFilterPartSerializable {
			public abstract PawnFilterPart Deserialize();
		}

		public string label = "No label";
		public bool toRemove = false;
		public PawnFilterPartDef def;

		public static float RowHeight => Text.LineHeight;

		public abstract bool Matches(Pawn pawn);

		public abstract void DoEditInterface(PawnFilterListing list);

		public void ExposeData() => Scribe_Defs.Look(ref this.def, "def");

		public abstract PawnFilterPartSerializable Serialize();
	}
}
