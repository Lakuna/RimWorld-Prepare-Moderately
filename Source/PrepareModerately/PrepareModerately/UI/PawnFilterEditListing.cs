using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;
using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.UI {
	public class PawnFilterEditListing : Listing_Standard {
		private readonly PawnFilter filter;

		public PawnFilterEditListing(PawnFilter filter) => this.filter = filter;

		private static readonly float HeaderHeight = Text.LineHeight;

		private static readonly Color PartRectBgColor = new Color(1, 1, 1, 0.08f);

		private const float WidgetRowMaxWidth = 72;

		private const float GapSize = 4;

		public Rect GetPawnFilterPartRect(PawnFilterPart part, float height) => this.GetPawnFilterPartRect(part, height, out _, out _);

		public Rect GetPawnFilterPartRect(PawnFilterPart part, float height, out float totalAddedListHeight) => this.GetPawnFilterPartRect(part, height, out totalAddedListHeight, out _);

		public Rect GetPawnFilterPartRect(PawnFilterPart part, float height, out float totalAddedListHeight, out Rect headerRemainderRect) {
			if (part == null) {
				throw new ArgumentNullException(nameof(part));
			}

			Rect rect = this.GetRect(HeaderHeight + height);
			Widgets.DrawBoxSolid(rect, PartRectBgColor);
			Rect headerRect = new Rect(rect.x, rect.y, rect.width, HeaderHeight);
			Rect widgetRect = new Rect(headerRect.x, headerRect.y, WidgetRowMaxWidth, headerRect.height);
			WidgetRow widgetRow = new WidgetRow(widgetRect.x, widgetRect.y, UIDirection.RightThenDown, WidgetRowMaxWidth, 0);

			if (part.Def.PlayerAddRemovable && widgetRow.ButtonIcon(Textures.DeleteX, null, GenUI.SubtleMouseoverColor)) {
				this.filter.RemovePart(part);
				SoundDefOf.Click.PlayOneShotOnCamera();
			}

			if (this.filter.CanReorder(part, ReorderDirection.Up) && widgetRow.ButtonIcon(Textures.ReorderUp)) {
				this.filter.Reorder(part, ReorderDirection.Up);
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}

			if (this.filter.CanReorder(part, ReorderDirection.Down) && widgetRow.ButtonIcon(Textures.ReorderDown)) {
				this.filter.Reorder(part, ReorderDirection.Down);
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}

			string label = part.Label.CapitalizeFirst();
			Rect labelRect = new Rect(widgetRect.xMax, headerRect.y, Text.CalcSize(label).x, headerRect.height);
			Widgets.Label(labelRect, label);

			headerRemainderRect = new Rect(labelRect.xMax, headerRect.y, headerRect.width - widgetRect.width - labelRect.width, headerRect.height);

			this.Gap(GapSize);

			totalAddedListHeight = rect.height + GapSize;
			return new Rect(rect.x, rect.y + HeaderHeight, rect.width, rect.height - HeaderHeight).Rounded();
		}
	}
}
