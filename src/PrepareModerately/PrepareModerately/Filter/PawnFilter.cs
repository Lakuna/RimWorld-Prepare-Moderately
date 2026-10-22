using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Lakuna.PrepareModerately.Filter.Part;
using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.Scenario`.
	public sealed class PawnFilter : IExposable {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static PawnFilter Current {
#else
		public static PawnFilter? Current {
#endif
			get; set;
		}

		public const int DescriptionMaxLength = 1000;

		public const int NameMaxLength = 55;

		public const int SummaryMaxLength = 300;

		public bool CanEdit =>
			this.Category == PawnFilterCategory.CustomLocal;

		public PawnFilterCategory Category { get; set; } = PawnFilterCategory.Undefined;

		public IEnumerable<string> ConfigErrors {
			get {
				if (this.Description is null) {
					yield return "Filter has null description.";
				}

				if (this.Name is null) {
					yield return "Filter has null name.";
				}

				if (this.Summary is null) {
					yield return "Filter has null summary.";
				}

				if (this.Parts is null) {
					yield return "Filter has null parts.";
				} else {
					foreach (PawnFilterPart part in this.Parts) {
						foreach (string item in part.ConfigErrors) {
							yield return item;
						}
					}
				}
			}
		}

		public PawnFilter CopyForEditing {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				PawnFilter copyForEditing = new PawnFilter() {
#else
				PawnFilter copyForEditing = new() {
#endif
					Category = PawnFilterCategory.CustomLocal,
					Description = this.Description,
					Name = this.Name,
					Summary = this.Summary
				};
				if (copyForEditing.Parts is null) {
					PrepareModeratelyLogger.LogErrorMessage("Created copy of filter with null parts.");
				} else {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					foreach (PawnFilterPart part in this.Parts ?? new List<PawnFilterPart>()) {
#else
					foreach (PawnFilterPart part in this.Parts ?? []) {
#endif
						copyForEditing.Parts.Add(part.CopyForEditing);
					}
				}

				return copyForEditing;
			}
		}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public string Description {
#else
		public string? Description {
#endif
			get => this._description;
			set => this._description = value;
		}

		public bool Enabled { get; set; } = true;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public FileInfo File =>
			this.FileName is null ? null
			: new FileInfo(PawnFilterFilePaths.AbsolutePathForName(this.FileName));
#else
		public FileInfo? File =>
			this.FileName is null ? null
			: new(PawnFilterFilePaths.AbsolutePathForName(this.FileName));
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public string FileName {
#else
		public string? FileName {
#endif
			get; set;
		}

		public string FullInformationText {
			get {
				try {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					StringBuilder stringBuilder = new StringBuilder();
#else
					StringBuilder stringBuilder = new();
#endif
					stringBuilder = stringBuilder.AppendLine(this.Description);
					stringBuilder = stringBuilder.AppendLine();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					foreach (PawnFilterPart part in (this.Parts ?? new List<PawnFilterPart>())
#else
					foreach (PawnFilterPart part in (this.Parts ?? [])
#endif
						.OrderByDescending((part) => part.Def?.SummaryPriority)
						.ThenBy((part) => part.Def?.defName)
						.Where((part) => part.Visible)) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
						string summary = part.Summary(this).CapitalizeFirstSafe().EndWithPeriodSafe();
#else
						string summary = part.Summary(this).CapitalizeFirstSafe.EndWithPeriodSafe;
#endif
						if (summary.NullOrEmpty()) {
							continue;
						}

						stringBuilder = stringBuilder.AppendLine(summary);
					}

					return stringBuilder.ToString().Trim();
				} catch (Exception e) {
					PrepareModeratelyLogger.LogException(e, "Failed to get full information text.", PrepareModeratelyLoggerCategory.GetFullInformationText);
#if V1_0
					return "PM.FailedToGetFullInformationText".Translate()?.CapitalizeFirstSafe().EndWithPeriodSafe() ?? "null";
#else
					return "PM.FailedToGetFullInformationText".Translate().CapitalizeFirst().EndWithPeriod();
#endif
					throw;
				}
			}
		}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public string Name {
#else
		public string? Name {
#endif
			get => this._name;
			set => this._name = value;
		}

#pragma warning disable CA1002 // `Scribe_Collections.Look` must be used with a `List`.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public List<PawnFilterPart> Parts =>
#else
		public List<PawnFilterPart>? Parts =>
#endif
#pragma warning restore CA1002
			this._parts;

		public bool ShowInUi { get; set; } = true;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public string Summary {
#else
		public string? Summary {
#endif
			get => this._summary;
			set => this._summary = value;
		}

		public bool Valid { get; set; } = true;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private string _description = "";
#else
		private string? _description = "";
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private string _name = "";
#else
		private string? _name = "";
#endif

#pragma warning disable CA1002 // `Scribe_Collections.Look` must be used with a `List`.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private List<PawnFilterPart> _parts = new List<PawnFilterPart>();
#else
		private List<PawnFilterPart>? _parts = [];
#endif
#pragma warning restore CA1002

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private string _summary = "";
#else
		private string? _summary = "";
#endif

		public void ExposeData() {
			Scribe_Values.Look(ref this._name, "name");
			Scribe_Values.Look(ref this._summary, "summary");
			Scribe_Values.Look(ref this._description, "description");
			Scribe_Collections.Look(ref this._parts, "parts", LookMode.Deep);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Scribe.mode == LoadSaveMode.PostLoadInit && this.Parts != null) {
#else
			if (Scribe.mode == LoadSaveMode.PostLoadInit && this.Parts is not null) {
#endif
				int removed = 0;
				for (int i = this.Parts.Count - 1; i >= 0; i--) {
					if (this.Parts[i].Valid) {
						continue;
					}

					this.Parts.RemoveAt(i);
					removed++;
				}

				if (removed > 0) {
					PrepareModeratelyLogger.LogErrorMessage("Some filter parts were invalid.");
				}
			}
		}

		public override int GetHashCode() {
			int hash = 6126121;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
			hash ^= this.Name?.GetHashCode() ?? 0;
			hash ^= this.Summary?.GetHashCode() ?? 0;
			hash ^= this.Description?.GetHashCode() ?? 0;
#else
			hash ^= this.Name?.GetHashCode(StringComparison.Ordinal) ?? 0;
			hash ^= this.Summary?.GetHashCode(StringComparison.Ordinal) ?? 0;
			hash ^= this.Description?.GetHashCode(StringComparison.Ordinal) ?? 0;
#endif
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (this.Parts != null) {
#else
			if (this.Parts is not null) {
#endif
				for (int i = 0; i < this.Parts.Count; i++) {
					hash ^= this.Parts[i].GetHashCode() + i++;
				}
			}

			return hash;
		}

		public override string ToString() =>
			(this.Name is null || this.Name.NullOrEmpty()) ? "PM.UnnamedFilter".Translate().ToString().CapitalizeFirst()
			: this.Name;

		public void AddPart(PawnFilterPart part, bool prepend = false) {
			if (this.Parts is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to add part to filter with null parts.");
				return;
			}

			if (prepend) {
				this.Parts.Insert(0, part);
				return;
			}

			this.Parts.Add(part);
		}

		public bool CanReorder(PawnFilterPart part, ReorderDirection dir) {
			if (part is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to check if it was possible to reorder a null filter part.");
				return false;
			}

			if (part.Def is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to check if it was possible to reorder a filter part with null definition.");
				return false;
			}

			if (!part.Def.PlayerAddRemovable) {
				return false;
			}

			if (this.Parts is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to check if it was possible to reorder in a pawn filter with null parts.");
				return false;
			}

			int index = this.Parts.IndexOf(part);
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			switch (dir) {
				case ReorderDirection.Up:
					return index > 0 && (this.Parts[index - 1].Def?.PlayerAddRemovable ?? false);
				case ReorderDirection.Down:
					return index < this.Parts.Count - 1;
				default:
					return false;
			}
#else
			return dir switch {
				ReorderDirection.Up => index > 0 && (this.Parts[index - 1].Def?.PlayerAddRemovable ?? false),
				ReorderDirection.Down => index < this.Parts.Count - 1,
				_ => false
			};
#endif
		}

		public bool Matches(Pawn pawn) {
			if (this.Parts is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to match pawn with filter with null parts.");
				return true;
			}

			return this.Parts.All((part) => part.IgnoreFor(pawn) || part.Matches(pawn)); // AND gate logic.
		}

		public void PreConfigure() {
			if (this.Parts is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to pre-configure a filter with null parts.");
				return;
			}

			foreach (PawnFilterPart part in this.Parts) {
				part.PreConfigure();
			}
		}

		public void RemovePart(PawnFilterPart part) {
			if (this.Parts is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to remove part from filter with null parts.");
				return;
			}

			if (!this.Parts.Remove(part)) {
				PrepareModeratelyLogger.LogErrorMessage("Failed to remove filter part.");
			}
		}

		public void Reorder(PawnFilterPart part, ReorderDirection dir) {
			if (this.Parts is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to reorder part in filter with null parts.");
				return;
			}

			int index = this.Parts.IndexOf(part);
			this.Parts.RemoveAt(index);
			switch (dir) {
				case ReorderDirection.Up:
					this.Parts.Insert(index - 1, part);
					break;
				case ReorderDirection.Down:
					this.Parts.Insert(index + 1, part);
					break;
				default:
					return;
			}
		}
	}
}
