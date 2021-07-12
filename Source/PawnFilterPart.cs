using System;
using System.Xml.Serialization;
using Verse;

namespace PrepareModerately {
	public abstract class PawnFilterPart : IExposable {
		[Serializable]
		[XmlInclude(typeof(PawnFilterPart_AgeRange.SerializableAgeRange))]
		[XmlInclude(typeof(PawnFilterPart_CapableOf.SerializableCapableOf))]
		[XmlInclude(typeof(PawnFilterPart_CapableOfEverything.SerializableCapableOfEverything))]
		[XmlInclude(typeof(PawnFilterPart_Gender.SerializableGender))]
		[XmlInclude(typeof(PawnFilterPart_HasRelationship.SerializableHasRelationship))]
		[XmlInclude(typeof(PawnFilterPart_IsSpecies.SerializableIsSpecies))]
		[XmlInclude(typeof(PawnFilterPart_LogicGate.SerializableLogicGate))]
		[XmlInclude(typeof(PawnFilterPart_MinimumInterests.SerializableMinimumInterests))]
		[XmlInclude(typeof(PawnFilterPart_NameContains.SerializableNameContains))]
		[XmlInclude(typeof(PawnFilterPart_NoAddictions.SerializableNoAddictions))]
		[XmlInclude(typeof(PawnFilterPart_NoPermanentMedicalConditions.SerializableNoPermanentMedicalConditions))]
		[XmlInclude(typeof(PawnFilterPart_NoRelationships.SerializableNoRelationships))]
		[XmlInclude(typeof(PawnFilterPart_PassionValue.SerializablePassionValue))]
		[XmlInclude(typeof(PawnFilterPart_RequiredTrait.SerializableRequiredTrait))]
		[XmlInclude(typeof(PawnFilterPart_SkillLevelMinimum.SerializableSkillLevelMinimum))]
		[XmlInclude(typeof(PawnFilterPart_WellRounded.SerializableWellRounded))]
		public abstract class SerializablePawnFilterPart {
			public abstract PawnFilterPart Deserialize();
		}

		public string label = "No label";
		public bool toRemove = false;
		public PawnFilterPartDef def;

		public static float RowHeight => Text.LineHeight;

		public abstract bool Matches(Pawn pawn);

		public abstract void DoEditInterface(Listing_PawnFilter list);

		public void ExposeData() => Scribe_Defs.Look(ref this.def, "def");

		public abstract SerializablePawnFilterPart Serialize();
	}
}
