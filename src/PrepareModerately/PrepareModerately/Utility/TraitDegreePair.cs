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
		internal static IEnumerable<TraitDegreePair> _all =>
			DefDatabase<TraitDef>.AllDefs.SelectMany((def) => def.degreeDatas.Select((data) => new TraitDegreePair(def, data.degree)));

#pragma warning disable CA1051, IDE1006 // `Def` relies on static fields with names matching XML definitions.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		internal int degree;
#else
		internal int degree = degree;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		internal TraitDef trait;
#else
		internal TraitDef trait = def;
#endif
#pragma warning restore CA1051, IDE1006

		internal TraitDegreeData _traitDegreeData =>
			this.trait.DataAtDegree(this.degree);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		// Must be public.
		public TraitDegreePair() {
		}

		internal TraitDegreePair(TraitDef def, int degree) {
			this.trait = def;
			this.degree = degree;
		}
#else
		// Must be public.
		public TraitDegreePair() : this(default, default) { }
#endif

		public void ExposeData() {
			Scribe_Defs.Look(ref this.trait, nameof(this.trait));
			Scribe_Values.Look(ref this.degree, nameof(this.degree));
		}
	}
}
