using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.UI;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsSpecies : PawnFilterPart {
		private static IEnumerable<ThingDef> LegalThings => DefDatabase<ThingDef>.AllDefs.Where((def) => def.race != null && !def.IsCorpse);

#if V1_0
		private static string GetUniqueLabelFor(ThingDef def) =>
			def.LabelCap.NullOrEmpty() ? def.defName
			: LegalThings.Count((def2) => def.LabelCap == def2.LabelCap) > 1 ? $"{def.LabelCap} ({def.defName})"
			: def.LabelCap;
#else
		private static TaggedString GetUniqueLabelFor(ThingDef def) =>
			def.LabelCap.NullOrEmpty() ? new TaggedString(def.defName)
			: LegalThings.Count((def2) => def.LabelCap == def2.LabelCap) > 1 ? new TaggedString($"{def.LabelCap} ({def.defName})")
			: def.LabelCap;
#endif

		private ThingDef species;

		public override bool Matches(Pawn pawn) => pawn is null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.def == this.species;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, GetUniqueLabelFor(this.species))) {
				FloatMenuUtility.MakeMenu(LegalThings.OrderBy((def) => def.label),
					(def) => GetUniqueLabelFor(def),
					(def) => () => this.species = def);
			}
		}

		public override string Summary(PawnFilter filter) => "PM.IsSpecies".Translate(this.species.label);

		public override void Randomize() => this.species = ThingDefOf.Human; // Other things wouldn't be legal for vanilla RimWorld.

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.species, nameof(this.species));
		}
	}
}
