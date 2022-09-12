using System.Linq;
using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasAdulthoodFilterPart : FilterPart {
		private string backstoryIdentifier;

		public Backstory Backstory {
			get {
				return BackstoryDatabase.allBackstories.Values
					.Where((Backstory backstory) => backstory.slot == BackstorySlot.Adulthood)
					.First((Backstory backstory) => backstory.identifier == this.backstoryIdentifier);
			}
		}

		public override bool Matches(Pawn pawn) {
			return pawn.story.adulthood == this.Backstory;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.Backstory.title.CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(BackstoryDatabase.allBackstories.Values.Where((Backstory backstory) => backstory.slot == BackstorySlot.Adulthood),
					(Backstory backstory) => backstory.title.CapitalizeFirst(),
					(Backstory backstory) => () => this.backstoryIdentifier = backstory.identifier);
			}
		}

		public override string Summary(Filter filter) {
			return "HasAdulthood".Translate(this.Backstory.title);
		}

		public override void Randomize() {
			Backstory[] values = BackstoryDatabase.allBackstories.Values.Where((Backstory backstory) => backstory.slot == BackstorySlot.Adulthood).ToArray();
			this.backstoryIdentifier = values[Rand.Range(0, values.Length - 1)].identifier;
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.backstoryIdentifier, nameof(this.backstoryIdentifier));
		}
	}
}

// TODO: Make incompatible with incompatible traits.
// TODO: Make incompatible with NOT filters that exclude required traits.
// TODO: Make incompatible with disabled work types.
// TODO: Make incompatible with other backstories than required backstories.
