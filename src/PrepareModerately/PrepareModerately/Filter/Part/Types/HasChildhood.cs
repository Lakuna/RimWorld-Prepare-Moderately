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
		private static IEnumerable<Backstory> _legalBackstories
#else
		private static IEnumerable<BackstoryDef> _legalBackstories
#endif
		{
			get {
#if V1_0 || V1_1 || V1_2 || V1_3
				List<Backstory> empty = new List<Backstory>();
#elif V1_4 || V1_5 || V1_6
				List<BackstoryDef> empty = new List<BackstoryDef>();
#else
				List<BackstoryDef> empty = [];
#endif
#if V1_0 || V1_1 || V1_2 || V1_3
				return BackstoryDatabase.allBackstories?.Values?.Where((backstory) => backstory?.slot == BackstorySlot.Childhood) ?? empty;
#else
				return DefDatabase<BackstoryDef>.AllDefs?.Where((def) => def.slot == BackstorySlot.Childhood) ?? empty;
#endif
			}
		}

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

		private static string GetCombinedLabelFor(
#if V1_0 || V1_1 || V1_2 || V1_3
			Backstory def
#elif V1_4 || V1_5 || V1_6
			BackstoryDef def
#else
			BackstoryDef? def
#endif
		) =>
			def is null ? "null"
			: (def.titleFemale is null || def.titleFemale.NullOrEmpty()) ? (def.title ?? "null")
			: $"{def.title ?? "null"}/{def.titleFemale}";

		private static string GetUniqueCombinedLabelFor(
#if V1_0 || V1_1 || V1_2 || V1_3
			Backstory def
#elif V1_4 || V1_5 || V1_6
			BackstoryDef def
#else
			BackstoryDef? def
#endif
		) {
			if (def is null) {
				return "null";
			}

#if V1_0 || V1_1 || V1_2 || V1_3
			string defName = def.identifier ?? "null";
#else
			string defName = def.defName ?? "null";
#endif
			string combinedLabel = GetCombinedLabelFor(def);
			return combinedLabel.NullOrEmpty() ? defName
				: _legalBackstories.Count((def2) => combinedLabel == GetCombinedLabelFor(def2)) > 1 ? $"{combinedLabel} ({defName})"
				: combinedLabel;
		}

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
			if (Widgets.ButtonText(rect, GetUniqueCombinedLabelFor(this._backstory).Finish(CapitalizeType.First))) {
				FloatMenuUtility.MakeMenu(
					_legalBackstories.OrderBy(GetUniqueCombinedLabelFor),
					(def) => GetUniqueCombinedLabelFor(def).Finish(CapitalizeType.First),
					(def) => () => this._backstory = def);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			string backstoryName = "backstory";
#if V1_0 || V1_1 || V1_2 || V1_3
			Scribe_Values.Look(ref this._backstory, backstoryName);
#else
			Scribe_Defs.Look(ref this._backstory, backstoryName);
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
			"PM.HasChildhood".TranslateFull(GetUniqueCombinedLabelFor(this._backstory));
	}
}
