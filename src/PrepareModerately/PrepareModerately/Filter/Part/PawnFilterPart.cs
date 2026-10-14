using System;
using System.Collections.Generic;

using Lakuna.PrepareModerately.Ui;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part {
	// Based on `RimWorld.ScenPart`.
	public abstract class PawnFilterPart : IExposable {
		public virtual string Label =>
			this.def.label;

		public virtual PawnFilterPartDef PDef {
			get => this.def;
			set => this.def = value;
		}

		internal virtual PawnFilterPart CopyForEditing =>
			(PawnFilterPart)this.MemberwiseClone();

		internal virtual bool Summarized {
			get; set;
		}

		internal virtual bool Visible =>
			true;

		[TranslationHandle]
		private PawnFilterPartDef def;

		public virtual void ExposeData() =>
			Scribe_Defs.Look(ref this.def, nameof(this.def));

		public virtual bool Matches(Pawn pawn) =>
			true;

		public virtual bool NotMatches(Pawn pawn) =>
			!this.Matches(pawn);

		public virtual string Summary(PawnFilter filter) =>
			this.def.description;

		internal virtual bool canCoexistWith(PawnFilterPart other) =>
			true;

		internal virtual IEnumerable<string> configErrors() {
			if (this.def is null) {
				yield return $"{this.GetType()} has a null definition.".CapitalizeFirst();
			}
		}

		internal virtual void doEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.getPawnFilterPartRect(this, 0, out totalAddedListHeight);
		}

		internal virtual bool hasNullDefs() =>
			this.def is null;

		internal virtual void randomize() {
		}

		internal virtual bool tryMerge(PawnFilterPart other) =>
			false;
	}
}
