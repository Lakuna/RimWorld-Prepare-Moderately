using System;
using UnityEngine;
using RimWorld;
using Verse;
using System.Collections.Generic;

namespace PrepareModerately {
	public class Page_PrepareModerately : Page {
		private const float controlColumnWidthPercentage = 0.35f;
		private const int dividerWidth = 17;
		private float partViewHeight = 0;
		private Vector2 scrollPosition = Vector2.zero;

		public override string PageTitle => "Prepare Moderately";

		public override void PreOpen() {
			base.PreOpen();
			this.scrollPosition = Vector2.zero;
		}

		public override void DoWindowContents(Rect rect) {
			this.DrawPageTitle(rect);
			Rect mainRect = this.GetMainRect(rect);
			GUI.BeginGroup(mainRect);

			// Build control column.
			Rect controlColumn = new Rect(0, 0, mainRect.width * controlColumnWidthPercentage, mainRect.height).Rounded();
			Listing_Standard controlButtonList = new Listing_Standard { ColumnWidth = 200 };
			controlButtonList.Begin(controlColumn);

			// Back button.
			if (controlButtonList.ButtonText("Close")) {
				this.Close();
				Find.WindowStack.Add(PrepareModerately.Instance.originalPage);
			}

			// Add part button.
			if (controlButtonList.ButtonText("Add part")) { this.OpenAddPartMenu(); }

			// End control column.
			controlButtonList.End();

			// Build filter column.
			Rect filterColumn = new Rect(controlColumn.xMax + dividerWidth, 0, mainRect.width - controlColumn.width - dividerWidth, mainRect.height).Rounded();
			Widgets.DrawMenuSection(filterColumn);
			filterColumn = filterColumn.GetInnerRect();
			Rect filterViewRect = new Rect(0, 0, filterColumn.width - (dividerWidth - 1), this.partViewHeight);
			Widgets.BeginScrollView(filterColumn, ref this.scrollPosition, filterViewRect);
			Rect filterViewInnerRect = new Rect(0, 0, filterViewRect.width, 99999);

			// Draw filter parts.
			Listing_PawnFilter filterPartList = new Listing_PawnFilter(PrepareModerately.Instance.currentFilter) { ColumnWidth = filterViewInnerRect.width };
			filterPartList.Begin(filterViewInnerRect);
			_ = filterPartList.Label("Filters");
			List<PawnFilterPart> partsToRemove = new List<PawnFilterPart>(); // Remove parts that should be removed here in order to avoid modifying enumerable during foreach.
			foreach (PawnFilterPart part in PrepareModerately.Instance.currentFilter.parts) {
				if (part.toRemove) { partsToRemove.Add(part); } else { part.DoEditInterface(filterPartList); }
			}
			foreach (PawnFilterPart part in partsToRemove) { _ = PrepareModerately.Instance.currentFilter.parts.Remove(part); }
			filterPartList.End();
			this.partViewHeight = filterPartList.CurHeight + 100;

			// End filter column.
			Widgets.EndScrollView();
			GUI.EndGroup();
		}

		private void OpenAddPartMenu() => FloatMenuUtility.MakeMenu(PawnFilter.allFilterParts, def => def.label, def => () => this.AddPawnFilterPart(def));

		private void AddPawnFilterPart(PawnFilterPartDef partDef) {
			PawnFilterPart part = (PawnFilterPart) Activator.CreateInstance(partDef.partClass);
			part.def = partDef;
			PrepareModerately.Instance.currentFilter.parts.Add(part);
		}
	}
}
