﻿using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasAdulthood : PawnFilterPart {
#if V1_0 || V1_1 || V1_2 || V1_3
		private string backstoryIdentifier;

		public Backstory Backstory => BackstoryDatabase.allBackstories.Values
			.Where((backstory) => backstory.slot == BackstorySlot.Adulthood)
			.First((backstory) => backstory.identifier == this.backstoryIdentifier);
#else
		private BackstoryDef backstory;
#endif

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
#if V1_0 || V1_1 || V1_2 || V1_3
			: pawn.story.adulthood == this.Backstory;
#else
			: pawn.story.Adulthood == this.backstory;
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
#if V1_0 || V1_1 || V1_2 || V1_3
			if (Widgets.ButtonText(rect, this.Backstory.title.CapitalizeFirst())) {
				IOrderedEnumerable<Backstory> backstories = BackstoryDatabase.allBackstories.Values
					.Where((backstory) => backstory.slot == BackstorySlot.Adulthood)
					.OrderBy((backstory) => backstory.title);
				FloatMenuUtility.MakeMenu(backstories,
					(backstory) => backstory.title.CapitalizeFirst(),
					(backstory) => () => this.backstoryIdentifier = backstory.identifier);
			}
#else
			if (Widgets.ButtonText(rect, this.backstory.title.CapitalizeFirst())) {
				IOrderedEnumerable<BackstoryDef> backstories = DefDatabase<BackstoryDef>.AllDefsListForReading
					.Where((def) => def.slot == BackstorySlot.Adulthood)
					.OrderBy((def) => def.title);
				FloatMenuUtility.MakeMenu(backstories, (def) => def.title.CapitalizeFirst(), (def) => () => this.backstory = def);
			}
#endif
		}

		public override string Summary(PawnFilter filter) =>
#if V1_0 || V1_1 || V1_2 || V1_3
			"HasAdulthood".Translate(this.Backstory.title);
#else
			"HasAdulthood".Translate(this.backstory.title);
#endif

		public override void Randomize() =>
#if V1_0 || V1_1 || V1_2 || V1_3
			this.backstoryIdentifier = BackstoryDatabase.allBackstories.Values
				.Where((backstory) => backstory.slot == BackstorySlot.Adulthood)
				.RandomElement().identifier;
#else
			this.backstory = DefDatabase<BackstoryDef>.AllDefsListForReading
				.Where((def) => def.slot == BackstorySlot.Adulthood)
				.RandomElement();
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
