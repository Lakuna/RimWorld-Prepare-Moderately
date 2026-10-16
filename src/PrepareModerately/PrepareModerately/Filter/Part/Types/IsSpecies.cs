using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class IsSpecies : PawnFilterPart {
#pragma warning restore CA1812
		private static IEnumerable<ThingDef> LegalThings =>
			DefDatabase<ThingDef>.AllDefs.Where((def) => def.race != null && !def.IsCorpse);

#pragma warning disable IDE1006 // `Def` relies on static fields with names matching XML definitions.
		private ThingDef _species;
#pragma warning restore IDE1006

#if V1_0
		private static string GetUniqueLabelFor(ThingDef def) =>
			def.LabelCap.NullOrEmpty() ? def.defName
			: LegalThings.Count((def2) => def.LabelCap == def2.LabelCap) > 1 ? $"{def.LabelCap} ({def.defName})"
			: def.LabelCap;
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static TaggedString GetUniqueLabelFor(ThingDef def) =>
			def is null ? throw new ArgumentNullException(nameof(def))
			: def.LabelCap.NullOrEmpty() ? new TaggedString(def.defName)
			: LegalThings.Count((def2) => def.LabelCap == def2.LabelCap) > 1 ? new TaggedString($"{def.LabelCap} ({def.defName})")
			: def.LabelCap;
#else
		private static TaggedString GetUniqueLabelFor(ThingDef def) =>
			def is null ? throw new ArgumentNullException(nameof(def))
			: def.LabelCap.NullOrEmpty() ? def.defName
			: LegalThings.Count((def2) => def.LabelCap == def2.LabelCap) > 1 ? $"{def.LabelCap} ({def.defName})"
			: def.LabelCap;
#endif

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this._species, nameof(this._species));
		}

		public override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
			: pawn.def == this._species;

		public override string Summary(PawnFilter filter) =>
			"PM.IsSpecies".Translate(this._species.label);

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, GetUniqueLabelFor(this._species))) {
				FloatMenuUtility.MakeMenu(LegalThings.OrderBy((def) => def.label),
					(def) => GetUniqueLabelFor(def),
					(def) => () => this._species = def);
			}
		}

		internal override void Randomize() =>
			this._species = ThingDefOf.Human; // Other things wouldn't be legal for vanilla RimWorld.
	}
}
