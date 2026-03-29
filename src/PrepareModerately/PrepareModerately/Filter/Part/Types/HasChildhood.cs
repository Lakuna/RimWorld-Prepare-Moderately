using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.UI;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasChildhood : PawnFilterPart {
#if V1_0 || V1_1 || V1_2 || V1_3
		private static IEnumerable<Backstory> LegalBackstories => BackstoryDatabase.allBackstories.Values.Where((backstory) => backstory.slot == BackstorySlot.Childhood);
#else
		private static IEnumerable<BackstoryDef> LegalBackstories => DefDatabase<BackstoryDef>.AllDefs.Where((def) => def.slot == BackstorySlot.Childhood);
#endif

#if V1_0 || V1_1 || V1_2 || V1_3
		private string backstoryIdentifier;

		public Backstory Backstory => LegalBackstories.First((backstory) => backstory.identifier == this.backstoryIdentifier);
#else
		private BackstoryDef backstory;
#endif

		public override bool Matches(Pawn pawn) => pawn is null
			? throw new ArgumentNullException(nameof(pawn))
#if V1_0 || V1_1 || V1_2 || V1_3
			: pawn.story.childhood == this.Backstory;
#else
			: pawn.story.Childhood == this.backstory;
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
#if V1_0 || V1_1 || V1_2 || V1_3
			if (Widgets.ButtonText(rect, this.Backstory.title.CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(LegalBackstories.OrderBy((backstory) => backstory.title),
					(backstory) => backstory.title.CapitalizeFirst(),
					(backstory) => () => this.backstoryIdentifier = backstory.identifier);
			}
#else
			if (Widgets.ButtonText(rect, this.backstory.title.CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(LegalBackstories.OrderBy((def) => def.title),
					(def) => def.title.CapitalizeFirst(),
					(def) => () => this.backstory = def);
			}
#endif
		}

		public override string Summary(PawnFilter filter) =>
#if V1_0 || V1_1 || V1_2 || V1_3
			"PM.HasChildhood".Translate(this.Backstory.title);
#else
			"PM.HasChildhood".Translate(this.backstory.title);
#endif

		public override void Randomize() =>
#if V1_0 || V1_1 || V1_2 || V1_3
			this.backstoryIdentifier = LegalBackstories.RandomElement().identifier;
#else
			this.backstory = LegalBackstories.RandomElement();
#endif

		public override void ExposeData() {
			base.ExposeData();
#if V1_0 || V1_1 || V1_2 || V1_3
			Scribe_Values.Look(ref this.backstoryIdentifier, nameof(this.backstoryIdentifier));
#else
			Scribe_Defs.Look(ref this.backstory, nameof(this.backstory));
#endif
		}
	}
}
