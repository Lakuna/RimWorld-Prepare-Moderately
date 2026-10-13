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
	public class PawnFilter : IExposable {
		public static PawnFilter Current {
			get; set;
		}

		[MustTranslate]
		private string name;

		public string NameP {
			get => this.name;
			set => this.name = value;
		}

		[MustTranslate]
		private string summary;

		public string SummaryP {
			get => this.summary;
			set => this.summary = value;
		}

		[MustTranslate]
		private string description;

		public string DescriptionP {
			get => this.description;
			set => this.description = value;
		}

		private Collection<PawnFilterPart> parts;

		public IEnumerable<PawnFilterPart> PartsP => this.parts;

		public void AddPart(PawnFilterPart part, bool prepend = false) {
			if (prepend) {
				this.parts.Insert(0, part);
			} else {
				this.parts.Add(part);
			}
		}

		public void RemovePart(PawnFilterPart part) {
			if (!this.parts.Contains(part)) {
				PrepareModeratelyLogger.LogErrorMessage("Failed to remove filter part.");
			}

			_ = this.parts.Remove(part);
		}

		private PawnFilterCategory category;

		public PawnFilterCategory CategoryP {
			get {
				if (this.category == PawnFilterCategory.Undefined) {
					PrepareModeratelyLogger.LogErrorMessage("Filter category is undefined.");
				}

				return this.category;
			}

			set => this.category = value;
		}

		[field: NoTranslate]
		public string FileName {
			get; set;
		}

		public bool Enabled {
			get;
		}

		public bool ShowInUi {
			get;
		}

		public const int NameMaxLength = 55;

		public const int SummaryMaxLength = 300;

		public const int DescriptionMaxLength = 1000;

		public static IEnumerable<Version> SupportedVersions =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new Version[] { new Version(VersionControl.CurrentMajor, VersionControl.CurrentMinor) };
#else
			[new(VersionControl.CurrentMajor, VersionControl.CurrentMinor)];
#endif

		public FileInfo File =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new FileInfo(AbsolutePathForName(this.FileName));
#else
			new(AbsolutePathForName(this.FileName));
#endif

		public void ExposeData() {
			Scribe_Values.Look(ref this.name, nameof(this.name));
			Scribe_Values.Look(ref this.summary, nameof(this.summary));
			Scribe_Values.Look(ref this.description, nameof(this.description));


#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			List<PawnFilterPart> parts = this.parts.ToList();
#else
			List<PawnFilterPart> parts = [.. this.parts];
#endif
			Scribe_Collections.Look(ref parts, nameof(parts), LookMode.Deep);
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
			this.parts = new Collection<PawnFilterPart>(parts);
#elif V1_6
			this.parts = new Collection(parts);
#else
			this.parts = new(parts);
#endif

			if (Scribe.mode == LoadSaveMode.PostLoadInit) {
				if (this.parts.Any((part) => part is null)) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
					while (this.parts.Any((part) => part is null)) {
						this.parts.Remove(this.parts.First((part) => part is null));
					}
#else
					this.parts.RemoveWhere((part) => part is null);
#endif
					PrepareModeratelyLogger.LogErrorMessage("Some filter parts were null after loading.");
				}

				if (this.parts.Any((part) => part.HasNullDefs())) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
					while (this.parts.Any((part) => part.HasNullDefs())) {
						this.parts.Remove(this.parts.First((part) => part.HasNullDefs()));
					}
#else
					this.parts.RemoveWhere((part) => part.HasNullDefs());
#endif
					PrepareModeratelyLogger.LogErrorMessage("Some filter parts had null definitions.");
				}
			}
		}

		public IEnumerable<string> ConfigErrors() {
			if (this.NameP.NullOrEmpty()) {
				yield return "No title.";
			}

			// if (this.parts.NullOrEmpty()) { yield return "No parts."; }

			foreach (PawnFilterPart part in this.PartsP) {
				foreach (string item in part.ConfigErrors()) {
					yield return item;
				}
			}
		}

		public string FullInformationText {
			get {
				try {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					StringBuilder stringBuilder = new StringBuilder();
#else
					StringBuilder stringBuilder = new();
#endif
					_ = stringBuilder.AppendLine(this.DescriptionP);
					_ = stringBuilder.AppendLine();

					foreach (PawnFilterPart part in this.PartsP) {
						part.Summarized = false;
					}

					foreach (PawnFilterPart part in
						from part in this.PartsP
						orderby part.DefP.summaryPriority descending,
						part.DefP.defName
						where part.Visible
						select part) {
						string summary = MiscellaneousUtility.EndWithPeriod(part.Summary(this).CapitalizeFirst());
						if (!summary.NullOrEmpty()) {
							_ = stringBuilder.AppendLine(summary);
						}
					}

					return stringBuilder.ToString().TrimEndNewlines();
#pragma warning disable CA1031 // Don't rethrow the exception to avoid messing with the game.
				} catch (Exception e) {
#pragma warning restore CA1031
					PrepareModeratelyLogger.LogException(e, "Failed to get full information text.", PrepareModeratelyLoggerCategory.GetFullInformationText);
					return MiscellaneousUtility.EndWithPeriod("PM.FailedToGetFullInformationText".Translate().CapitalizeFirst());
				}
			}
		}

		public PawnFilter CopyForEditing {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				PawnFilter copyForEditing = new PawnFilter {
#else
				PawnFilter copyForEditing = new() {
#endif
					NameP = this.NameP,
					SummaryP = this.SummaryP,
					DescriptionP = this.DescriptionP,
					CategoryP = PawnFilterCategory.CustomLocal,
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
					parts = new Collection<PawnFilterPart>(this.parts.Select((part) => part.CopyForEditing()).ToList())
#elif V1_6
					parts = new Collection(this.parts.Select((part) => part.CopyForEditing()).ToList())
#else
					parts = new([.. this.parts.Select((part) => part.CopyForEditing())])
#endif
				};
				return copyForEditing;
			}
		}

		public bool Matches(Pawn pawn) {
			foreach (PawnFilterPart part in this.PartsP) {
				if (!part.Matches(pawn)) {
					return false;
				}
			}

			return true;
		}

		public bool CanReorder(PawnFilterPart part, ReorderDirection dir) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (part is null) {
				throw new ArgumentNullException(nameof(part));
			}
