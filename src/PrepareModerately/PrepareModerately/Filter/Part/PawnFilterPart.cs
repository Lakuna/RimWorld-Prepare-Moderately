using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part {
	// Based on `RimWorld.ScenPart`.
	public abstract class PawnFilterPart : IExposable {
		[TranslationHandle]
		private PawnFilterPartDef def;

		public virtual PawnFilterPartDef PDef {
			get => this.def;
			set => this.def = value;
		}

		public virtual bool Visible => true;

		public virtual bool Summarized {
			get; set;
		}

		public virtual string Label => this.def.label;

		public virtual void ExposeData() => Scribe_Defs.Look(ref this.def, nameof(this.def));

		public virtual PawnFilterPart CopyForEditing() {
			PawnFilterPart copyForEditing = this.CopyForEditingInner();
			copyForEditing.def = this.def;
			return copyForEditing;
		}

		private PawnFilterPart CopyForEditingInner() => (PawnFilterPart)this.MemberwiseClone();

		public virtual void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight);
		}

		public virtual string Summary(PawnFilter filter) => this.def.description;

		public virtual IEnumerable<string> GetSummaryListEntries(string tag) {
			yield break;
		}

		public virtual void Randomize() {
		}

		public virtual bool TryMerge(PawnFilterPart other) => false;

		public virtual bool CanCoexistWith(PawnFilterPart other) => true;

		public virtual bool Matches(Pawn pawn) => true;

		public virtual bool NotMatches(Pawn pawn) => !this.Matches(pawn);

		public virtual IEnumerable<string> ConfigErrors() {
			if (this.def is null) {
				yield return $"{this.GetType()} has a null definition.".CapitalizeFirst();
			}
		}

		public virtual bool HasNullDefs() => this.def is null;
	}
}
