using System.Collections.Generic;
using System.Linq;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Utility {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
	internal sealed class TraitDegree : IExposable {
#else
	internal sealed class TraitDegree(TraitDef def, int degree) : IExposable {
#endif
		internal static IEnumerable<TraitDegree> _all =>
			DefDatabase<TraitDef>.AllDefs.SelectMany((def) => def.degreeDatas.Select((data) => new TraitDegree(def, data.degree)));

		internal TraitDegreeData _data =>
			this.trait.DataAtDegree(this.degree);

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

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		// Must be public.
		public TraitDegree() {
		}

		internal TraitDegree(TraitDef def, int degree) {
			this.trait = def;
			this.degree = degree;
		}
#else
		// Must be public.
		public TraitDegree() : this(default, default) { }
#endif

		public void ExposeData() {
			Scribe_Defs.Look(ref this.trait, nameof(this.trait));
			Scribe_Values.Look(ref this.degree, nameof(this.degree));
		}
	}
}
