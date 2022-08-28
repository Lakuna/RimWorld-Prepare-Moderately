using System.Linq;
using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasAdulthoodFilterPart : FilterPart {
		public Backstory backstory;

		public override bool Matches(Pawn pawn) {
			return pawn.story.adulthood == this.backstory;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.backstory.title.CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(BackstoryDatabase.allBackstories.Values.Where((Backstory backstory) => backstory.slot == BackstorySlot.Adulthood),
					(Backstory backstory) => backstory.title.CapitalizeFirst(),
					(Backstory backstory) => () => this.backstory = backstory);
			}
		}

		public override string Summary(Filter filter) {
			return "HasAdulthood".Translate(this.backstory.title);
		}

		public override void Randomize() {
			Backstory[] values = BackstoryDatabase.allBackstories.Values.Where((Backstory backstory) => backstory.slot == BackstorySlot.Adulthood).ToArray();
			this.backstory = values[Rand.Range(0, values.Length)];
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.backstory, nameof(this.backstory));
		}
	}
}

// TODO: Make incompatible with incompatible traits.
// TODO: Make incompatible with NOT filters that exclude required traits.
// TODO: Make incompatible with disabled work types.
// TODO: Make incompatible with other backstories than required backstories.
