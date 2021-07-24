using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Verse;

namespace PrepareModerately.PawnFilter {
	public class PawnFilter {
		public string name;
		public List<PawnFilterPart> parts;

		public PawnFilter() {
			this.name = "Name";
			this.parts = new List<PawnFilterPart>();
		}

		public void CreatePart(PawnFilterPartDef def) => this.parts.Add((PawnFilterPart) Activator.CreateInstance(def.partClass));

		public bool Matches(Pawn pawn) => this.parts.All((part) => part.Matches(pawn));

		public void Load(string path) {
			try {
				Directory.CreateDirectory(PrepareModerately.dataPath);
				XmlSerializer serializer = new XmlSerializer(typeof(PawnFilter));
				using (StreamReader reader = new StreamReader(path)) {
					PawnFilter loadedFilter = (PawnFilter) serializer.Deserialize(reader);
					this.name = loadedFilter.name;
					this.parts = loadedFilter.parts;
				}
			} catch (Exception e) {
				PrepareModerately.LogError(e);
			}
		}

		public void Save() {
			try {
				Directory.CreateDirectory(PrepareModerately.dataPath);
				XmlSerializer serializer = new XmlSerializer(typeof(PawnFilter));
				using (StreamWriter writer = new StreamWriter(PrepareModerately.dataPath + "\\" + this.name + PrepareModerately.filterExtension)) {
					serializer.Serialize(writer, this);
				}
			} catch (Exception e) {
				PrepareModerately.LogError(e);
			}
		}
	}
}
