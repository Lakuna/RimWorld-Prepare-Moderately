using System;
using System.Collections.Generic;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part {
	public class PawnFilterPartDef : Def {
		public PawnFilterPartCategory Category { get; }

		public Type FilterPartClass { get; }

		public float SummaryPriority { get; }

		public float SelectionWeight { get; }

		public int MaxUses { get; }

		public bool PlayerAddRemovable => this.Category != PawnFilterPartCategory.Fixed;

		public override IEnumerable<string> ConfigErrors() {
			foreach (string item in base.ConfigErrors()) {
				yield return item;
			}

			if (this.FilterPartClass == null) {
				yield return "Filter part class is null.";
			}
		}

		public PawnFilterPartDef() {
			this.SummaryPriority = 1;
			this.SelectionWeight = 1;
			this.MaxUses = 999999;
		}
	}
}
