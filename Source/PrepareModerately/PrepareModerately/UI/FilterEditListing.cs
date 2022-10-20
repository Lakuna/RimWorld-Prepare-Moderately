﻿using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;
using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.UI {
	public class FilterEditListing : Listing_Standard {
		private readonly PawnFilter filter;

		public FilterEditListing(PawnFilter filter) => this.filter = filter;

		private static readonly float labelHeight = new float[] { deleteXTexture.height, reorderUpTexture.height, reorderDownTexture.height, Text.LineHeight }.Max();

		private static readonly Color partRectBgColor = new Color(1, 1, 1, 0.08f);

		private const float widgetRowMaxWidth = 72;

		private static readonly Texture2D deleteXTexture = ContentFinder<Texture2D>.Get("UI/Widgets/Delete"); // TODO: TexButton.DeleteX

		private static readonly Texture2D reorderUpTexture = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderUp"); // TODO: TexButton.ReorderUp

		private static readonly Texture2D reorderDownTexture = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderDown"); // TODO: TexButton.ReorderDown

		private const float gap = 4;

		public Rect GetPawnFilterPartRect(PawnFilterPart part, float height) {
			if (part == null) {
				throw new ArgumentNullException(nameof(part));
			}

			Rect rect = this.GetRect(labelHeight + height);
			Widgets.DrawBoxSolid(rect, partRectBgColor);
			WidgetRow widgetRow = new WidgetRow(rect.x, rect.y, UIDirection.RightThenDown, widgetRowMaxWidth, 0);

			if (part.Def.PlayerAddRemovable && widgetRow.ButtonIcon(deleteXTexture, null, GenUI.SubtleMouseoverColor)) {
				this.filter.RemovePart(part);
				SoundDefOf.Click.PlayOneShotOnCamera();
			}

			if (this.filter.CanReorder(part, ReorderDirection.Up) && widgetRow.ButtonIcon(reorderUpTexture)) {
				this.filter.Reorder(part, ReorderDirection.Up);
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}

			if (this.filter.CanReorder(part, ReorderDirection.Down) && widgetRow.ButtonIcon(reorderDownTexture)) {
				this.filter.Reorder(part, ReorderDirection.Down);
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}

			Rect labelRect = new Rect(rect.x + widgetRowMaxWidth, rect.y, rect.width - widgetRowMaxWidth, labelHeight);
			Text.Anchor = TextAnchor.UpperRight;
			Widgets.Label(labelRect, part.Label.CapitalizeFirst());
			Text.Anchor = TextAnchor.UpperLeft; // Text anchor must end on upper left.

			this.Gap(gap);

			return new Rect(rect.x, rect.y + labelRect.height, rect.width, rect.height - labelRect.height).Rounded();
		}
	}
}