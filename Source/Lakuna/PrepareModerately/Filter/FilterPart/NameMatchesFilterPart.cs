using System.Text.RegularExpressions;
using Lakuna.PrepareModerately.UI;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class NameMatchesFilterPart : FilterPart {
		public string regex;

		public override bool Matches(Pawn pawn) {
			return new Regex(this.regex).Matches(pawn.Name.ToStringFull).Count > 0;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight);
			this.regex = Widgets.TextArea(rect, this.regex);
		}

		public override string Summary(Filter filter) {
			return "NameMatches".Translate(this.regex);
		}

		public override void Randomize() {
			// These names are all guaranteed to be in the game due to the name-in-game list.
			string[] names = new string[] {
				"Tynan", // Lead developer of RimWorld.
				"Travis", // Me!
				"Amelia", // The best pawn ever.
				"Aelanna", // RimWorld comic artist: https://www.reddit.com/user/Aelanna/.
				"Alyssa",
				"Benjamin",
				"Christian",
				"Daniel",
				"Emily",
				"Felix",
				"James",
				"Jonathan",
				"Lia",
				"Lumine",
				"Matthew",
				"Nathaniel",
				"Peter",
				"Priscilla",
				"Robert",
				"Sam",
				"Skye"
			};

			this.regex = (string)names.GetValue(Rand.Range(0, names.Length));
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.regex, nameof(this.regex));
		}
	}
}
