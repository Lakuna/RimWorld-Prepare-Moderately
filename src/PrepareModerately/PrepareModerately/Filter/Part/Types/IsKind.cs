using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.UI;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsKind : PawnFilterPart {
		private static IEnumerable<PawnKindDef> LegalKinds => DefDatabase<PawnKindDef>.AllDefs;

#if V1_0
		private static string GetUniqueLabelFor(PawnKindDef def) =>
			def.LabelCap.NullOrEmpty() ? def.defName
			: LegalKinds.Count((def2) => def.LabelCap == def2.LabelCap) > 1 ? $"{def.LabelCap} ({def.defName})"
			: def.LabelCap;
#else
		private static TaggedString GetUniqueLabelFor(PawnKindDef def) =>
			def.LabelCap.NullOrEmpty() ? new TaggedString(def.defName)
			: LegalKinds.Count((def2) => def.LabelCap == def2.LabelCap) > 1 ? new TaggedString($"{def.LabelCap} ({def.defName})")
			: def.LabelCap;
#endif

		private PawnKindDef kind;

		public override bool Matches(Pawn pawn) => pawn is null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.kindDef == this.kind;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, GetUniqueLabelFor(this.kind))) {
				FloatMenuUtility.MakeMenu(LegalKinds.OrderBy((def) => def.label),
					(def) => GetUniqueLabelFor(def),
					(def) => () => this.kind = def);
			}
		}

		public override string Summary(PawnFilter filter) => "PM.IsKind".Translate(this.kind.label);

		public override void Randomize() => this.kind =
#if !(V1_0 || V1_1 || V1_2 || V1_3)
		Find.GameInitData?.startingPawnKind ??
#endif
		Faction.OfPlayerSilentFail?.def.basicMemberKind ?? PawnKindDefOf.Colonist; // Other kinds wouldn't be legal for vanilla RimWorld.

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.kind, nameof(this.kind));
		}
	}
}
