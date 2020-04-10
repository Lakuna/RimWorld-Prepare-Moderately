using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class Listing_PawnFilter : Listing_Standard {
		private readonly PawnFilter filter;

		public Listing_PawnFilter(PawnFilter filter) => this.filter = filter;

		public Rect GetPawnFilterPartRect(PawnFilterPart part, float height) {
			Rect rect = this.GetRect(height);
			Widgets.DrawBoxSolid(rect, new Color(1, 1, 1, 0.08f));
			WidgetRow widgetRow = new WidgetRow(rect.x, rect.y, UIDirection.RightThenDown, 72, 0);
			if (widgetRow.ButtonIcon(ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true), null, new Color?(GenUI.SubtleMouseoverColor))) { _ = this.filter.parts.Remove(part); }
			Text.Anchor = TextAnchor.UpperRight;
			Rect leftRect = rect.LeftHalf().Rounded();
			leftRect.xMax -= 4;
			Widgets.Label(leftRect, part.label);
			Text.Anchor = TextAnchor.UpperLeft;
			this.Gap(4);
			return rect.RightHalf().Rounded();
		}
	}
}
