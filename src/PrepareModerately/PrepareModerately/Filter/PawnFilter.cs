using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Lakuna.PrepareModerately.Filter.Part;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.Scenario`.
	internal sealed class PawnFilter : IExposable {
		internal static PawnFilter _current;

		internal const int _descriptionMaxLength = 1000;

		internal const int _nameMaxLength = 55;

		internal const int _summaryMaxLength = 300;

		internal bool _canEdit =>
			this._category == PawnFilterCategory.CustomLocal;

		internal PawnFilterCategory _category { get; set; } = PawnFilterCategory.Undefined;

		internal PawnFilter _copyForEditing {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				PawnFilter copyForEditing = new PawnFilter {
					name = this.name,
					summary = this.summary,
					description = this.description,
					_category = PawnFilterCategory.CustomLocal
				};
#else
				PawnFilter copyForEditing = new() {
					name = this.name,
					summary = this.summary,
					description = this.description,
					_category = PawnFilterCategory.CustomLocal
				};
#endif
				copyForEditing.parts.AddRange(this.parts.Select((part) => part._copyForEditing));
				return copyForEditing;
			}
		}

#pragma warning disable CA1002, CA1051, IDE1006 // `Def` relies on static fields with names matching XML definitions.
		internal string description;
#pragma warning restore CA1002, CA1051, IDE1006

		internal bool _enabled { get; } = true;

		internal FileInfo _file =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new FileInfo(PawnFilterFilePaths.AbsolutePathForName(this._fileName));
#else
			new(PawnFilterFilePaths.AbsolutePathForName(this._fileName));
#endif

		internal string _fileName {
			get; set;
		}

		internal string _fullInformationText {
			get {
				try {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					StringBuilder stringBuilder = new StringBuilder();
#else
					StringBuilder stringBuilder = new();
#endif
					_ = stringBuilder.AppendLine(this.description);
					_ = stringBuilder.AppendLine();

					foreach (PawnFilterPart part in this.parts) {
						part._summarized = false;
					}

					foreach (PawnFilterPart part in
						from part in this.parts
						orderby part.def.summaryPriority descending, part.def.defName
						where part._visible
						select part) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
						string summary = $"{part.Summary(this).CapitalizeFirst()}.";
#else
						string summary = part.Summary(this).CapitalizeFirst().EndWithPeriod();
#endif
						if (!summary.NullOrEmpty()) {
							_ = stringBuilder.AppendLine(summary);
						}
					}

					return stringBuilder.ToString().TrimEndNewlines();
#pragma warning disable CA1031 // Don't rethrow the exception to avoid messing with the game.
				} catch (Exception e) {
#pragma warning restore CA1031
					PrepareModeratelyLogger.LogException(e, "Failed to get full information text.", PrepareModeratelyLoggerCategory.GetFullInformationText);
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
					return $"{"PM.FailedToGetFullInformationText".Translate().CapitalizeFirst()}.";
#else
					return "PM.FailedToGetFullInformationText".Translate().CapitalizeFirst().EndWithPeriod();
#endif
				}
			}
		}

#pragma warning disable CA1002, CA1051, IDE1006 // `Def` relies on static fields with names matching XML definitions.
		internal string name;

		internal List<PawnFilterPart> parts;
#pragma warning restore CA1002, CA1051, IDE1006

		internal bool _showInUi { get; } = true;

#pragma warning disable CA1002, CA1051, IDE1006 // `Def` relies on static fields with names matching XML definitions.
		internal string summary;
#pragma warning restore CA1002, CA1051, IDE1006

		internal bool _valid { get; set; } = true;

		// Must be public.
		public PawnFilter() =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this.parts = new List<PawnFilterPart>();
#else
			this.parts = [];
#endif

		public void ExposeData() {
			Scribe_Values.Look(ref this.name, nameof(this.name));
			Scribe_Values.Look(ref this.summary, nameof(this.summary));
			Scribe_Values.Look(ref this.description, nameof(this.description));
			Scribe_Collections.Look(ref this.parts, nameof(this.parts), LookMode.Deep);

			if (Scribe.mode == LoadSaveMode.PostLoadInit) {
				if (this.parts.RemoveAll((part) => !part.IsValid()) > 0) {
					PrepareModeratelyLogger.LogErrorMessage("Some filter parts were invalid.");
				}
			}
		}

		public override int GetHashCode() {
			int hash = 6126121;
			if (this.name != null) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				hash ^= this.name.GetHashCode();
#else
				hash ^= this.name.GetHashCode(StringComparison.Ordinal);
#endif
			}

			if (this.summary != null) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				hash ^= this.summary.GetHashCode();
#else
				hash ^= this.summary.GetHashCode(StringComparison.Ordinal);
#endif
			}

			if (this.description != null) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				hash ^= this.description.GetHashCode();
#else
				hash ^= this.description.GetHashCode(StringComparison.Ordinal);
#endif
			}

			int i = 0;
			foreach (PawnFilterPart part in this.parts) {
				hash ^= part.GetHashCode() + i++;
			}

			return hash;
		}

		public override string ToString() =>
			this.name.NullOrEmpty() ? "PM.UnnamedFilter".Translate().ToString().CapitalizeFirst()
			: this.name;

		internal void AddPart(PawnFilterPart part, bool prepend = false) {
			if (prepend) {
				this.parts.Insert(0, part);
				return;
			}

			this.parts.Add(part);
		}

		internal bool CanReorder(PawnFilterPart part, ReorderDirection dir) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (part is null) {
				throw new ArgumentNullException(nameof(part));
			}
#else
			ArgumentNullException.ThrowIfNull(part);
#endif

			if (!part.def._playerAddRemovable) {
				return false;
			}

			int index = this.parts.IndexOf(part);
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			switch (dir) {
				case ReorderDirection.Up:
					return index > 0 && this.parts[index - 1].def._playerAddRemovable;
				case ReorderDirection.Down:
					return index != this.parts.Count - 1;
				default:
					throw new NotImplementedException();
			}
#else
			return dir switch {
				ReorderDirection.Up => index > 0 && this.parts[index - 1].def._playerAddRemovable,
				ReorderDirection.Down => index != this.parts.Count - 1,
				_ => throw new NotImplementedException(),
			};
#endif
		}

		internal IEnumerable<string> ConfigErrors() {
			if (this.name.NullOrEmpty()) {
				yield return "No title.";
			}

			// if (this.parts.NullOrEmpty()) { yield return "No parts."; }

			foreach (PawnFilterPart part in this.parts) {
				if (part is null) {
					yield return "Filter has null part.";
					continue;
				}

				foreach (string item in part.ConfigErrors()) {
					yield return item;
				}
			}
		}

		internal bool Matches(Pawn pawn) {
			foreach (PawnFilterPart part in this.parts) {
				if (!part.Matches(pawn)) {
					return false;
				}
			}

			return true;
		}

		internal void PreConfigure() {
			foreach (PawnFilterPart part in this.parts) {
				part.PreConfigure();
			}
		}

		internal void RemovePart(PawnFilterPart part) {
			if (!this.parts.Contains(part)) {
				PrepareModeratelyLogger.LogErrorMessage("Failed to remove filter part.");
			}

			_ = this.parts.Remove(part);
		}

		internal void Reorder(PawnFilterPart part, ReorderDirection dir) {
			int index = this.parts.IndexOf(part);
			this.parts.RemoveAt(index);
			switch (dir) {
				case ReorderDirection.Up:
					this.parts.Insert(index - 1, part);
					break;
				case ReorderDirection.Down:
					this.parts.Insert(index + 1, part);
					break;
				default:
					throw new NotImplementedException();
			}
		}
	}
}
