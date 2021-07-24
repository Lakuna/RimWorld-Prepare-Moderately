using PrepareModerately.PawnFilter;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace PrepareModerately.UI {
	public class Page_PrepareModerately : Page {
		private const float controlColumnWidthPercentage = 0.2f;
		private const float scrollBuffer = 100;

		private float partViewHeight;
		private Vector2 scrollPosition;

		public Page_PrepareModerately() {
			this.partViewHeight = 0;
			this.scrollPosition = Vector2.zero;
		}

		public override string PageTitle => "Prepare Moderately";

		public override void PreOpen() {
			base.PreOpen();

			this.scrollPosition = Vector2.zero;
		}

		public override void DoWindowContents(Rect rect) {
			this.DrawPageTitle(rect);
			Rect mainRect = this.GetMainRect(rect);
			GUI.BeginGroup(mainRect);

			Rect controlRect = mainRect.LeftPart(Page_PrepareModerately.controlColumnWidthPercentage).Rounded();
			Listing_Standard controlList = new Listing_Standard { ColumnWidth = controlRect.width };
			controlList.Begin(controlRect);

			if (controlList.ButtonText("Close")) {
				this.Close();
				Find.WindowStack.Add(PrepareModerately.Instance.configureStartingPawnsPage);
			}

			if (controlList.ButtonText("Add part")) {
				FloatMenuUtility.MakeMenu(DefDatabase<PawnFilterPartDef>.AllDefsListForReading, (def) => def.label, (def) => () => PrepareModerately.Instance.activeFilter.CreatePart(def));
			}

			PrepareModerately.Instance.activeFilter.name = controlList.TextEntry(PrepareModerately.Instance.activeFilter.name);

			if (controlList.ButtonText("Save")) {
				PrepareModerately.Instance.activeFilter.Save();
			}

			if (controlList.ButtonText("Load")) {
				try {
					Directory.CreateDirectory(PrepareModerately.dataPath);
					string[] filePaths = Directory.GetFiles(PrepareModerately.dataPath).Where((filePath) => filePath.EndsWith(PrepareModerately.filterExtension)).ToArray();
					if (filePaths.Length > 0) {
						FloatMenuUtility.MakeMenu(filePaths, (filePath) => {
							int start = filePath.LastIndexOf("\\") + 1;
							int end = filePath.LastIndexOf(PrepareModerately.filterExtension);
							return filePath.Substring(start, end - start);
						}, (filePath) => () => PrepareModerately.Instance.activeFilter.Load(filePath));
					} else {
						FloatMenuUtility.MakeMenu(new string[] { "N/A" }, (s) => s, (s) => () => { });
					}
				} catch (Exception e) {
					PrepareModerately.LogError(e);
				}
			}

			controlList.Label("If you would like to invert a filter, put it inside of a logic gate with its type set to \"NOT\".");

			controlList.End();

			Rect filterRect = mainRect.RightPart(1 - Page_PrepareModerately.controlColumnWidthPercentage).Rounded();
			Widgets.DrawMenuSection(filterRect);
			filterRect = filterRect.GetInnerRect();
			Rect filterViewRect = new Rect(0, 0, filterRect.width, this.partViewHeight);
			Widgets.BeginScrollView(filterRect, ref this.scrollPosition, filterViewRect);
			Rect filterViewInnerRect = new Rect(0, 0, filterViewRect.width, 0xFFFFF);

			Listing_PawnFilter filterListing = new Listing_PawnFilter() { ColumnWidth = filterViewInnerRect.width };
			filterListing.Begin(filterViewInnerRect);
			List<PawnFilterPart> partsToRemove = new List<PawnFilterPart>();
			foreach (PawnFilterPart part in PrepareModerately.Instance.activeFilter.parts) {
				if (part.planToRemove) {
					partsToRemove.Add(part);
				} else {
					part.DoEditInterface(filterListing);
				}
			}
			foreach (PawnFilterPart part in partsToRemove) {
				PrepareModerately.Instance.activeFilter.parts.Remove(part);
			}
			filterListing.End();
			this.partViewHeight = filterListing.CurHeight + Page_PrepareModerately.scrollBuffer;

			Widgets.EndScrollView();
			GUI.EndGroup();
		}
	}
}
