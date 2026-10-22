using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasTrait : PawnFilterPart {
		private static IEnumerable<TraitDegree> _legalTraitDegrees =>
			TraitDegree.All;

		public override IEnumerable<string> ConfigErrors {
			get {
				foreach (string item in base.ConfigErrors) {
					yield return item;
				}

				if (this._traitDegree is null) {
					yield return "Has trait filter part trait degree is null.";
				}
			}
		}

		public override bool Valid =>
			base.Valid
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			&& this._traitDegree != null;
#else
			&& this._traitDegree is not null;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private TraitDegree _traitDegree;
#else
		private TraitDegree? _traitDegree;
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();

			if (this._traitDegree is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw edit interface of has trait filter part with null trait degree.");
				return;
			}

#if V1_0
			if (Widgets.ButtonText(rect, this._traitDegree.Data.label?.CapitalizeFirstSafe() ?? "null")) {
#else
			if (Widgets.ButtonText(rect, this._traitDegree.Data.LabelCap ?? "null")) {
#endif
				FloatMenuUtility.MakeMenu(
					_legalTraitDegrees.OrderBy((pair) => pair.Data.label),
#if V1_0
					(traitDegreePair) => traitDegreePair.Data.label?.CapitalizeFirstSafe() ?? "null",
#else
					(traitDegreePair) => traitDegreePair.Data.LabelCap ?? "null",
#endif
					(traitDegreePair) => () => this._traitDegree = traitDegreePair);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Deep.Look(ref this._traitDegree, "traitDegreePair");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ (this._traitDegree?.GetHashCode() ?? 0);

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		public override bool IgnoreFor(Pawn pawn) =>
			base.IgnoreFor(pawn)
			|| this._traitDegree?.Trait is null
			|| (pawn.genes?.GenesListForReading?.Any((gene) =>
				(gene?.def?.suppressedTraits?.Any((traitData) => traitData?.def == this._traitDegree.Trait && traitData?.degree == this._traitDegree.Degree) ?? false)
				|| (gene?.def?.forcedTraits?.Any((traitData) => traitData?.def == this._traitDegree.Trait && traitData?.degree == this._traitDegree.Degree) ?? false)) ?? false);
#endif

		public override bool Matches(Pawn pawn) =>
			this._traitDegree?.Trait is null
			|| pawn.story?.traits?.allTraits?.Find((trait) => trait?.def == this._traitDegree.Trait)?.Degree == this._traitDegree.Degree;

		public override void Randomize() =>
			this._traitDegree = _legalTraitDegrees.RandomElement();

		public override string Summary(PawnFilter filter) =>
#if V1_0
			"PM.HasTrait".Translate(this._traitDegree?.Data.label ?? "null") ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.HasTrait".Translate(this._traitDegree?.Data.label ?? "null").StringSafe();
#else
			"PM.HasTrait".Translate(this._traitDegree?.Data.label ?? "null").StringSafe;
#endif
	}
}
