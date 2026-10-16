using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasAdulthood : PawnFilterPart {
#pragma warning restore CA1812
#if V1_0 || V1_1 || V1_2 || V1_3
		private static IEnumerable<Backstory> LegalBackstories =>
			BackstoryDatabase.allBackstories.Values.Where((backstory) => backstory.slot == BackstorySlot.Adulthood);
#else
		private static IEnumerable<BackstoryDef> LegalBackstories =>
			DefDatabase<BackstoryDef>.AllDefs.Where((def) => def.slot == BackstorySlot.Adulthood);
#endif

#if V1_0 || V1_1 || V1_2 || V1_3
		private Backstory Backstory =>
			LegalBackstories.First((backstory) => backstory.identifier == this._backstoryIdentifier);
#endif

#if V1_0 || V1_1 || V1_2 || V1_3
		private string _backstoryIdentifier;
#else
		private BackstoryDef _backstory;
#endif

		public override void ExposeData() {
			base.ExposeData();
#if V1_0 || V1_1 || V1_2 || V1_3
			Scribe_Values.Look(ref this._backstoryIdentifier, MiscUtil.NoUnder(nameof(this._backstoryIdentifier)));
#else
			Scribe_Defs.Look(ref this._backstory, MiscUtil.NoUnder(nameof(this._backstory)));
#endif
		}

		public override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
#if V1_0 || V1_1 || V1_2 || V1_3
			: pawn.story.adulthood == this.Backstory;
#else
			: pawn.story.Adulthood == this._backstory;
#endif

		public override string Summary(PawnFilter filter) =>
#if V1_0 || V1_1 || V1_2 || V1_3
			"PM.HasAdulthood".Translate(this.Backstory.title);
#else
			"PM.HasAdulthood".Translate(this._backstory.title);
#endif

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
#if V1_0 || V1_1 || V1_2 || V1_3
			if (Widgets.ButtonText(rect, this.Backstory.title.CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(LegalBackstories.OrderBy((backstory) => backstory.title),
					(backstory) => backstory.title.CapitalizeFirst(),
					(backstory) => () => this._backstoryIdentifier = backstory.identifier);
			}
#else
			if (Widgets.ButtonText(rect, this._backstory.title.CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(LegalBackstories.OrderBy((def) => def.title),
					(def) => def.title.CapitalizeFirst(),
					(def) => () => this._backstory = def);
			}
#endif
		}

		internal override void Randomize() =>
#if V1_0 || V1_1 || V1_2 || V1_3
			this._backstoryIdentifier = LegalBackstories.RandomElement().identifier;
#else
			this._backstory = LegalBackstories.RandomElement();
#endif
	}
}
