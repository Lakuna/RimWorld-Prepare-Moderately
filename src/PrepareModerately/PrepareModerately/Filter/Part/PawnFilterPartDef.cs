using System;
using System.Collections.Generic;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part {
	// Based on `RimWorld.ScenPartDef`.
	internal sealed class PawnFilterPartDef : Def {
#pragma warning disable CA1051, CS0649, IDE1006 // `Def` relies on static fields with names matching XML definitions.
		internal readonly PawnFilterPartCategory category;

		internal readonly Type filterPartClass;

		internal readonly int maxUses;
#pragma warning restore CA1051, CS0649, IDE1006

		internal bool _playerAddRemovable =>
			this.category != PawnFilterPartCategory.Fixed;

#pragma warning disable CA1051, CS0649, IDE1006 // `Def` relies on static fields with names matching XML definitions.
		internal readonly float selectionWeight;

		internal readonly float summaryPriority;
#pragma warning restore CA1051, CS0649, IDE1006

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
