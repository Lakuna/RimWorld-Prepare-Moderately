using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class IsCapableOf : PawnFilterPart {
		private static IEnumerable<WorkTags> _legalWorkTags {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				WorkTags[] values = Enum.GetValues(typeof(WorkTags));
#else
				WorkTags[] values = Enum.GetValues<WorkTags>();
#endif
				return values.OfType<WorkTags>();
			}
		}

		private WorkTags _workTag;

		public override bool CanCoexistWith(PawnFilterPart other) =>
			other.Def != PawnFilterPartDefOf.IsCapableOfEverything;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
			if (Widgets.ButtonText(rect, this._workTag.ToString().Finish(CapitalizeType.First))) {
				FloatMenuUtility.MakeMenu(
					_legalWorkTags.OrderBy((workTag) => workTag.ToString()),
					(workTag) => workTag.ToString().Finish(CapitalizeType.First),
					(workTag) => () => this._workTag = workTag);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._workTag, "workTag");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ this._workTag.GetHashCode();

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		public override bool IgnoreFor(Pawn pawn) =>
			base.IgnoreFor(pawn)
			|| (pawn.genes?.DisabledWorkTags.HasFlag(this._workTag) ?? false);
#endif

		public override bool Matches(Pawn pawn) =>
#if V1_0
			!(pawn.story?.WorkTagIsDisabled(this._workTag) ?? false);
#else
			!pawn.WorkTagIsDisabled(this._workTag);
#endif

		public override void Randomize() =>
			this._workTag = _legalWorkTags.RandomElement();

		public override string Summary(PawnFilter filter) =>
			"PM.IsCapableOfWorkTag".TranslateFull(this._workTag.ToString());
	}
}
