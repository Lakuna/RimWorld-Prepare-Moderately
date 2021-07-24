using PrepareModerately.UI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class LogicGate : PawnFilterPart {
		private const float filterHeightBuffer = 100;

		public enum Type { AND, OR, XOR, NOT }

		public PawnFilter innerFilter;
		public Type type;
		private float filterHeight;

		public LogicGate() {
			this.innerFilter = new PawnFilter();
			this.type = Type.AND;
			this.filterHeight = 0;
		}

		public override bool Matches(Pawn pawn) {
			switch (this.type) {
				case Type.AND:
					return this.innerFilter.parts.All((part) => part.Matches(pawn));
				case Type.OR:
					return this.innerFilter.parts.Any((part) => part.Matches(pawn));
				case Type.XOR:
					bool alreadyMatched = false;
					foreach (PawnFilterPart part in this.innerFilter.parts) {
						if (part.Matches(pawn)) {
							if (alreadyMatched) {
								return false;
							}
							alreadyMatched = true;
						}
					}
					return alreadyMatched;
				case Type.NOT: // NAND
					return !this.innerFilter.parts.Any((part) => part.Matches(pawn));
				default:
					throw new Exception("Unknown logic gate type \"" + this.type.ToString() + "\"");
			}
		}

		public override void DoEditInterface(Listing_PawnFilter listing) {
			Rect rect = listing.GetPawnFilterPartRect(this, (Text.LineHeight * 3) + this.filterHeight);

			Rect typeRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(typeRect, this.type.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((Type[]) Enum.GetValues(typeof(Type)), (type) => type.ToString().CapitalizeFirst(), (type) => () => this.type = type);
			}

			Rect addPartRect = new Rect(rect.x, typeRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(addPartRect, "Add part")) {
				FloatMenuUtility.MakeMenu(DefDatabase<PawnFilterPartDef>.AllDefsListForReading, (def) => def.label, (def) => () => this.innerFilter.CreatePart(def));
			}

			Rect loadRect = new Rect(rect.x, addPartRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(loadRect, "Load")) {
				try {
					Directory.CreateDirectory(PrepareModerately.dataPath);
					string[] filePaths = Directory.GetFiles(PrepareModerately.dataPath).Where((filePath) => filePath.EndsWith(PrepareModerately.filterExtension)).ToArray();
					if (filePaths.Length > 0) {
						FloatMenuUtility.MakeMenu(filePaths, (filePath) => {
							int start = filePath.LastIndexOf("\\") + 1;
							int end = filePath.LastIndexOf(PrepareModerately.filterExtension);
							return filePath.Substring(start, end - start);
						}, (filePath) => () => this.innerFilter.Load(filePath));
					} else {
						FloatMenuUtility.MakeMenu(new string[] { "N/A" }, (s) => s, (s) => () => { });
					}
				} catch (Exception e) {
					PrepareModerately.LogError(e);
				}
			}

			Rect filterRect = new Rect(rect.x, loadRect.yMax, rect.width, 0);
			Widgets.DrawMenuSection(filterRect);
			filterRect = filterRect.GetInnerRect();
			filterRect.height = this.filterHeight;

			Listing_PawnFilter filterListing = new Listing_PawnFilter() { ColumnWidth = filterRect.width };
			filterListing.Begin(filterRect);
			List<PawnFilterPart> partsToRemove = new List<PawnFilterPart>();
			foreach (PawnFilterPart part in this.innerFilter.parts) {
				if (part.planToRemove) {
					partsToRemove.Add(part);
				} else {
					part.DoEditInterface(filterListing);
				}
			}
			foreach (PawnFilterPart part in partsToRemove) {
				this.innerFilter.parts.Remove(part);
			}
			filterListing.End();
			this.filterHeight = filterListing.CurHeight + LogicGate.filterHeightBuffer;
		}
	}
}
