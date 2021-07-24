using PrepareModerately.PawnFilter;
using UnityEngine;
using Verse;

namespace PrepareModerately.UI {
	public class Listing_PawnFilter : Listing_Standard {
		// Must get texture after initialization in order to be on the proper thread.
		private Texture2D deleteButtonTextureCache;
		private Texture2D DeleteButtonTexture {
			get {
				if (!this.deleteButtonTextureCache) {
					this.deleteButtonTextureCache = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");
				}

				return this.deleteButtonTextureCache;
			}
		}

		public const int GapSize = 8;

		public Rect GetPawnFilterPartRect(PawnFilterPart pawnFilterPart, float height) {
			Rect rect = this.GetRect(Text.LineHeight + height);
			Widgets.DrawBoxSolid(rect, new Color(1, 1, 1, 0.08f));
			WidgetRow widgetRow = new WidgetRow(rect.x, rect.y, UIDirection.RightThenDown, 72, 0);

			if (widgetRow.ButtonIcon(this.DeleteButtonTexture, null, new Color?(GenUI.SubtleMouseoverColor))) {
				pawnFilterPart.Remove();
			}

			Rect labelRect = new Rect(rect.x + this.DeleteButtonTexture.width, rect.y, rect.width - this.DeleteButtonTexture.width, Text.LineHeight);
			Widgets.Label(labelRect, pawnFilterPart.Label);

			this.Gap(Listing_PawnFilter.GapSize);

			return new Rect(rect.x, rect.y + labelRect.height, rect.width, rect.height - labelRect.height);
		}
	}
}
