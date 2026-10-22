using System.Collections.Generic;
using System.Linq;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Utility {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
	public sealed class TraitDegree : IExposable {
#else
	public sealed class TraitDegree(TraitDef? trait = default, int degree = default) : IExposable {
#endif
		public static IEnumerable<TraitDegree> All =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			DefDatabase<TraitDef>.AllDefs?.SelectMany((def) => def.degreeDatas?.Select((data) => new TraitDegree(def, data?.degree ?? default)) ?? new List<TraitDegree>()) ?? new List<TraitDegree>();
#else
			DefDatabase<TraitDef>.AllDefs?.SelectMany((def) => def.degreeDatas?.Select((data) => new TraitDegree(def, data?.degree ?? default)) ?? []) ?? [];
#endif

		public TraitDegreeData Data {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				TraitDegreeData data = this._trait?.DataAtDegree(this._degree);
#else
				TraitDegreeData? data = this._trait?.DataAtDegree(this._degree);
#endif
				if (data is null) {
					PrepareModeratelyLogger.LogErrorMessage($"Unable to get data for trait {this._trait} at degree {this._degree}.");
					return new TraitDegreeData();
				}

				return data;
			}
		}

		public int Degree {
			get => this._degree;
			set => this._degree = value;
		}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public TraitDef Trait {
#else
		public TraitDef? Trait {
#endif
			get => this._trait;
			set => this._trait = value;
		}

		public TraitDegree() : this(default, default) { }

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public TraitDegree(TraitDef trait = default, int degree = default) {
			this._trait = trait;
			this._degree = degree;
		}
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private int _degree;
#else
		private int _degree = degree;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private TraitDef _trait;
#else
		private TraitDef? _trait = trait;
#endif

		public void ExposeData() {
			Scribe_Defs.Look(ref this._trait, "trait");
			Scribe_Values.Look(ref this._degree, "degree");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ this.Degree.GetHashCode() ^ (this.Trait?.GetHashCode() ?? 0);
	}
}
