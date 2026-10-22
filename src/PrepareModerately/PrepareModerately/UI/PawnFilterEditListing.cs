using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `Verse.Listing_ScenEdit`.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
	public sealed class PawnFilterEditListing : Listing_Standard {
#else
	public sealed class PawnFilterEditListing(PawnFilter filter) : Listing_Standard {
#endif
		private const float _gapSize = 4;

		private const float _labelGapSize = 4;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly Color _partRectBgColor = new Color(1, 1, 1, 0.08f);
#else
		private static readonly Color _partRectBgColor = new(1, 1, 1, 0.08f);
#endif

		private const float _widgetRowMaxWidth = 72;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private readonly PawnFilter _filter;
#else
		private readonly PawnFilter _filter = filter;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public PawnFilterEditListing(PawnFilter filter) {
			this._filter = filter;
		}
#endif

		public Rect GetPawnFilterPartRect(PawnFilterPart part, float height = 0) =>
			this.GetPawnFilterPartRect(part, height, out _, out _);

		public Rect GetPawnFilterPartRect(PawnFilterPart part, float height, out float totalAddedListHeight) =>
			this.GetPawnFilterPartRect(part, height, out totalAddedListHeight, out _);

		public Rect GetPawnFilterPartRect(PawnFilterPart part, float height, out float totalAddedListHeight, out Rect headerRemainderRect) {
			float headerHeight = Text.LineHeight;
			Rect rect = this.GetRect(headerHeight + height).Rounded();
			Widgets.DrawBoxSolid(rect, _partRectBgColor);
			Rect headerRect = new Rect(rect.x, rect.y, rect.width, headerHeight).Rounded();

			if (part is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to get a listing rectangle for a null part.");
				headerRemainderRect = headerRect;
			} else if (Textures.Delete is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw null delete texture.");
				headerRemainderRect = headerRect;
			} else if (Textures.ReorderUp is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw null reorder up texture.");
				headerRemainderRect = headerRect;
			} else if (Textures.ReorderDown is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw null reorder down texture.");
				headerRemainderRect = headerRect;
			} else {
				Rect widgetRect = new Rect(headerRect.x, headerRect.y, _widgetRowMaxWidth, headerRect.height).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				WidgetRow widgetRow = new WidgetRow(widgetRect.x, widgetRect.y, UIDirection.RightThenDown, widgetRect.width, 0);
#else
				WidgetRow widgetRow = new(widgetRect.x, widgetRect.y, UIDirection.RightThenDown, widgetRect.width, 0);
#endif

				if ((part.Def?.PlayerAddRemovable ?? false) && widgetRow.ButtonIcon(Textures.Delete, null, GenUI.SubtleMouseoverColor)) {
					SoundDefOf.Click?.PlayOneShotOnCamera();
					this._filter.RemovePart(part);
				}

				if (this._filter.CanReorder(part, ReorderDirection.Up) && widgetRow.ButtonIcon(Textures.ReorderUp)) {
					SoundDefOf.Tick_High?.PlayOneShotOnCamera();
					this._filter.Reorder(part, ReorderDirection.Up);
				}

				if (this._filter.CanReorder(part, ReorderDirection.Down) && widgetRow.ButtonIcon(Textures.ReorderDown)) {
					SoundDefOf.Tick_Low?.PlayOneShotOnCamera();
					this._filter.Reorder(part, ReorderDirection.Down);
				}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				string label = part.Label.CapitalizeFirstSafe();
#else
				string label = part.Label.CapitalizeFirstSafe;
#endif
				Rect labelRect = new Rect(widgetRect.xMax, headerRect.y, Text.CalcSize(label).x, headerRect.height).Rounded();
				Widgets.Label(labelRect, label);

				headerRemainderRect = new Rect(labelRect.xMax + _labelGapSize, headerRect.y, headerRect.width - widgetRect.width - labelRect.width, headerRect.height).Rounded();
			}

			this.Gap(_gapSize);

			totalAddedListHeight = rect.height + _gapSize;

			return new Rect(rect.x, headerRect.yMax, rect.width, rect.height - headerHeight).Rounded();
		}
	}
}
