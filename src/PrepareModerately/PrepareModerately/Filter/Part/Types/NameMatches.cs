using System;
using System.Text.RegularExpressions;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class NameMatches : PawnFilterPart {
#pragma warning restore CA1812
		private string _regex;

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._regex, MiscUtil.NoUnder(nameof(this._regex)));
		}

		public override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			: new Regex(this._regex).Matches(pawn.Name.ToStringFull).Count > 0;
#else
			: new Regex(this._regex).Count(pawn.Name.ToStringFull) > 0;
#endif

		public override string Summary(PawnFilter filter) =>
			"PM.NameMatches".Translate(this._regex);

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			this._regex = Widgets.TextArea(rect, this._regex);
		}

		internal override void Randomize() {
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
	}
}
