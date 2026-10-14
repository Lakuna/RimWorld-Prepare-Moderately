using System.Collections.Generic;
using System.Linq;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Utility {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
	internal sealed class TraitDegreePair : IExposable {
#else
	internal sealed class TraitDegreePair(TraitDef def, int degree) : IExposable {
#endif
		internal static IEnumerable<TraitDegreePair> TraitDegreePairs =>
			DefDatabase<TraitDef>.AllDefs.SelectMany((def) => def.degreeDatas.Select((degree) => new TraitDegreePair(def, degree.degree)));

		internal int PDegree =>
			this.degree;

		internal TraitDef PTrait =>
			this.trait;

		internal TraitDegreeData TraitDegreeData =>
			this.trait.DataAtDegree(this.degree);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private int degree;
#else
		private int degree = degree;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private TraitDef trait;
#else
		private TraitDef trait = def;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		// Must be public.
		public TraitDegreePair() {
		}

		internal TraitDegreePair(TraitDef def, int degree) {
			this.trait = def;
			this.degree = degree;
		}
#endif

		public void ExposeData() {
			Scribe_Defs.Look(ref this.trait, nameof(this.trait));
			Scribe_Values.Look(ref this.degree, nameof(this.degree));
		}
	}
}
