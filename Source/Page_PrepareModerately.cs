﻿using System;
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
		public int randomizeMultiplier;
		private string randomizeMultiplierBuffer;
		public int randomizeModulus;
		private string randomizeModulusBuffer;

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

			// Add filter name input field.
			PrepareModerately.Instance.currentFilter.name = controlButtonList.TextEntry(PrepareModerately.Instance.currentFilter.name);

			// Add save filter button.
			if (controlButtonList.ButtonText("Save")) { PrepareModerately.Instance.currentFilter.Save(PrepareModerately.dataPath + "\\" + PrepareModerately.Instance.currentFilter.name + ".filter"); }

			// Add load filter button.
			if (controlButtonList.ButtonText("Load")) { PrepareModerately.Instance.currentFilter.Load(PrepareModerately.dataPath + "\\" + PrepareModerately.Instance.currentFilter.name + ".filter"); }

			// Randomize multiplier input field.
			controlButtonList.TextFieldNumericLabeled("Multiplier ", ref this.randomizeMultiplier, ref this.randomizeMultiplierBuffer);

			// Randomize modulus input field.
			controlButtonList.TextFieldNumericLabeled("Modulus ", ref this.randomizeModulus, ref this.randomizeModulusBuffer);

			// Multiplier and modulus help labels.
			controlButtonList.Label("Pawn randomization speed is multiplied by the multiplier and divided by the modulus.");
			if (this.randomizeMultiplier < 1 || this.randomizeModulus < 1) { _ = controlButtonList.Label("Multiplier and modulus values less than 1 will be set to 1."); }
			if (this.randomizeMultiplier > 5) { _ = controlButtonList.Label("Randomization speed will still be limited by your computer's hardware. Use high multiplier values at your own risk."); }
			if (this.randomizeModulus > 1) { _ = controlButtonList.Label("Higher modulus values will not make randomization easier on your computer."); }

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
