using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server._Starlight.NullSpace;

/// <summary>
/// Component that pulses to eject NullSpace entities within range.
/// When <see cref="Continuous"/> is true the system polls every <see cref="Cooldown"/> seconds
/// (used by BluespaceFlasher). When false the pulse fires once on a TriggerEvent
/// (used by BluespacecryStal items so crafted objects are unaffected).
/// </summary>
[RegisterComponent, AutoGenerateComponentPause]
public sealed partial class BluespacePulseOnTriggerComponent : Component
{
    [DataField]
    public float Radius = 10f;

    [DataField]
    public float StunSeconds = 4f;

    [DataField]
    public TimeSpan Cooldown = TimeSpan.FromSeconds(5);

    /// <summary>
    /// When true the system actively polls for NullSpace entities each frame.
    /// Set false for items that should only pulse when manually triggered.
    /// </summary>
    [DataField]
    public bool Continuous = false;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan NextTrigger;
}
