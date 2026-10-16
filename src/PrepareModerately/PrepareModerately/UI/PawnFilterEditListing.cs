using System;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;

using RimWorld;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `Verse.Listing_ScenEdit`.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
	internal sealed class PawnFilterEditListing : Listing_Standard {
#else
	internal sealed class PawnFilterEditListing(PawnFilter filter) : Listing_Standard {
#endif
		private const float GapSize = 4;

		private const float LabelGapSize = 4;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly Color s_partRectBgColor = new Color(1, 1, 1, 0.08f);
#else
		private static readonly Color s_partRectBgColor = new(1, 1, 1, 0.08f);
#endif

		private const float WidgetRowMaxWidth = 72;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private readonly PawnFilter _filter;
#else
		private readonly PawnFilter _filter = filter;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		internal PawnFilterEditListing(PawnFilter filter) =>
			this._filter = filter;
#endif

		internal Rect GetPawnFilterPartRect(PawnFilterPart part, float height = 0) =>
			this.GetPawnFilterPartRect(part, height, out _, out _);

		internal Rect GetPawnFilterPartRect(PawnFilterPart part, float height, out float totalAddedListHeight) =>
			this.GetPawnFilterPartRect(part, height, out totalAddedListHeight, out _);

		internal Rect GetPawnFilterPartRect(PawnFilterPart part, float height, out float totalAddedListHeight, out Rect headerRemainderRect) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (part is null) {
				throw new ArgumentNullException(nameof(part));
			}
#else
			ArgumentNullException.ThrowIfNull(part);
#endif

			float headerHeight = Text.LineHeight;
			Rect rect = this.GetRect(headerHeight + height);
			Widgets.DrawBoxSolid(rect, s_partRectBgColor);
			Rect headerRect = new Rect(rect.x, rect.y, rect.width, headerHeight).Rounded();
			Rect widgetRect = new Rect(headerRect.x, headerRect.y, WidgetRowMaxWidth, headerRect.height).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			WidgetRow widgetRow = new WidgetRow(widgetRect.x, widgetRect.y, UIDirection.RightThenDown, widgetRect.width, 0);
#else
			WidgetRow widgetRow = new(widgetRect.x, widgetRect.y, UIDirection.RightThenDown, widgetRect.width, 0);
#endif

			if (part.Def.PlayerAddRemovable && widgetRow.ButtonIcon(Textures.s_delete, null, GenUI.SubtleMouseoverColor)) {
				this._filter.RemovePart(part);
				SoundDefOf.Click.PlayOneShotOnCamera();
			}

			if (this._filter.CanReorder(part, ReorderDirection.Up) && widgetRow.ButtonIcon(Textures.s_reorderUp)) {
				this._filter.Reorder(part, ReorderDirection.Up);
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}

			if (this._filter.CanReorder(part, ReorderDirection.Down) && widgetRow.ButtonIcon(Textures.s_reorderDown)) {
				this._filter.Reorder(part, ReorderDirection.Down);
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}

			string label = part.Label.CapitalizeFirst();
			Rect labelRect = new Rect(widgetRect.xMax, headerRect.y, Text.CalcSize(label).x, headerRect.height).Rounded();
			Widgets.Label(labelRect, label);

			this.Gap(GapSize);

			totalAddedListHeight = rect.height + GapSize;
			headerRemainderRect = new Rect(labelRect.xMax + LabelGapSize, headerRect.y, headerRect.width - widgetRect.width - labelRect.width, headerRect.height).Rounded();

			return new Rect(rect.x, headerRect.yMax, rect.width, rect.height - headerHeight).Rounded();
		}
	}
}
