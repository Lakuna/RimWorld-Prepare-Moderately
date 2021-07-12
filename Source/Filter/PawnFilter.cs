using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Verse;

namespace PrepareModerately.Filter {
	public class PawnFilter {
		[Serializable]
		public class PawnFilterSerializable {
			public string name;
			public PawnFilterPart.PawnFilterPartSerializable[] parts;

			private PawnFilterSerializable() { } // Parameterless constructor necessary for serialization.

			public PawnFilterSerializable(PawnFilter pawnFilter) {
				this.name = pawnFilter.name;

				List<PawnFilterPart.PawnFilterPartSerializable> parts = new List<PawnFilterPart.PawnFilterPartSerializable>();
				foreach (PawnFilterPart pawnFilterPart in pawnFilter.parts) { parts.Add(pawnFilterPart.Serialize()); }
				this.parts = parts.ToArray();
			}

			public PawnFilter Deserialize() {
				PawnFilter pawnFilter = new PawnFilter {
					name = this.name
				};
				foreach (PawnFilterPart.PawnFilterPartSerializable pawnFilterPart in this.parts) {
					pawnFilter.parts.Add(pawnFilterPart.Deserialize());
				}
				return pawnFilter;
			}
		}

		// List of defs collected only once to save memory.
		public static List<SkillDef> allSkills = DefDatabase<SkillDef>.AllDefsListForReading;
		public static List<PawnRelationDef> allRelations = DefDatabase<PawnRelationDef>.AllDefsListForReading;
		public static List<TraitDef> allTraits = DefDatabase<TraitDef>.AllDefsListForReading;
		public static List<WorkTypeDef> allWorkTypes = DefDatabase<WorkTypeDef>.AllDefsListForReading;
		public static List<StatDef> allStats = DefDatabase<StatDef>.AllDefsListForReading;
		public static List<PawnFilterPartDef> allFilterParts = DefDatabase<PawnFilterPartDef>.AllDefsListForReading;
		private static List<ThingDef> allHumanlikeDefs;
		public static List<ThingDef> AllHumanlikeDefs {
			get {
				if (allHumanlikeDefs == null) {
					allHumanlikeDefs = new List<ThingDef>();
					foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading) {
						if (def.race != null && def.race.Humanlike) { allHumanlikeDefs.Add(def); }
					}
				}
				return allHumanlikeDefs;
			}
		}

		public string name;
		public readonly List<PawnFilterPart> parts;

		public PawnFilter() {
			this.name = "Filter Name";
			this.parts = new List<PawnFilterPart>();
		}

		public bool Matches(Pawn pawn) {
			foreach (PawnFilterPart part in this.parts) {
				if (!part.Matches(pawn)) { return false; }
			}
			return true;
		}

		public void Save(string path) {
			XmlSerializer serializer = new XmlSerializer(typeof(PawnFilterSerializable));
			using (StreamWriter writer = new StreamWriter(path)) { serializer.Serialize(writer, new PawnFilterSerializable(this)); }
		}

		public void Load(string path) {
			// Load saved filter.
			PawnFilter loadedPawnFilter;
			XmlSerializer serializer = new XmlSerializer(typeof(PawnFilterSerializable));
			using (StreamReader reader = new StreamReader(path)) { loadedPawnFilter = ((PawnFilterSerializable) serializer.Deserialize(reader)).Deserialize(); }

			// Copy attributes from loaded filter.
			this.name = loadedPawnFilter.name;
			this.parts.Clear();
			foreach (PawnFilterPart pawnFilterPart in loadedPawnFilter.parts) { this.parts.Add(pawnFilterPart); }
		}
	}
}
