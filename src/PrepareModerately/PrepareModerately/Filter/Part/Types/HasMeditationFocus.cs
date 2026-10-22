#if !V1_0
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasMeditationFocus : PawnFilterPart {
		private static IEnumerable<MeditationFocusDef> _allMeditationFoci =>
#if V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			DefDatabase<MeditationFocusDef>.AllDefs ?? new List<MeditationFocusDef>();
#else
			DefDatabase<MeditationFocusDef>.AllDefs ?? [];
#endif

		public override IEnumerable<string> ConfigErrors {
			get {
				foreach (string item in base.ConfigErrors) {
					yield return item;
				}

				if (this._meditationFocus is null) {
					yield return "Has meditation focus filter part meditation focus is null.";
				}
			}
		}

		public override bool HasNullDefs =>
			base.HasNullDefs || this._meditationFocus is null;

#if V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private MeditationFocusDef _meditationFocus;
#else
		private MeditationFocusDef? _meditationFocus;
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
#if V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(rect, this._meditationFocus?.LabelCap.StringSafe() ?? "null")) {
#else
			if (Widgets.ButtonText(rect, this._meditationFocus?.LabelCap.StringSafe ?? "null")) {
#endif
				FloatMenuUtility.MakeMenu(
					_allMeditationFoci.OrderBy((def) => def.label),
#if V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(def) => def.LabelCap.StringSafe(),
#else
					(def) => def.LabelCap.StringSafe,
#endif
					(def) => () => this._meditationFocus = def);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this._meditationFocus, "meditationFocus");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ (this._meditationFocus?.GetHashCode() ?? 0);

		public override bool Matches(Pawn pawn) =>
			this._meditationFocus?.CanPawnUse(pawn) ?? false;

		public override void Randomize() =>
			this._meditationFocus = _allMeditationFoci.RandomElement();

		public override string Summary(PawnFilter filter) =>
#if V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.HasMeditationFocus".Translate(this._meditationFocus?.label ?? "null").StringSafe();
#else
			"PM.HasMeditationFocus".Translate(this._meditationFocus?.label ?? "null").StringSafe;
#endif
	}
}
#endif
