using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasChildhood : PawnFilterPart {
#if V1_0 || V1_1 || V1_2 || V1_3
		private string backstoryIdentifier;

		public Backstory Backstory => BackstoryDatabase.allBackstories.Values
			.Where((Backstory backstory) => backstory.slot == BackstorySlot.Childhood)
			.First((Backstory backstory) => backstory.identifier == this.backstoryIdentifier);
#else
		private BackstoryDef backstory;
#endif

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
#if V1_0 || V1_1 || V1_2 || V1_3
			: pawn.story.childhood == this.Backstory;
#else
			: pawn.story.Childhood == this.backstory;
#endif

		public override void DoEditInterface(PawnFilterEditListing listing) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight);
#if V1_0 || V1_1 || V1_2 || V1_3
			if (Widgets.ButtonText(rect, this.Backstory.title.CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(BackstoryDatabase.allBackstories.Values.Where((Backstory backstory) => backstory.slot == BackstorySlot.Childhood),
					(Backstory backstory) => backstory.title.CapitalizeFirst(),
					(Backstory backstory) => () => this.backstoryIdentifier = backstory.identifier);
			}
#else
			if (Widgets.ButtonText(rect, this.backstory.title.CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(DefDatabase<BackstoryDef>.AllDefsListForReading.Where((BackstoryDef def) => def.slot == BackstorySlot.Childhood),
					(BackstoryDef def) => def.title.CapitalizeFirst(),
					(BackstoryDef def) => () => this.backstory = def);
			}
#endif
		}

		public override string Summary(PawnFilter filter) =>
#if V1_0 || V1_1 || V1_2 || V1_3
			"HasChildhood".Translate(this.Backstory.title);
#else
			"HasChildhood".Translate(this.backstory.title);
#endif

		public override void Randomize() {
#if V1_0 || V1_1 || V1_2 || V1_3
			Backstory[] values = BackstoryDatabase.allBackstories.Values.Where((Backstory backstory) => backstory.slot == BackstorySlot.Childhood).ToArray();
			this.backstoryIdentifier = values[Rand.Range(0, values.Length - 1)].identifier;
#else
			this.backstory = GetRandomOfDef<BackstoryDef>();
#endif
		}

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
