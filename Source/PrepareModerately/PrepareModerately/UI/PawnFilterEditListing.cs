using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;
using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.UI {
	[StaticConstructorOnStartup]
	public class PawnFilterEditListing : Listing_Standard {
		private readonly PawnFilter filter;

		public PawnFilterEditListing(PawnFilter filter) => this.filter = filter;

		private static readonly float HeaderHeight = Text.LineHeight;

		private static readonly Color PartRectBgColor = new Color(1, 1, 1, 0.08f);

		private const float WidgetRowMaxWidth = 72;

		private static readonly Texture2D DeleteXTexture;

		private static readonly Texture2D ReorderUpTexture;

		private static readonly Texture2D ReorderDownTexture;

		private const float GapSize = 4;

#pragma warning disable CA1810 // Textures must be loaded from the main thread.
		static PawnFilterEditListing() {
#pragma warning restore CA1810
#if V1_0 || V1_1 || V1_2
			DeleteXTexture = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");
			ReorderUpTexture = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderUp");
			ReorderDownTexture = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderDown");
#else
			DeleteXTexture = TexButton.DeleteX;
			ReorderUpTexture = TexButton.ReorderUp;
			ReorderDownTexture = TexButton.ReorderDown;
#endif
		}

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

			if (part.Def.PlayerAddRemovable && widgetRow.ButtonIcon(DeleteXTexture, null, GenUI.SubtleMouseoverColor)) {
				this.filter.RemovePart(part);
				SoundDefOf.Click.PlayOneShotOnCamera();
			}

			if (this.filter.CanReorder(part, ReorderDirection.Up) && widgetRow.ButtonIcon(ReorderUpTexture)) {
				this.filter.Reorder(part, ReorderDirection.Up);
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}

			if (this.filter.CanReorder(part, ReorderDirection.Down) && widgetRow.ButtonIcon(ReorderDownTexture)) {
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
