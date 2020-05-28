using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using RimWorld;
using Verse;

namespace PrepareModerately {
	public class PawnFilter {
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
			string output = "name=" + this.name;
			foreach (PawnFilterPart filterPart in this.parts) { output += "\n" + filterPart.ToLoadableString(); }
			using (StreamWriter writer = new StreamWriter(path)) { writer.Write(output); }
		}

		public void Load(string path) {
			string s = "";
			using (StreamReader reader = new StreamReader(path)) { s = reader.ReadToEnd(); }
			this.parts.Clear();
			foreach (string line in s.Split('\n')) {
				if (line.StartsWith("name=")) { this.name = line.Substring("name=".Length); } else {
					string type = line.Split(' ')[0];
					try {
						PawnFilterPart part = (PawnFilterPart) Activator.CreateInstance(allFilterParts.Find(def => def.partClass.Name == type).partClass);
						Log.Message("1: " + part.label);
						part.FromLoadableString(line);
						Log.Message("2: " + part.ToLoadableString());
						this.parts.Add(part);
						Log.Message("3: " + this.parts.Count);
					} catch (Exception e) { Log.Error("Exception with filter type \"" + type + "\":\n" + e + "\n"); }
				}
			}
		}
	}
}
