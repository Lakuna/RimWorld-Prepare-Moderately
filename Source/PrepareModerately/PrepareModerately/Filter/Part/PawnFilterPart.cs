using Lakuna.PrepareModerately.UI;
using System;
using System.Collections.Generic;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part {
	// Based on `RimWorld.ScenPart`.
	public abstract class PawnFilterPart : IExposable {
		[TranslationHandle]
		private PawnFilterPartDef def;

		public virtual PawnFilterPartDef Def {
			get => this.def;
			set => this.def = value;
		}

		public virtual bool Visible => true;

		public virtual bool Summarized { get; set; }

		public virtual string Label => this.def.label;

		public virtual void ExposeData() => Scribe_Defs.Look(ref this.def, nameof(this.def));

		public virtual PawnFilterPart CopyForEditing() {
			PawnFilterPart copyForEditing = this.CopyForEditingInner();
			copyForEditing.def = this.def;
			return copyForEditing;
		}

		private PawnFilterPart CopyForEditingInner() => (PawnFilterPart)this.MemberwiseClone();

		public virtual void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight);
		}

		public virtual string Summary(PawnFilter filter) => this.def.description;

		public virtual IEnumerable<string> GetSummaryListEntries(string tag) {
			yield break;
		}

		// Helper method for Randomize().
		public static T GetRandomOfEnum<T>(T t) where T : Enum {
			Array values = Enum.GetValues(t.GetType());
			return (T)values.GetValue(Rand.Range(0, values.Length - 1));
		}

		public virtual void Randomize() { }

		public virtual bool TryMerge(PawnFilterPart other) => false;

		public virtual bool CanCoexistWith(PawnFilterPart other) => true;

		public virtual bool Matches(Pawn pawn) => true;

		public virtual bool NotMatches(Pawn pawn) => !this.Matches(pawn);

		public virtual IEnumerable<string> ConfigErrors() {
			if (this.def == null) {
				yield return (this.GetType().ToString() + " has a null definition.").CapitalizeFirst();
			}
		}

		public virtual bool HasNullDefs() => this.def == null;
	}
}
