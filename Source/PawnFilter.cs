using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Verse;

namespace PrepareModerately {
	public class PawnFilter {
		[Serializable]
		public class SerializablePawnFilter {
			public static SerializablePawnFilter Load(string path) {
				XmlSerializer serializer = new XmlSerializer(typeof(SerializablePawnFilter));
				using (StreamReader reader = new StreamReader(path)) { return (SerializablePawnFilter) serializer.Deserialize(reader); }
			}

			public string name;
			public PawnFilterPart.SerializablePawnFilterPart[] parts;

			private SerializablePawnFilter() { } // Parameterless constructor necessary for serialization.

			public SerializablePawnFilter(PawnFilter pawnFilter) {
				this.name = pawnFilter.name;

				List<PawnFilterPart.SerializablePawnFilterPart> parts = new List<PawnFilterPart.SerializablePawnFilterPart>();
				foreach (PawnFilterPart pawnFilterPart in pawnFilter.parts) { parts.Add(pawnFilterPart.Serialize()); }
				this.parts = parts.ToArray();
			}

			public void Save(string path) {
				XmlSerializer serializer = new XmlSerializer(typeof(SerializablePawnFilter));
				using (StreamWriter writer = new StreamWriter(path)) { serializer.Serialize(writer, this); }
			}

			public PawnFilter Deserialize() {
				PawnFilter pawnFilter = new PawnFilter {
					name = this.name
				};
				foreach (PawnFilterPart.SerializablePawnFilterPart pawnFilterPart in this.parts) {
					pawnFilter.parts.Add(pawnFilterPart.Deserialize());
				}
				return pawnFilter;
			}
		}

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

		public void Save(string path) => new SerializablePawnFilter(this).Save(path);

		public void Load(string path) {
			PawnFilter loadedPawnFilter = SerializablePawnFilter.Load(path).Deserialize();

			this.name = loadedPawnFilter.name;
			this.parts.Clear();
			foreach (PawnFilterPart pawnFilterPart in loadedPawnFilter.parts) { this.parts.Add(pawnFilterPart); }
		}
	}
}
