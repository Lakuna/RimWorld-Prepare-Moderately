using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasTraitDegreeData : IExposable {
		public TraitDef trait;

		public int degree;

		public TraitDegreeData Degree => this.trait.DataAtDegree(this.degree);

		/*
		private static readonly List<HasTraitDegreeData> traitDegreeDatas;

		static HasTraitFilterPart() {
			HasTraitFilterPart.traitDegreeDatas = new List<HasTraitDegreeData>();

			foreach (TraitDef trait in DefDatabase<TraitDef>.AllDefsListForReading) {
				foreach (TraitDegreeData degree in trait.degreeDatas) {
					HasTraitFilterPart.traitDegreeDatas.Add(new HasTraitDegreeData(trait, degree.degree));
				}
			}
		}
		*/

		public static List<HasTraitDegreeData> AllTraitDegreeDatas {
			get {
				List<HasTraitDegreeData> output = new List<HasTraitDegreeData>();
				foreach (TraitDef trait in DefDatabase<TraitDef>.AllDefsListForReading) {
					foreach (TraitDegreeData degree in trait.degreeDatas) {
						output.Add(new HasTraitDegreeData(trait, degree.degree));
					}
				}
				return output;
			}
		}

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