#else
			ArgumentNullException.ThrowIfNull(part);
#endif

			if (!part.DefP.PlayerAddRemovable) {
				return false;
			}

			int index = this.parts.IndexOf(part);
			switch (dir) {
				case ReorderDirection.Up:
					if (index == 0) {
						return false;
					}

					if (index > 0 && !this.parts[index - 1].DefP.PlayerAddRemovable) {
						return false;
					}

					return true;
				case ReorderDirection.Down:
					return index != this.parts.Count - 1;
				default:
					throw new NotImplementedException();
			}
		}

		public void Reorder(PawnFilterPart part, ReorderDirection dir) {
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

		public override string ToString() => this.NameP.NullOrEmpty() ? "PM.UnnamedFilter".Translate().ToString().CapitalizeFirst() : this.NameP;

		public override int GetHashCode() {
			int hash = 5251977;
			if (this.NameP != null) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				hash ^= this.NameP.GetHashCode();
#else
				hash ^= this.NameP.GetHashCode(StringComparison.Ordinal);
#endif
			}

			if (this.SummaryP != null) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				hash ^= this.SummaryP.GetHashCode();
#else
				hash ^= this.SummaryP.GetHashCode(StringComparison.Ordinal);
#endif
			}

			if (this.DescriptionP != null) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				hash ^= this.DescriptionP.GetHashCode();
#else
				hash ^= this.DescriptionP.GetHashCode(StringComparison.Ordinal);
#endif
			}

			return hash;
		}

		public PawnFilter() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this.parts = new Collection<PawnFilterPart>();
#else
			this.parts = [];
#endif
			this.Enabled = true;
			this.ShowInUi = true;
		}

		public const string FileExtension = ".rpf";

		public const string DataFolder = "PawnFilters/";

		public static string DefaultDataPath => (string)typeof(GenFilePaths)
			.GetMethod("FolderUnderSaveData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			.Invoke(null, new object[] { DataFolder });
#else
			.Invoke(null, [DataFolder]);
#endif

		public static string DataPath => PrepareModeratelyMod.Settings.FilterSavePathP.NullOrEmpty() ? DefaultDataPath : PrepareModeratelyMod.Settings.FilterSavePathP;

		public static IEnumerable<FileInfo> AllFiles {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				DirectoryInfo directoryInfo = new DirectoryInfo(DataPath);
#else
				DirectoryInfo directoryInfo = new(DataPath);
#endif
				if (!directoryInfo.Exists) {
					directoryInfo.Create();
				}

				return from file in directoryInfo.GetFiles() where file.Extension == FileExtension orderby file.LastWriteTime descending select file;
			}
		}

		public static string AbsolutePathForName(string filterName) => Path.Combine(DataPath, filterName + FileExtension);

		private static readonly Collection<PawnFilter> LocalFiltersInternal =
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new Collection<PawnFilter>();
#else
			[];
#endif

		public static IEnumerable<PawnFilter> LocalFilters => LocalFiltersInternal;

		public static void RecacheLocalFilters() {
			LocalFiltersInternal.Clear();
			foreach (FileInfo file in AllFiles) {
				if (PawnFilterSaveLoader.Load(file.FullName, PawnFilterCategory.CustomLocal, out PawnFilter filter)) {
					LocalFiltersInternal.Add(filter);
				}
			}
		}
	}
}
