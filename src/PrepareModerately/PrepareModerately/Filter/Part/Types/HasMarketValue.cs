using Lakuna.PrepareModerately.Ui;
#if !V1_0
using Lakuna.PrepareModerately.Utility;
#endif

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasMarketValue : PawnFilterPart {
		private const float _maxMarketValue = 10000;

		private const float _minMarketValue = 0;

		private FloatRange _range;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect headerRemainderRect).Rounded();
			Widgets.FloatRange(headerRemainderRect, Rand.Int, ref this._range, _minMarketValue, _maxMarketValue);
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._range, "range");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ this._range.GetHashCode();

		public override bool Matches(Pawn pawn) =>
			pawn.MarketValue <= this._range.max && pawn.MarketValue >= this._range.min;

		public override void Randomize() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._range = new FloatRange(Rand.Range(0, 3000), Rand.Range(1000, 10000));
#else
			this._range = new(Rand.Range(0, 3000), Rand.Range(3000, 10000));
#endif
		}

		public override string Summary(PawnFilter filter) =>
			(this._range.min == this._range.max ? "PM.HasMarketValue".Translate(this._range.min)
					: "PM.HasBetweenMarketValue".Translate(this._range.min, this._range.max))
#if V1_0
				?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				.StringSafe();
#else
				.StringSafe;
#endif
	}
}
