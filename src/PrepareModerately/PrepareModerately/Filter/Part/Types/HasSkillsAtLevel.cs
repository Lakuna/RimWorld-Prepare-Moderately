using System.Linq;

using Lakuna.PrepareModerately.Ui;
#if !V1_0
using Lakuna.PrepareModerately.Utility;
#endif

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasSkillsAtLevel : PawnFilterPart {
		private IntRange _countRange;

		private IntRange _levelRange;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2, out totalAddedListHeight).Rounded();

			Rect countRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight).Rounded();
			Widgets.IntRange(countRect, Rand.Int, ref this._countRange, 0, 12, "PM.RangeSkills");

			Widgets.IntRange(new Rect(rect.x, countRect.yMax, rect.width, Text.LineHeight).Rounded(), Rand.Int, ref this._levelRange, 0, 20, "PM.LevelRange");
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._levelRange, "range");
			Scribe_Values.Look(ref this._countRange, "countRange");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ this._countRange.GetHashCode() ^ this._levelRange.GetHashCode();

		public override bool Matches(Pawn pawn) {
			int count = pawn.skills?.skills?.AsEnumerable().Count((skill) => (skill?.Level ?? 0) <= this._levelRange.max && (skill?.Level ?? 0) >= this._levelRange.min) ?? 0;
			return count <= this._countRange.max && count >= this._countRange.min;
		}

		public override void Randomize() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._countRange = new IntRange(Rand.Range(0, 1), Rand.Range(1, 3));
			this._levelRange = new IntRange(Rand.Range(0, 5), Rand.Range(5, 10));
#else
			this._countRange = new(Rand.Range(0, 1), Rand.Range(1, 3));
			this._levelRange = new(Rand.Range(0, 5), Rand.Range(5, 10));
#endif
		}

		public override string Summary(PawnFilter filter) =>
			(this._countRange.min == this._countRange.max ?
					this._levelRange.min == this._levelRange.max ? "PM.HasSkillsAtLevel".Translate(this._countRange.min, this._levelRange.min)
					: "PM.HasSkillsBetweenLevels".Translate(this._countRange.min, this._levelRange.min, this._levelRange.max)
				: this._levelRange.min == this._levelRange.max ? "PM.HasBetweenSkillsAtLevel".Translate(this._countRange.min, this._countRange.max, this._levelRange.min)
				: "PM.HasBetweenSkillsBetweenLevels".Translate(this._countRange.min, this._countRange.max, this._levelRange.min, this._levelRange.max))
#if V1_0
				?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				.StringSafe();
#else
				.StringSafe;
#endif
	}
}
