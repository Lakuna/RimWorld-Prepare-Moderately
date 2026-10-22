namespace Lakuna.PrepareModerately.Filter.Part {
	// Based on `RimWorld.ScenPartCategory`.
	public enum PawnFilterPartCategory : int {
		Undefined,
		Fixed,
		Normal,
		LogicGate, // Separate so that logic gates aren't added when randomizing a filter (chooses from `Normal` category only).
		Obsolete
	}
}
