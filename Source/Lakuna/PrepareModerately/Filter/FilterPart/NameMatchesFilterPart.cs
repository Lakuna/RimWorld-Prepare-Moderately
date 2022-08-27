using System.Text.RegularExpressions;
using Lakuna.PrepareModerately.UI;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class NameMatchesFilterPart : FilterPart {
		public string regex;

		public override bool Matches(Pawn pawn) {
			return new Regex(this.regex).Matches(pawn.Name.ToStringFull).Count > 0;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			this.regex = Widgets.TextArea(listing.GetFilterPartRect(this, Text.LineHeight), this.regex);
		}

		public override string Summary(Filter filter) {
			return "NameMatches".Translate(this.regex);
		}

		public override void Randomize() {
			// All of the available random names (listed below) are people who purchased the name-in-game option.
			switch (Rand.RangeInclusive(0, 20)) {
				case 0:
					this.regex = "Tynan"; // Lead developer.
					break;
				case 1:
					this.regex = "Travis"; // Me!
					break;
				case 2:
					this.regex = "Amelia"; // Best pawn ever.
					break;
				case 3:
					this.regex = "Aelanna"; // Comic artist: https://www.reddit.com/user/Aelanna/.
					break;
				case 4:
					this.regex = "Alyssa";
					break;
				case 5:
					this.regex = "Benjamin";
					break;
				case 6:
					this.regex = "Christian";
					break;
				case 7:
					this.regex = "Daniel";
					break;
				case 8:
					this.regex = "Emily";
					break;
				case 9:
					this.regex = "Felix";
					break;
				case 10:
					this.regex = "James";
					break;
				case 11:
					this.regex = "Jonathan";
					break;
				case 12:
					this.regex = "Lia";
					break;
				case 13:
					this.regex = "Lumine";
					break;
				case 14:
					this.regex = "Matthew";
					break;
				case 15:
					this.regex = "Nathaniel";
					break;
				case 16:
					this.regex = "Peter";
					break;
				case 17:
					this.regex = "Priscilla";
					break;
				case 18:
					this.regex = "Robert";
					break;
				case 19:
					this.regex = "Sam";
					break;
				case 20:
					this.regex = "Skye";
					break;
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.regex, nameof(this.regex));
		}
	}
}
