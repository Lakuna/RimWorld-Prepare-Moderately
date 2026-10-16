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
			DefDatabase<TraitDef>.AllDefs.SelectMany((def) => def.degreeDatas.Select((data) => new TraitDegreePair(def, data.degree)));

		internal int Degree =>
			this._degree;

		internal TraitDef Trait =>
			this._trait;

		internal TraitDegreeData TraitDegreeData =>
			this._trait.DataAtDegree(this._degree);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private int _degree;
#else
		private int _degree = degree;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private TraitDef _trait;
#else
		private TraitDef _trait = def;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		// Must be public.
		public TraitDegreePair() {
		}

		internal TraitDegreePair(TraitDef def, int degree) {
			this._trait = def;
			this._degree = degree;
		}
#else
		// Must be public.
		public TraitDegreePair() : this(default, default) { }
#endif

		public void ExposeData() {
			Scribe_Defs.Look(ref this._trait, MiscUtil.NoUnder(nameof(this._trait)));
			Scribe_Values.Look(ref this._degree, MiscUtil.NoUnder(nameof(this._degree)));
		}
	}
}
