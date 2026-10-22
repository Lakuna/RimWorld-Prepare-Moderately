#if !(V1_0 || V1_1 || V1_2)
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasFavoriteColor : PawnFilterPart {
		private static IEnumerable<ColorDef> _legalColors =>
#if V1_3
			DefDatabase<ColorDef>.AllDefs ?? new List<ColorDef>();
#elif V1_4 || V1_5 || V1_6
			DefDatabase<ColorDef>.AllDefs?.Where((def) => def.colorType == ColorType.Ideo || def.colorType == ColorType.Misc) ?? new List<ColorDef>();
#else
			DefDatabase<ColorDef>.AllDefs?.Where((def) => def.colorType == ColorType.Ideo || def.colorType == ColorType.Misc) ?? [];
#endif

		public override IEnumerable<string> ConfigErrors {
			get {
				foreach (string item in base.ConfigErrors) {
					yield return item;
				}

				if (this._color is null) {
					yield return "Has favorite color filter part color is null.";
				}
			}
		}

		public override bool HasNullDefs =>
			base.HasNullDefs || this._color is null;

#if V1_3 || V1_4 || V1_5 || V1_6
		private ColorDef _color;
#else
		private ColorDef? _color;
#endif

		private static string ColorToHex(Color color) =>
			$"#{(int)(color.r * 0xff):X2}{(int)(color.g * 0xff):X2}{(int)(color.b * 0xff):X2}";

#if V1_3 || V1_4 || V1_5 || V1_6
		private static string LabelFor(ColorDef def) =>
			def?.color is null ? "PM.UnnamedColor".Translate().StringSafe()
			: def.label.NullOrEmpty() ? "PM.UnnamedColorHex".Translate(ColorToHex(def.color)).StringSafe()
			: def.label;
#else
		private static string LabelFor(ColorDef? def) =>
			def?.color is null ? "PM.UnnamedColor".Translate().StringSafe
			: def.label.NullOrEmpty() ? "PM.UnnamedColorHex".Translate(ColorToHex(def.color)).StringSafe
			: def.label;
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
			if (Widgets.ButtonText(rect, LabelFor(this._color))) {
				FloatMenuUtility.MakeMenu(
					_legalColors.OrderBy((def) => def.label),
					(def) => LabelFor(def),
					(def) => () => this._color = def);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this._color, "color");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ (this._color?.GetHashCode() ?? 0);

		public override bool Matches(Pawn pawn) =>
#if V1_3 || V1_4 || V1_5
			pawn.story?.favoriteColor == this._color.color;
#else
			pawn.story?.favoriteColor == this._color;
#endif

		public override void Randomize() =>
			this._color = _legalColors.RandomElement();

		public override string Summary(PawnFilter filter) =>
#if V1_3 || V1_4 || V1_5 || V1_6
			"PM.HasFavoriteColor".Translate(LabelFor(this._color)).StringSafe();
#else
			"PM.HasFavoriteColor".Translate(LabelFor(this._color)).StringSafe;
#endif
	}
}
#endif
