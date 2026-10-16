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
	internal sealed class PawnFilter : IExposable {
		internal static PawnFilter Current {
			get; set;
		}

		private static readonly string s_dataFolder = $"PawnFilters{Path.DirectorySeparatorChar}";

		internal const int DescriptionMaxLength = 1000;

		private const string FileExtension = ".rpf";

		internal const int NameMaxLength = 55;

		internal const int SummaryMaxLength = 300;

		// Based on `Verse.GenFilePaths.AllCustomScenarioFiles`.
		internal static IList<FileInfo> AllFiles {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				DirectoryInfo directoryInfo = new DirectoryInfo(DataPath);
#else
				DirectoryInfo directoryInfo = new(DataPath);
#endif
				if (!directoryInfo.Exists) {
					directoryInfo.Create();
				}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				return directoryInfo
					.GetFiles()
					.Where((file) => file.Extension == FileExtension)
					.OrderByDescending((file) => file.LastWriteTime).ToList();
#else
				return [.. directoryInfo
					.GetFiles()
					.Where((file) => file.Extension == FileExtension)
					.OrderByDescending((file) => file.LastWriteTime)];
#endif
			}
		}

		internal static string DefaultDataPath =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			(string)typeof(GenFilePaths)
				.GetMethod("FolderUnderSaveData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
				.Invoke(null, new object[] { s_dataFolder });
#else
			(string)typeof(GenFilePaths)
				.GetMethod("FolderUnderSaveData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
				.Invoke(null, [s_dataFolder]);
#endif

		internal static IEnumerable<PawnFilter> LocalFilters =>
			s_localFiltersInternal;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly Collection<PawnFilter> s_localFiltersInternal = new Collection<PawnFilter>();
#else
		private static readonly Collection<PawnFilter> s_localFiltersInternal = [];
#endif

		private static string DataPath =>
			PrepareModeratelyMod.Settings.filterSavePath.NullOrEmpty() ? DefaultDataPath
			: PrepareModeratelyMod.Settings.filterSavePath;

		internal PawnFilterCategory Category {
			get {
				if (this._category == PawnFilterCategory.Undefined) {
					PrepareModeratelyLogger.LogErrorMessage("Filter category is undefined.");
				}

				return this._category;
			}

			set => this._category = value;
		}

		internal bool Valid { get; set; } = true;

		internal PawnFilter CopyForEditing {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				PawnFilter copyForEditing = new PawnFilter {
					name = this.name,
					summary = this.summary,
					description = this.description,
					Category = PawnFilterCategory.CustomLocal
				};
#else
				PawnFilter copyForEditing = new() {
					name = this.name,
					summary = this.summary,
					description = this.description,
					Category = PawnFilterCategory.CustomLocal
				};
#endif
				copyForEditing.parts.AddRange(this.parts.Select((part) => part.CopyForEditing));
				return copyForEditing;
			}
		}

		internal bool Enabled { get; } = true;

		internal FileInfo File =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new FileInfo(AbsolutePathForName(this.FileName));
#else
			new(AbsolutePathForName(this.FileName));
#endif

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
					_ = stringBuilder.AppendLine(this.description);
					_ = stringBuilder.AppendLine();

					foreach (PawnFilterPart part in this.parts) {
						part.Summarized = false;
					}

					foreach (PawnFilterPart part in
						from part in this.parts
						orderby part.def.summaryPriority descending, part.def.defName
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

		internal bool ShowInUi { get; } = true;

		private PawnFilterCategory _category;

#pragma warning disable CA1002, CA1051, IDE1006 // `Def` relies on static fields with names matching XML definitions.
		internal string description;

		internal string name;

		internal List<PawnFilterPart> parts;

		internal string summary;
#pragma warning restore CA1002, CA1051, IDE1006

		// Must be public.
		public PawnFilter() =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this.parts = new List<PawnFilterPart>();
#else
			this.parts = [];
#endif

		// Based on `Verse.GenFilePaths.AbsPathForScenario`.
		internal static string AbsolutePathForName(string filterName) =>
			Path.Combine(DataPath, filterName + FileExtension);

		// Based on `RimWorld.ScenarioFiles.RecacheData`.
		internal static void RecacheLocalFilters() {
			s_localFiltersInternal.Clear();
			foreach (FileInfo file in AllFiles) {
				if (PawnFilterSaveLoader.Load(file.FullName, PawnFilterCategory.CustomLocal, out PawnFilter filter)) {
					s_localFiltersInternal.Add(filter);
				}
			}
		}

		internal void AddPart(PawnFilterPart part, bool prepend = false) {
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

		internal bool Matches(Pawn pawn) {
			foreach (PawnFilterPart part in this.parts) {
				if (!part.Matches(pawn)) {
					return false;
				}
			}

			return true;
		}

		internal void RemovePart(PawnFilterPart part) {
			if (!this.parts.Contains(part)) {
				PrepareModeratelyLogger.LogErrorMessage("Failed to remove filter part.");
			}

			_ = this.parts.Remove(part);
		}

		public override string ToString() =>
			this.name.NullOrEmpty() ? "PM.UnnamedFilter".Translate().ToString().CapitalizeFirst()
			: this.name;

		internal bool CanReorder(PawnFilterPart part, ReorderDirection dir) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (part is null) {
				throw new ArgumentNullException(nameof(part));
			}
#else
			ArgumentNullException.ThrowIfNull(part);
#endif

			if (!part.def.PlayerAddRemovable) {
				return false;
			}

			int index = this.parts.IndexOf(part);
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			switch (dir) {
				case ReorderDirection.Up:
					return index > 0 && this.parts[index - 1].def.PlayerAddRemovable;
				case ReorderDirection.Down:
					return index != this.parts.Count - 1;
				default:
					throw new NotImplementedException();
			}
#else
			return dir switch {
				ReorderDirection.Up => index > 0 && this.parts[index - 1].def.PlayerAddRemovable,
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

		internal void PreConfigure() {
			foreach (PawnFilterPart part in this.parts) {
				part.PreConfigure();
			}
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
