using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_LogicGate : PawnFilterPart {
		[Serializable]
		public class SerializableLogicGate : SerializablePawnFilterPart {
			public int logicGateType;
			public PawnFilter.SerializablePawnFilter innerFilter;

			public SerializableLogicGate(PawnFilterPart_LogicGate pawnFilterPart) {
				this.logicGateType = (int) pawnFilterPart.logicGateType;
				this.innerFilter = new PawnFilter.SerializablePawnFilter(pawnFilterPart.innerFilter);
			}

			public override PawnFilterPart Deserialize() => new PawnFilterPart_LogicGate {
				logicGateType = (LogicGateType) this.logicGateType,
				innerFilter = this.innerFilter.Deserialize()
			};
		}

		public override SerializablePawnFilterPart Serialize() => new SerializableLogicGate(this);

		private enum LogicGateType { AND, OR, XOR, NOT };

		private const int filterFieldHeightParts = 8;
		private const int dividerWidth = 17;

		private PawnFilter innerFilter;
		private LogicGateType logicGateType;
		private float partViewHeight = 0;
		private Vector2 scrollPosition = Vector2.zero;

		public PawnFilterPart_LogicGate() {
			this.label = "Logic gate:";
			this.innerFilter = new PawnFilter();
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			int heightParts = 2 + filterFieldHeightParts;
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight * heightParts);

			// Logic gate type list.
			Rect gateTypeRect = new Rect(rect.x, rect.y, rect.width, rect.height / heightParts);
			if (Widgets.ButtonText(gateTypeRect, this.logicGateType.ToString())) { FloatMenuUtility.MakeMenu((LogicGateType[]) Enum.GetValues(typeof(LogicGateType)), type => type.ToString(), type => () => this.logicGateType = type); }

			// Add part button.
			Rect addPartButtonRect = new Rect(rect.x, rect.y + gateTypeRect.height, rect.width, rect.height / heightParts);
			if (Widgets.ButtonText(addPartButtonRect, "Add part")) {
				FloatMenuUtility.MakeMenu(PawnFilter.allFilterParts, def => def.label, def => () => {
					PawnFilterPart part = (PawnFilterPart) Activator.CreateInstance(def.partClass);
					part.def = def;
					this.innerFilter.parts.Add(part);
				});
			}

			// Build filter field.
			Rect filterFieldRect = new Rect(rect.x, rect.y + gateTypeRect.height + addPartButtonRect.height, rect.width, rect.height / heightParts * filterFieldHeightParts).Rounded();
			Widgets.DrawMenuSection(filterFieldRect);
			filterFieldRect = filterFieldRect.GetInnerRect();
			Rect filterViewRect = new Rect(0, 0, filterFieldRect.width - (dividerWidth - 1), this.partViewHeight);
			Widgets.BeginScrollView(filterFieldRect, ref this.scrollPosition, filterViewRect);
			Rect filterViewInnerRect = new Rect(0, 0, filterViewRect.width, 99999);

			// Draw filter parts.
			Listing_PawnFilter filterPartList = new Listing_PawnFilter(this.innerFilter) { ColumnWidth = filterViewInnerRect.width };
			filterPartList.Begin(filterViewInnerRect);
			_ = filterPartList.Label("Filters");
			List<PawnFilterPart> partsToRemove = new List<PawnFilterPart>(); // Remove parts that should be removed here in order to avoid modifying enumerable during foreach.
			foreach (PawnFilterPart part in this.innerFilter.parts) {
				if (part.toRemove) { partsToRemove.Add(part); } else { part.DoEditInterface(filterPartList); }
			}
			foreach (PawnFilterPart part in partsToRemove) { _ = this.innerFilter.parts.Remove(part); }
			filterPartList.End();
			this.partViewHeight = filterPartList.CurHeight + 100;

			// End filter field.
			Widgets.EndScrollView();
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
				case LogicGateType.NOT: // Acts like NAND.
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
