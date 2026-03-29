using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Lakuna.PrepareModerately.Utility {
	public class TraitDegreePair : IExposable {
		private TraitDef trait;

		public TraitDef Trait => this.trait;

		private int degree;

		public int Degree => this.degree;

		public TraitDegreeData TraitDegreeData => this.trait.DataAtDegree(this.degree);

		public static IEnumerable<TraitDegreePair> TraitDegreePairs {
			get {
				foreach (TraitDef def in DefDatabase<TraitDef>.AllDefsListForReading) {
					foreach (TraitDegreeData degree in def.degreeDatas) {
						yield return new TraitDegreePair(def, degree.degree);
					}
				}
			}
		}

		public TraitDegreePair() { } // Constructor without arguments for loading.

		public TraitDegreePair(TraitDef def, int degree) {
			this.trait = def;
			this.degree = degree;
		}

		public void ExposeData() {
			Scribe_Defs.Look(ref this.trait, nameof(this.trait));
			Scribe_Values.Look(ref this.degree, nameof(this.degree));
		}
	}
}
