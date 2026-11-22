using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

#if V1_0 || V1_1
using RimWorld;
#endif

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasPsychicSensitivity : PawnFilterPart {
		private const float _maxPsychicSensitivity = 10;

		private const float _minPsychicSensitivity = 0;

		private FloatRange _range;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect headerRemainderRect).Rounded();
			Widgets.FloatRange(headerRemainderRect, Rand.Int, ref this._range, _minPsychicSensitivity, _maxPsychicSensitivity);
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._range, "range");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ this._range.GetHashCode();

		public override bool Matches(Pawn pawn) {
#if V1_0 || V1_1
			float psychicSensitivity = pawn.GetStatValue(StatDefOf.PsychicSensitivity) ?? 0;
#else
			float psychicSensitivity = pawn.psychicEntropy?.PsychicSensitivity ?? 0;
#endif
			return psychicSensitivity <= this._range.max && psychicSensitivity >= this._range.min;
		}

		public override void Randomize() {
			int min = Rand.Range(0, 1);
			int max = Rand.Range(1, 10);
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._range = new FloatRange(min, max);
#else
			this._range = new(min, max);
#endif
		}

		public override string Summary(PawnFilter filter) =>
			(this._range.min == this._range.max) ? "PM.HasPsychicSensitivity".TranslateFull(this._range.min)
			: "PM.HasBetweenPsychicSensitivity".TranslateFull(this._range.min, this._range.max);
	}
}
