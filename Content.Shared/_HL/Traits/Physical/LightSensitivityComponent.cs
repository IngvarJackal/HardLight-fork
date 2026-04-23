namespace Content.Shared._HL.Traits.Physical;

/// <summary>
/// Modifies shadekin light-exposure burn and slowdown thresholds,
/// shifting them to lower exposure levels with increased severity.
/// Burn damage scales as (LightExposure - BurnThreshold + 1) per tick.
/// </summary>
[RegisterComponent]
public sealed partial class LightSensitivityComponent : Component
{
    /// <summary>
    /// Minimum LightExposure level at which burning starts. Replaces the default of 4.
    /// </summary>
    [DataField]
    public int BurnThreshold = 3;

    /// <summary>
    /// Minimum LightExposure level at which movement slowing starts. Replaces the default of 4.
    /// </summary>
    [DataField]
    public int SlowdownThreshold = 3;

    /// <summary>
    /// Speed multiplier applied to both walk and sprint when above SlowdownThreshold.
    /// </summary>
    [DataField]
    public float SpeedMultiplier = 0.8f;
}
