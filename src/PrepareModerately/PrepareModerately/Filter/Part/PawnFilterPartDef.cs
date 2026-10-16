using System;
using System.Collections.Generic;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part {
	// Based on `RimWorld.ScenPartDef`.
	public sealed class PawnFilterPartDef : Def {
#pragma warning disable CA1051, IDE1006 // `Def` relies on static fields with names matching XML definitions.
		public readonly PawnFilterPartCategory category;

		public readonly Type filterPartClass;

		public readonly int maxUses;

		public readonly float selectionWeight;

		public readonly float summaryPriority;
#pragma warning restore CA1051, IDE1006

		internal bool PlayerAddRemovable =>
			this.category != PawnFilterPartCategory.Fixed;

		public PawnFilterPartDef() {
			this.summaryPriority = 1;
			this.selectionWeight = 1;
			this.maxUses = 999999;
		}

		public override IEnumerable<string> ConfigErrors() {
			foreach (string item in base.ConfigErrors()) {
				yield return item;
			}

			if (this.filterPartClass is null) {
				yield return "Filter part class is null.";
			}
		}
	}
}
