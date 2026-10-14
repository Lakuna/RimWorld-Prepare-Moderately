using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

using Lakuna.PrepareModerately.Filter.Part;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.Scenario`.
	public sealed class PawnFilter : IExposable {
		public static IEnumerable<Version> SupportedVersions =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new Version[] { new Version(VersionControl.CurrentMajor, VersionControl.CurrentMinor) };
#else
			[new(VersionControl.CurrentMajor, VersionControl.CurrentMinor)];
#endif

		internal static PawnFilter Current {
			get; set;
		}

		internal static readonly string DataFolder = $"PawnFilters{Path.DirectorySeparatorChar}";

		internal const int DescriptionMaxLength = 1000;

		internal const string FileExtension = ".rpf";

		internal const int NameMaxLength = 55;

		internal const int SummaryMaxLength = 300;

		internal static IEnumerable<FileInfo> AllFiles {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				DirectoryInfo directoryInfo = new DirectoryInfo(DataPath);
#else
				DirectoryInfo directoryInfo = new(DataPath);
#endif
				if (!directoryInfo.Exists) {
					directoryInfo.Create();
				}

				return from file in directoryInfo.GetFiles()
					   where file.Extension == FileExtension
					   orderby file.LastWriteTime descending
					   select file;
			}
		}

		internal static string DefaultDataPath =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			(string)typeof(GenFilePaths)
				.GetMethod("FolderUnderSaveData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
				.Invoke(null, new object[] { DataFolder });
#else
			(string)typeof(GenFilePaths)
				.GetMethod("FolderUnderSaveData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
				.Invoke(null, [DataFolder]);
#endif

		internal static IEnumerable<PawnFilter> LocalFilters =>
			LocalFiltersInternal;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly Collection<PawnFilter> LocalFiltersInternal = new Collection<PawnFilter>();
#else
		private static readonly Collection<PawnFilter> LocalFiltersInternal = [];
#endif

		private static string DataPath =>
			PrepareModeratelyMod.Settings.PFilterSavePath.NullOrEmpty() ? DefaultDataPath
			: PrepareModeratelyMod.Settings.PFilterSavePath;

		public PawnFilterCategory PCategory {
			get {
				if (this.category == PawnFilterCategory.Undefined) {
					PrepareModeratelyLogger.LogErrorMessage("Filter category is undefined.");
				}

				return this.category;
			}

			set => this.category = value;
		}

		public string PDescription {
			get => this.description;
			set => this.description = value;
		}

		public string PName {
			get => this.name;
			set => this.name = value;
		}

		public IEnumerable<PawnFilterPart> PParts =>
			this.parts;

		public string PSummary {
			get => this.summary;
			set => this.summary = value;
		}

		internal bool Valid {
			get; set;
		}

		internal PawnFilter CopyForEditing {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				PawnFilter copyForEditing = new PawnFilter {
					PName = this.PName,
					PSummary = this.PSummary,
					PDescription = this.PDescription,
					PCategory = PawnFilterCategory.CustomLocal
				};
#else
				PawnFilter copyForEditing = new() {
					PName = this.PName,
					PSummary = this.PSummary,
					PDescription = this.PDescription,
					PCategory = PawnFilterCategory.CustomLocal
				};
#endif
				copyForEditing.parts.AddRange(this.PParts.Select((part) => part.CopyForEditing));
				return copyForEditing;
			}
		}

		internal bool Enabled {
			get;
		}

		internal FileInfo File =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new FileInfo(AbsolutePathForName(this.FileName));
#else
			new(AbsolutePathForName(this.FileName));
#endif

		[field: NoTranslate]
		internal string FileName {
			get; set;
		}

		internal string FullInformationText {
			get {
				try {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					StringBuilder stringBuilder = new StringBuilder();
#else
					StringBuilder stringBuilder = new();
#endif
					_ = stringBuilder.AppendLine(this.PDescription);
					_ = stringBuilder.AppendLine();

					foreach (PawnFilterPart part in this.PParts) {
						part.Summarized = false;
					}

					foreach (PawnFilterPart part in
						from part in this.PParts
						orderby part.PDef.PSummaryPriority descending,
						part.PDef.defName
						where part.Visible
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

		internal bool ShowInUi {
			get;
		}

		private PawnFilterCategory category;

		[MustTranslate]
		private string description;

		[MustTranslate]
		private string name;

#pragma warning disable CA1002 // RimWorld requires a `List` to serialize with `Scribe_Collections.Look`.
		private List<PawnFilterPart> parts;
#pragma warning restore CA1002

		[MustTranslate]
		private string summary;

		public PawnFilter() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this.parts = new List<PawnFilterPart>();
#else
			this.parts = [];
#endif
			this.Enabled = true;
			this.ShowInUi = true;
		}

		internal static string AbsolutePathForName(string filterName) =>
			Path.Combine(DataPath, filterName + FileExtension);

		internal static void RecacheLocalFilters() {
			LocalFiltersInternal.Clear();
			foreach (FileInfo file in AllFiles) {
				if (PawnFilterSaveLoader.Load(file.FullName, PawnFilterCategory.CustomLocal, out PawnFilter filter)) {
					LocalFiltersInternal.Add(filter);
				}
			}
		}

		public void AddPart(PawnFilterPart part, bool prepend = false) {
			if (prepend) {
				this.parts.Insert(0, part);
				return;
			}

			this.parts.Add(part);
		}

		public void ExposeData() {
			Scribe_Values.Look(ref this.name, nameof(this.name));
			Scribe_Values.Look(ref this.summary, nameof(this.summary));
			Scribe_Values.Look(ref this.description, nameof(this.description));
			Scribe_Collections.Look(ref this.parts, nameof(this.parts), LookMode.Deep);

			if (Scribe.mode == LoadSaveMode.PostLoadInit) {
				if (this.parts.RemoveAll((part) => part is null) > 0) {
					PrepareModeratelyLogger.LogErrorMessage("Some filter parts were null after loading.");
				}

				if (this.parts.RemoveAll((part) => part.hasNullDefs()) > 0) {
					PrepareModeratelyLogger.LogErrorMessage("Some filter parts had null definitions.");
				}
			}
		}

		public override int GetHashCode() {
			int hash = 5251977;
			if (this.PName != null) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				hash ^= this.PName.GetHashCode();
#else
				hash ^= this.PName.GetHashCode(StringComparison.Ordinal);
#endif
			}

			if (this.PSummary != null) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				hash ^= this.PSummary.GetHashCode();
#else
				hash ^= this.PSummary.GetHashCode(StringComparison.Ordinal);
#endif
			}

			if (this.PDescription != null) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				hash ^= this.PDescription.GetHashCode();
#else
				hash ^= this.PDescription.GetHashCode(StringComparison.Ordinal);
#endif
			}

			return hash;
		}

		public bool Matches(Pawn pawn) {
			foreach (PawnFilterPart part in this.PParts) {
				if (!part.Matches(pawn)) {
					return false;
				}
			}

			return true;
		}

		public void RemovePart(PawnFilterPart part) {
			if (!this.parts.Contains(part)) {
				PrepareModeratelyLogger.LogErrorMessage("Failed to remove filter part.");
			}

			_ = this.parts.Remove(part);
		}

		public override string ToString() =>
			this.PName.NullOrEmpty() ? "PM.UnnamedFilter".Translate().ToString().CapitalizeFirst()
			: this.PName;

		internal bool canReorder(PawnFilterPart part, ReorderDirection dir) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (part is null) {
				throw new ArgumentNullException(nameof(part));
			}
#else
			ArgumentNullException.ThrowIfNull(part);
#endif

			if (!part.PDef.PlayerAddRemovable) {
				return false;
			}

			int index = this.parts.IndexOf(part);
			switch (dir) {
				case ReorderDirection.Up:
					if (index == 0) {
						return false;
					}

					if (index > 0 && !this.parts[index - 1].PDef.PlayerAddRemovable) {
						return false;
					}

					return true;
				case ReorderDirection.Down:
					return index != this.parts.Count - 1;
				default:
					throw new NotImplementedException();
			}
		}

		internal IEnumerable<string> configErrors() {
			if (this.PName.NullOrEmpty()) {
				yield return "No title.";
			}

			// if (this.parts.NullOrEmpty()) { yield return "No parts."; }

			foreach (PawnFilterPart part in this.PParts) {
				foreach (string item in part.configErrors()) {
					yield return item;
				}
			}
		}

		internal void reorder(PawnFilterPart part, ReorderDirection dir) {
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
