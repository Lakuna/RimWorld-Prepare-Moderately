using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class IsKind : PawnFilterPart {
#pragma warning restore CA1812
		private static IEnumerable<PawnKindDef> LegalKinds =>
			DefDatabase<PawnKindDef>.AllDefs;

#pragma warning disable IDE1006 // `Def` relies on static fields with names matching XML definitions.
		private PawnKindDef kind;
#pragma warning restore IDE1006

#if V1_0
		private static string GetUniqueLabelFor(PawnKindDef def) =>
			def.LabelCap.NullOrEmpty() ? def.defName
			: LegalKinds.Count((def2) => def.LabelCap == def2.LabelCap) > 1 ? $"{def.LabelCap} ({def.defName})"
			: def.LabelCap;
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static TaggedString GetUniqueLabelFor(PawnKindDef def) =>
			def is null ? throw new ArgumentNullException(nameof(def))
			: def.LabelCap.NullOrEmpty() ? new TaggedString(def.defName)
			: LegalKinds.Count((def2) => def.LabelCap == def2.LabelCap) > 1 ? new TaggedString($"{def.LabelCap} ({def.defName})")
			: def.LabelCap;
#else
		private static TaggedString GetUniqueLabelFor(PawnKindDef def) =>
			def is null ? throw new ArgumentNullException(nameof(def))
			: def.LabelCap.NullOrEmpty() ? def.defName
			: LegalKinds.Count((def2) => def.LabelCap == def2.LabelCap) > 1 ? $"{def.LabelCap} ({def.defName})"
			: def.LabelCap;
#endif

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.kind, nameof(this.kind));
		}

		public override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
			: pawn.kindDef == this.kind;

		public override string Summary(PawnFilter filter) =>
			"PM.IsKind".Translate(this.kind.label);

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, GetUniqueLabelFor(this.kind))) {
				FloatMenuUtility.MakeMenu(LegalKinds.OrderBy((def) => def.label),
					(def) => GetUniqueLabelFor(def),
					(def) => () => this.kind = def);
			}
		}

		internal override void Randomize() =>
#if V1_0 || V1_1 || V1_2 || V1_3
			this.kind = Faction.OfPlayerSilentFail?.def.basicMemberKind ?? PawnKindDefOf.Colonist; // Other kinds wouldn't be legal for vanilla RimWorld.
#else
			this.kind = Find.GameInitData?.startingPawnKind ?? Faction.OfPlayerSilentFail?.def.basicMemberKind ?? PawnKindDefOf.Colonist; // Other kinds wouldn't be legal for vanilla RimWorld.
#endif
	}
}
