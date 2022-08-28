using RimWorld;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasTraitDegreeData : IExposable {
		public TraitDef trait;

		public int degree;

		public TraitDegreeData Degree => this.trait.DataAtDegree(this.degree);

		// Constructor without arguments for loading.
		public HasTraitDegreeData() { }

		public HasTraitDegreeData(TraitDef trait, int degree) {
			this.trait = trait;
			this.degree = degree;
		}

		public void ExposeData() {
			Scribe_Defs.Look(ref this.trait, nameof(this.trait));
			Scribe_Values.Look(ref this.degree, nameof(this.degree));
		}
	}
}
