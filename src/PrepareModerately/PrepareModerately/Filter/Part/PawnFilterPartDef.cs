using System;
using System.Collections.Generic;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part {
	// Based on `RimWorld.ScenPartDef`.
	public class PawnFilterPartDef : Def {
		public PawnFilterPartCategory PCategory =>
			this.category;

		internal Type PFilterPartClass =>
			this.filterPartClass;

		internal bool PlayerAddRemovable =>
			this.PCategory != PawnFilterPartCategory.Fixed;

		internal int PMaxUses =>
			this.maxUses;

		internal float PSelectionWeight =>
			this.selectionWeight;

		internal float PSummaryPriority =>
			this.summaryPriority;

#pragma warning disable CS0649 // Value is assigned via XML definitions.
		private readonly PawnFilterPartCategory category;
#pragma warning restore CS0649

#pragma warning disable CS0649 // Value is assigned via XML definitions.
		private readonly Type filterPartClass;
#pragma warning restore CS0649

		private readonly int maxUses;

		private readonly float selectionWeight;

		private readonly float summaryPriority;

		public PawnFilterPartDef() {
			this.summaryPriority = 1;
			this.selectionWeight = 1;
			this.maxUses = 999999;
		}

		public override IEnumerable<string> ConfigErrors() {
			foreach (string item in base.ConfigErrors()) {
				yield return item;
			}

			if (this.PFilterPartClass is null) {
				yield return "Filter part class is null.";
			}
		}
	}
}
