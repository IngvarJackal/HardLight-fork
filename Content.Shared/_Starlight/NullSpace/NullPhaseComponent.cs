namespace Content.Shared._Starlight.NullSpace;

[RegisterComponent]
public sealed partial class NullPhaseComponent : Component
{
    [DataField]
    public EntityUid? PhaseAction;

    /// <summary>
    /// Overrides the Phase Shift action's use delay in seconds. If null, the action prototype default is used.
    /// </summary>
    [DataField]
    public float? UseDelay;
}