#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
using System;
#endif
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class NameMatches : PawnFilterPart {
		public override IEnumerable<string> ConfigErrors {
			get {
				foreach (string item in base.ConfigErrors) {
					yield return item;
				}

				if (this._regex is null) {
					yield return "Name matches filter part regex is null.";
				}
			}
		}

		public override bool Valid =>
			base.Valid
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			&& !(this._regex is null);
#else
			&& this._regex is not null;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private string _regex = "";
#else
		private string? _regex = "";
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
			this._regex = Widgets.TextArea(rect, this._regex) ?? "";
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._regex, "regex");
		}

		public override int GetHashCode() =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
			base.GetHashCode() ^ (this._regex?.GetHashCode() ?? 0);
#else
			base.GetHashCode() ^ (this._regex?.GetHashCode(StringComparison.Ordinal) ?? 0);
#endif

		public override bool Matches(Pawn pawn) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Regex regex = new Regex(this._regex ?? "");
#else
			Regex regex = new(this._regex ?? "");
#endif
			string name = pawn.Name?.ToStringFull ?? "null";
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			return regex.Matches(name).Count > 0;
#else
			return regex.Count(name) > 0;
#endif
		}

		public override void Randomize() {
			// These names are all guaranteed to be in the game due to the name-in-game list.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
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
#else
			string[] names = [
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
			];
#endif

			List<string> foo = ["a", "b"];

			this._regex = names.RandomElement();
		}

		public override string Summary(PawnFilter filter) =>
			"PM.NameMatches".TranslateFull(this._regex);
	}
}
