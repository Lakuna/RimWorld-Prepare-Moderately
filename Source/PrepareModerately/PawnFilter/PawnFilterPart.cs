using PrepareModerately.PawnFilter.PawnFilterParts;
using PrepareModerately.UI;
using System.Xml.Serialization;
using Verse;

namespace PrepareModerately.PawnFilter {
	[XmlInclude(typeof(HasAnyAddiction))]
	[XmlInclude(typeof(HasAnyPermanentMedicalCondition))]
	[XmlInclude(typeof(HasAnyRelationship))]
	[XmlInclude(typeof(HasMinimumPassionsAtLevel))]
	[XmlInclude(typeof(HasMinimumSkillsAtLevel))]
	[XmlInclude(typeof(HasPassionValue))]
	[XmlInclude(typeof(HasRelationship))]
	[XmlInclude(typeof(HasSkillLevelMinimum))]
	[XmlInclude(typeof(HasTrait))]
	[XmlInclude(typeof(IsCapableOf))]
	[XmlInclude(typeof(IsCapableOfEverything))]
	[XmlInclude(typeof(IsGender))]
	[XmlInclude(typeof(IsInAgeRange))]
	[XmlInclude(typeof(IsSpecies))]
	[XmlInclude(typeof(LogicGate))]
	[XmlInclude(typeof(NameContains))]
	public abstract class PawnFilterPart : IExposable {
		private PawnFilterPartDef def;

		public PawnFilterPartDef Def {
			get {
				if (this.def == null) {
					this.def = DefDatabase<PawnFilterPartDef>.AllDefsListForReading.Find((def) => def.partClass == this.GetType());
				}
				return this.def;
			}
		}

		[XmlIgnore]
		public PawnFilter Filter => PrepareModerately.Instance.activeFilter;

		public string Label => this.Def.label;

		public abstract bool Matches(Pawn pawn);

		public abstract void DoEditInterface(Listing_PawnFilter listing);

		public void Remove() => this.Filter.parts.Remove(this);

		public void ExposeData() => Scribe_Defs.Look(ref this.def, "def");
	}
}
