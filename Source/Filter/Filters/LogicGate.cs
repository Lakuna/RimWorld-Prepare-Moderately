using PrepareModerately.GUI;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class LogicGate : PawnFilterPart {
		[Serializable]
		public class LogicGateSerializable : PawnFilterPartSerializable {
			public int logicGateType;
			public PawnFilter.PawnFilterSerializable innerFilter;

			private LogicGateSerializable() { } // Parameterless constructor necessary for serialization.

			public LogicGateSerializable(LogicGate pawnFilterPart) {
				this.logicGateType = (int) pawnFilterPart.logicGateType;
				this.innerFilter = new PawnFilter.PawnFilterSerializable(pawnFilterPart.innerFilter);
			}

			public override PawnFilterPart Deserialize() => new LogicGate {
				logicGateType = (LogicGateType) this.logicGateType,
				innerFilter = this.innerFilter.Deserialize()
			};
		}

		public override PawnFilterPartSerializable Serialize() => new LogicGateSerializable(this);

		private enum LogicGateType { AND, OR, XOR, NOT };

		private const float filterPartListHeightBuffer = 40; // Don't touch.

		private PawnFilter innerFilter;
		private LogicGateType logicGateType;
		private float filterPartListHeight;

		public LogicGate() {
			this.label = "Logic gate:";
			this.innerFilter = new PawnFilter();
			this.logicGateType = LogicGateType.AND;
			this.filterPartListHeight = 0;
		}

		public override float DoEditInterface(PawnFilterListing list) {
			float rectHeight = RowHeight * 2 + this.filterPartListHeight;
			Rect rect = list.GetPawnFilterPartRect(this, rectHeight);

			// Logic gate type list.
			Rect gateTypeRect = new Rect(rect.x, rect.y, rect.width, RowHeight);
			if (Widgets.ButtonText(gateTypeRect, this.logicGateType.ToString())) { FloatMenuUtility.MakeMenu((LogicGateType[]) Enum.GetValues(typeof(LogicGateType)), type => type.ToString(), type => () => this.logicGateType = type); }

			// Add part button.
			Rect addPartButtonRect = new Rect(rect.x, rect.y + gateTypeRect.height, rect.width, RowHeight);
			if (Widgets.ButtonText(addPartButtonRect, "Add part")) {
				FloatMenuUtility.MakeMenu(PawnFilter.allFilterParts, def => def.label, def => () => {
					PawnFilterPart part = (PawnFilterPart) Activator.CreateInstance(def.partClass);
					part.def = def;
					this.innerFilter.parts.Add(part);
				});
			}

			// Build filter field.
			Rect filterFieldRect = new Rect(rect.x, rect.y + gateTypeRect.height + addPartButtonRect.height, rect.width, this.filterPartListHeight).Rounded();
			Widgets.DrawMenuSection(filterFieldRect);
			filterFieldRect = filterFieldRect.GetInnerRect();

			// Draw filter parts.
			this.filterPartListHeight = filterPartListHeightBuffer;
			PawnFilterListing filterPartList = new PawnFilterListing() { ColumnWidth = filterFieldRect.width };
			filterPartList.Begin(filterFieldRect);
			List<PawnFilterPart> partsToRemove = new List<PawnFilterPart>(); // Remove parts that should be removed here in order to avoid modifying enumerable during foreach.
			foreach (PawnFilterPart part in this.innerFilter.parts) {
				if (part.toRemove) {
					partsToRemove.Add(part);
				} else {
					this.filterPartListHeight += part.DoEditInterface(filterPartList) + RowHeight + PawnFilterListing.gapSize;
				}
			}
			foreach (PawnFilterPart part in partsToRemove) { _ = this.innerFilter.parts.Remove(part); }
			filterPartList.End();

			return rectHeight;
		}

		public override bool Matches(Pawn pawn) {
			switch (this.logicGateType) {
				case LogicGateType.AND:
					foreach (PawnFilterPart filterPart in this.innerFilter.parts) {
						if (!filterPart.Matches(pawn)) { return false; }
					}
					return true;
				case LogicGateType.OR:
					foreach (PawnFilterPart filterPart in this.innerFilter.parts) {
						if (filterPart.Matches(pawn)) { return true; }
					}
					return false;
				case LogicGateType.XOR:
					bool alreadyMatched = false;
					foreach (PawnFilterPart filterPart in this.innerFilter.parts) {
						if (filterPart.Matches(pawn)) {
							if (alreadyMatched) {
								return false;
							}
							alreadyMatched = true;
						}
					}
					return alreadyMatched;
				case LogicGateType.NOT: // NAND.
					foreach (PawnFilterPart filterPart in this.innerFilter.parts) {
						if (filterPart.Matches(pawn)) { return false; }
					}
					return true;
				default:
					throw new Exception("Unknown logic gate type \"" + this.logicGateType.ToString() + "\"");
			}
		}
	}
}
