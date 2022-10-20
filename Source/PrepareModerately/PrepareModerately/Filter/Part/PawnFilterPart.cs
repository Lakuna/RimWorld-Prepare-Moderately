using Lakuna.PrepareModerately.UI;
using System;
using System.Collections.Generic;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part {
	public class PawnFilterPart : IExposable {
		[TranslationHandle]
		private PawnFilterPartDef def;

		public PawnFilterPartDef Def {
			get => this.def;
			set => this.def = value;
		}

		public bool Visible { get; }

		public bool Summarized { get; set; }

		public string Label => this.def.label;

		public void ExposeData() => Scribe_Defs.Look(ref this.def, nameof(this.def));

		public PawnFilterPart CopyForEditing() {
			PawnFilterPart copyForEditing = this.CopyForEditingInner();
			copyForEditing.def = this.def;
			return copyForEditing;
		}

		private PawnFilterPart CopyForEditingInner() => (PawnFilterPart)this.MemberwiseClone();

		public void DoEditInterface(PawnFilterEditListing listing) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0);
		}

#pragma warning disable IDE0060 // filter parameter exists so that subclasses may utilize it.
		public string Summary(PawnFilter filter) => this.def.description;
#pragma warning restore IDE0060

#pragma warning disable IDE0060, CA1822 // tag parameter exists so that subclasses may utilize it.
		public IEnumerable<string> GetSummaryListEntries(string tag) {
#pragma warning restore IDE0060, CA1822
			yield break;
		}

		// Helper method for Randomize().
		public static T GetRandomOfEnum<T>(T t) where T : Enum {
			Array values = Enum.GetValues(t.GetType());
			return (T)values.GetValue(Rand.Range(0, values.Length - 1));
		}

		// Helper method for Randomize().
#pragma warning disable IDE0060 // t is used only for its type (T).
		public static T GetRandomOfDef<T>(T t) where T : Def, new() {
#pragma warning restore IDE0060
			List<T> values = DefDatabase<T>.AllDefsListForReading;
			return values[Rand.Range(0, values.Count - 1)];
		}

#pragma warning disable CA1822 // Randomize is defined so that subclasses may utilize it.
		public void Randomize() { }
#pragma warning restore CA1822

#pragma warning disable IDE0060, CA1822 // other parameter exists so that subclasses may utilize it.
		public bool TryMerge(PawnFilterPart other) => false;
#pragma warning restore IDE0060, CA1822

#pragma warning disable IDE0060, CA1822 // other parameter exists so that subclasses may utilize it.
		public bool CanCoexistWith(PawnFilterPart other) => false;
#pragma warning restore IDE0060, CA1822

#pragma warning disable IDE0060, CA1822 // pawn parameter exists so that subclasses may utilize it.
		public bool Matches(Pawn pawn) => true;
#pragma warning restore IDE0060, CA1822

		public IEnumerable<string> ConfigErrors() {
			if (this.def == null) {
				yield return (this.GetType().ToString() + " has a null definition.").CapitalizeFirst();
			}
		}

		public bool HasNullDefs() => this.def == null;

		public PawnFilterPart() => this.Visible = true;
	}
}
