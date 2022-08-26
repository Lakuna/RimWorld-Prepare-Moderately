using Lakuna.PrepareModerately.UI;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class NameContainsFilterPart : FilterPart {
		public string substring;

		public override bool Matches(Pawn pawn) {
			return pawn.Name.ToStringFull.Contains(this.substring);
		}

		public override void DoEditInterface(FilterEditListing listing) {
			this.substring = Widgets.TextArea(listing.GetFilterPartRect(this, Text.LineHeight), this.substring);
		}

		public override string Summary(Filter filter) {
			return "Name contains \"" + this.substring + "\".";
		}

		public override void Randomize() {
			// All of the available random names (listed below) are people who purchased the name-in-game option.
			switch (Rand.RangeInclusive(0, 20)) {
				case 0:
					this.substring = "Tynan"; // Lead developer.
					break;
				case 1:
					this.substring = "Travis"; // Me!
					break;
				case 2:
					this.substring = "Amelia"; // Best pawn ever.
					break;
				case 3:
					this.substring = "Aelanna"; // Comic artist: https://www.reddit.com/user/Aelanna/.
					break;
				case 4:
					this.substring = "Alyssa";
					break;
				case 5:
					this.substring = "Benjamin";
					break;
				case 6:
					this.substring = "Christian";
					break;
				case 7:
					this.substring = "Daniel";
					break;
				case 8:
					this.substring = "Emily";
					break;
				case 9:
					this.substring = "Felix";
					break;
				case 10:
					this.substring = "James";
					break;
				case 11:
					this.substring = "Jonathan";
					break;
				case 12:
					this.substring = "Lia";
					break;
				case 13:
					this.substring = "Lumine";
					break;
				case 14:
					this.substring = "Matthew";
					break;
				case 15:
					this.substring = "Nathaniel";
					break;
				case 16:
					this.substring = "Peter";
					break;
				case 17:
					this.substring = "Priscilla";
					break;
				case 18:
					this.substring = "Robert";
					break;
				case 19:
					this.substring = "Sam";
					break;
				case 20:
					this.substring = "Skye";
					break;
			}
		}
	}
}
