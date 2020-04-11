using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class Listing_PawnFilter : Listing_Standard {
		private readonly PawnFilter filter;

		public Listing_PawnFilter(PawnFilter filter) => this.filter = filter;

		public Rect GetPawnFilterPartRect(PawnFilterPart part, float height) {
			// Make rect.
			Rect rect = this.GetRect(height);
			Widgets.DrawBoxSolid(rect, new Color(1, 1, 1, 0.08f));
			WidgetRow widgetRow = new WidgetRow(rect.x, rect.y, UIDirection.RightThenDown, 72, 0);

			// Add removal button.
			if (widgetRow.ButtonIcon(ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true), null, new Color?(GenUI.SubtleMouseoverColor))) { part.toRemove = true; }

			// Add label on left half.
			Text.Anchor = TextAnchor.UpperRight;
			Rect leftRect = rect.LeftHalf().Rounded();
			leftRect.xMax -= 4;
			Widgets.Label(leftRect, part.label);

			// Return right half for further modification.
			Text.Anchor = TextAnchor.UpperLeft;
			this.Gap(4);
			return rect.RightHalf().Rounded();
		}
	}
}
