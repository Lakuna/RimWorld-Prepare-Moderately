using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasChildhood : PawnFilterPart {
#if V1_0 || V1_1 || V1_2 || V1_3
		private static IEnumerable<Backstory> _legalBackstories =>
			BackstoryDatabase.allBackstories?.Values?.Where((backstory) => backstory?.slot == BackstorySlot.Childhood) ?? new List<Backstory>();
#elif V1_4 || V1_5 || V1_6
		private static IEnumerable<BackstoryDef> _legalBackstories =>
			DefDatabase<BackstoryDef>.AllDefs?.Where((def) => def.slot == BackstorySlot.Childhood) ?? new List<BackstoryDef>();
#else
		private static IEnumerable<BackstoryDef> _legalBackstories =>
			DefDatabase<BackstoryDef>.AllDefs?.Where((def) => def.slot == BackstorySlot.Childhood) ?? [];
#endif

		public override IEnumerable<string> ConfigErrors {
			get {
				foreach (string item in base.ConfigErrors) {
					yield return item;
				}

				if (this._backstory is null) {
					yield return "Has childhood filter part backstory is null.";
				}
			}
		}

		public override bool HasNullDefs =>
			base.HasNullDefs || this._backstory is null;

#if V1_0 || V1_1 || V1_2 || V1_3
		private Backstory _backstory;
#elif V1_4 || V1_5 || V1_6
		private BackstoryDef _backstory;
#else
		private BackstoryDef? _backstory;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3
		private static string GetCombinedLabelFor(Backstory def) =>
#elif V1_4 || V1_5 || V1_6
		private static string GetCombinedLabelFor(BackstoryDef def) =>
#else
		private static string GetCombinedLabelFor(BackstoryDef? def) =>
#endif
			def is null ? "null"
			: (def.titleFemale is null || def.titleFemale.NullOrEmpty()) ? (def.title ?? "null")
			: $"{def.title ?? "null"}/{def.titleFemale}";

#if V1_0 || V1_1 || V1_2 || V1_3
		private static string GetUniqueCombinedLabelFor(Backstory def) =>
			def is null ? "null"
			: GetCombinedLabelFor(def).NullOrEmpty() ? (def.identifier ?? "null")
			: _legalBackstories.Count((def2) => GetCombinedLabelFor(def) == GetCombinedLabelFor(def2)) > 1 ? $"{GetCombinedLabelFor(def)} ({def.identifier ?? "null"})"
			: GetCombinedLabelFor(def);
#elif V1_4 || V1_5 || V1_6
		private static string GetUniqueCombinedLabelFor(BackstoryDef def) =>
			def is null ? "null"
			: GetCombinedLabelFor(def).NullOrEmpty() ? (def.defName ?? "null")
			: _legalBackstories.Count((def2) => GetCombinedLabelFor(def) == GetCombinedLabelFor(def2)) > 1 ? $"{GetCombinedLabelFor(def)} ({def.defName ?? "null"})"
			: GetCombinedLabelFor(def);
#else
		private static string GetUniqueCombinedLabelFor(BackstoryDef? def) =>
			def is null ? "null"
			: GetCombinedLabelFor(def).NullOrEmpty() ? (def.defName ?? "null")
			: _legalBackstories.Count((def2) => GetCombinedLabelFor(def) == GetCombinedLabelFor(def2)) > 1 ? $"{GetCombinedLabelFor(def)} ({def.defName ?? "null"})"
			: GetCombinedLabelFor(def);
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(rect, GetUniqueCombinedLabelFor(this._backstory).CapitalizeFirstSafe())) {
#else
			if (Widgets.ButtonText(rect, GetUniqueCombinedLabelFor(this._backstory).CapitalizeFirstSafe)) {
#endif
				FloatMenuUtility.MakeMenu(
					_legalBackstories.OrderBy(GetUniqueCombinedLabelFor),
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(def) => GetUniqueCombinedLabelFor(def).CapitalizeFirstSafe(),
#else
					(def) => GetUniqueCombinedLabelFor(def).CapitalizeFirstSafe,
#endif
					(def) => () => this._backstory = def);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
#if V1_0 || V1_1 || V1_2 || V1_3
			Scribe_Values.Look(ref this._backstory, "backstory");
#else
			Scribe_Defs.Look(ref this._backstory, "backstory");
#endif

			// Support loading old filters that use `backstoryIdentifier` instead of `backstory`.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			string backstoryIdentifier = "";
#else
			string? backstoryIdentifier = "";
#endif
			if (Scribe.mode == LoadSaveMode.LoadingVars) {
				Scribe_Values.Look(ref backstoryIdentifier, "backstoryIdentifier");
			}

			if (Scribe.mode == LoadSaveMode.PostLoadInit && this._backstory == default) {
				this._backstory = _legalBackstories.FirstOrDefault((backstory) => backstory.identifier == backstoryIdentifier);
			}
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ (this._backstory?.GetHashCode() ?? 0);

		public override bool Matches(Pawn pawn) =>
#if V1_0 || V1_1 || V1_2 || V1_3
			pawn.story?.childhood == this._backstory;
#else
			pawn.story?.Childhood == this._backstory;
#endif

		public override void Randomize() =>
			this._backstory = _legalBackstories.RandomElement();

		public override string Summary(PawnFilter filter) =>
#if V1_0
			"PM.HasChildhood".Translate(GetUniqueCombinedLabelFor(this._backstory)) ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.HasChildhood".Translate(GetUniqueCombinedLabelFor(this._backstory)).StringSafe();
#else
			"PM.HasChildhood".Translate(GetUniqueCombinedLabelFor(this._backstory)).StringSafe;
#endif
	}
}
