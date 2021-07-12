using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class Listing_PawnFilter : Listing_Standard {
		private readonly PawnFilter filter;

		public Listing_PawnFilter(PawnFilter filter) => this.filter = filter;

		public Rect GetPawnFilterPartRect(PawnFilterPart part, float height) {
			// Make rect.
			Rect rect = this.GetRect(PawnFilterPart.RowHeight + height);
			Widgets.DrawBoxSolid(rect, new Color(1, 1, 1, 0.08f));
			WidgetRow widgetRow = new WidgetRow(rect.x, rect.y, UIDirection.RightThenDown, 72, 0);

			// Add removal button.
			if (widgetRow.ButtonIcon(ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true), null, new Color?(GenUI.SubtleMouseoverColor))) { part.toRemove = true; }

			// Add label.
			Rect labelRect = new Rect(rect.x + 32, rect.y, rect.width, PawnFilterPart.RowHeight);
			Widgets.Label(labelRect, part.label);

			// Return remainder for further modification.
			this.Gap(4);
			return new Rect(rect.x, rect.y + labelRect.height, rect.width, rect.height - labelRect.height);
		}
	}
}
