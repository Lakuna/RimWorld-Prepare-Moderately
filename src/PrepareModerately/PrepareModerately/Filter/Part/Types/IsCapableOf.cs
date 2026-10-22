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
		private static IEnumerable<WorkTags> _legalWorkTags =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(WorkTags)).OfType<WorkTags>();
#else
			Enum.GetValues<WorkTags>().OfType<WorkTags>();
#endif

		private WorkTags _workTag;

		public override bool CanCoexistWith(PawnFilterPart other) =>
			other.Def != PawnFilterPartDefOf.IsCapableOfEverything;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(rect, this._workTag.ToString().CapitalizeFirstSafe())) {
#else
			if (Widgets.ButtonText(rect, this._workTag.ToString().CapitalizeFirstSafe)) {
#endif
				FloatMenuUtility.MakeMenu(
					_legalWorkTags.OrderBy((workTag) => workTag.ToString()),
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(workTag) => workTag.ToString().CapitalizeFirstSafe(),
#else
					(workTag) => workTag.ToString().CapitalizeFirstSafe,
#endif
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
#if V1_0
			"PM.IsCapableOfWorkTag".Translate(this._workTag.ToString()) ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.IsCapableOfWorkTag".Translate(this._workTag.ToString()).StringSafe();
#else
			"PM.IsCapableOfWorkTag".Translate(this._workTag.ToString()).StringSafe;
#endif
	}
}
