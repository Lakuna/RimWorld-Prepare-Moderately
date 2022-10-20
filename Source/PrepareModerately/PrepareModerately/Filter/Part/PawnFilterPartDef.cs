using System;
using System.Collections.Generic;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part {
	public class PawnFilterPartDef : Def {
#pragma warning disable CA1051 // Definitions loaded from XML files must use instance fields.
		public readonly PawnFilterPartCategory category;

		public readonly Type filterPartClass;

		public readonly float summaryPriority;

		public readonly float selectionWeight;

		public readonly int maxUses;
#pragma warning restore CA1051

		public bool PlayerAddRemovable => this.category != PawnFilterPartCategory.Fixed;

		public override IEnumerable<string> ConfigErrors() {
			foreach (string item in base.ConfigErrors()) {
				yield return item;
			}

			if (this.filterPartClass == null) {
				yield return "Filter part class is null.";
			}
		}

		public PawnFilterPartDef() {
			this.summaryPriority = 1;
			this.selectionWeight = 1;
			this.maxUses = 999999;
		}
	}
}
