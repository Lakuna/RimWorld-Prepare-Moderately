using Lakuna.PrepareModerately.Filter.FilterPart;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.UI {
	public class FilterEditListing : Listing_Standard {
		private Filter.Filter filter;

		public FilterEditListing(Filter.Filter filter) {
			this.filter = filter;
		}

		public Rect GetFilterPartRect(FilterPart part, float height) {
			string label = part.Label;

			Rect rect = this.GetRect(height);

			Widgets.DrawBoxSolid(rect, new Color(1, 1, 1, 0.08f));

			WidgetRow widgetRow = new WidgetRow(rect.x, rect.y, UIDirection.RightThenDown, 72, 0);

			if (part.def.PlayerAddRemovable && widgetRow.ButtonIcon(TexButton.DeleteX, null, GenUI.SubtleMouseoverColor)) {
				this.filter.RemovePart(part);
				SoundDefOf.Click.PlayOneShotOnCamera();
			}

			if (this.filter.CanReorder(part, ReorderDirection.Up) && widgetRow.ButtonIcon(TexButton.ReorderUp)) {
				this.filter.Reorder(part, ReorderDirection.Up);
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}

			if (this.filter.CanReorder(part, ReorderDirection.Down) && widgetRow.ButtonIcon(TexButton.ReorderDown)) {
				this.filter.Reorder(part, ReorderDirection.Down);
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}

			Rect labelRect = rect.LeftPart(0.5f).Rounded();
			labelRect.xMax -= 4;

			Text.Anchor = TextAnchor.UpperRight;
			Widgets.Label(labelRect, label);
			Text.Anchor = TextAnchor.UpperLeft; // Text anchor must end on upper left.

			this.Gap(4);

			return rect.RightPart(0.5f).Rounded();
		}
	}
}
