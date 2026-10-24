#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
using System;
#endif
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Lakuna.PrepareModerately.Ui;
#if !V1_0
using Lakuna.PrepareModerately.Utility;
#endif

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

		public override bool Matches(Pawn pawn) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new Regex(this._regex ?? "").Matches(pawn.Name?.ToStringFull ?? "null").Count > 0;
#else
			new Regex(this._regex ?? "").Count(pawn.Name?.ToStringFull ?? "null") > 0;
#endif

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

			this._regex = names.RandomElement();
		}

		public override string Summary(PawnFilter filter) =>
#if V1_0
			"PM.NameMatches".Translate(this._regex) ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.NameMatches".Translate(this._regex).StringSafe();
#else
			"PM.NameMatches".Translate(this._regex).StringSafe;
#endif
	}
}
