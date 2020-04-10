using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.Noise;

namespace PrepareModerately {
	public class Page_PrepareModerately : Page {
		private const float controlColumnWidthPercentage = 0.35f;
		private const int dividerWidth = 17;
		private float partViewHeight = 0;
		private Vector2 scrollPosition = Vector2.zero;

		public override string PageTitle => "Prepare Moderately";

		public override void DoWindowContents(Rect rect) {
			this.DrawPageTitle(rect);
			Rect mainRect = this.GetMainRect(rect);
			GUI.BeginGroup(mainRect);

			// Build control column.
			Rect controlColumn = new Rect(0, 0, mainRect.width * controlColumnWidthPercentage, mainRect.height).Rounded();
			Listing_Standard controlButtonList = new Listing_Standard();
			controlButtonList.ColumnWidth = 200;
			controlButtonList.Begin(controlColumn);

			// Back button.
			if (controlButtonList.ButtonText("Close")) { _ = Find.WindowStack.TryRemove(this); }

			// Add part button.
			if (controlButtonList.ButtonText("Add part")) { this.OpenAddPartMenu(); }

			// Build filter column.
			Rect filterColumn = new Rect(controlColumn.xMax + dividerWidth, 0, mainRect.width - controlColumn.width - dividerWidth, mainRect.height).Rounded();
			Widgets.DrawMenuSection(filterColumn);
			filterColumn = filterColumn.GetInnerRect();

			// Draw filter parts.
			Rect filterViewRect = new Rect(0, 0, filterColumn.width - (dividerWidth - 1), this.partViewHeight);
			Widgets.BeginScrollView(filterColumn, ref this.scrollPosition, filterViewRect);
			Rect filterViewInnerRect = new Rect(0, 0, filterViewRect.width - (dividerWidth - 1), 99999);
			Listing_PawnFilter filterPartList = new Listing_PawnFilter(PrepareModerately.Instance.currentFilter);
			filterPartList.ColumnWidth = filterViewInnerRect.width;
			filterPartList.Begin(filterViewInnerRect);
			foreach (PawnFilterPart part in PrepareModerately.Instance.currentFilter.parts) { part.DoEditInterface(filterPartList); }
			filterPartList.End();
			this.partViewHeight = filterPartList.CurHeight + 100;
			Widgets.EndScrollView();
		}

		private void OpenAddPartMenu() => FloatMenuUtility.MakeMenu(PawnFilter.allFilterParts, partDef => partDef.name, partDef => () => this.AddPawnFilterPart(partDef));

		private void AddPawnFilterPart(PawnFilterPartDef partDef) {
			PawnFilterPart part = (PawnFilterPart) Activator.CreateInstance(partDef.partClass);
			part.def = partDef;
			PrepareModerately.Instance.currentFilter.parts.Add(part);
		}
	}
}
