using Robust.Shared.GameStates;

namespace Content.Shared._HL.Factions.ManawaRite.Clothing.Rings;

public enum TransmogrificationEffect : byte
{
    None,
    AnomalyFire,
    AnomalyShadow,
    AnomalyFlora,
    AnomalyFrost,
    AnomalyBluespace,
    Jitter,
    Halo,
    RunicBelt,
    DraconicWings,
    CyanFireflies,
    WatchingEyes,
}

[RegisterComponent]
public sealed partial class RingOfTransmogrificationComponent : Component
{
    [DataField]
    public TransmogrificationEffect SelectedEffect = TransmogrificationEffect.None;

    public EntityUid? Wearer;
    public bool AddedJitter;
}

/// <summary>
/// Marker placed on the wearer so the client can render the cosmetic overlay.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class RingGlowEffectComponent : Component
{
    [DataField, AutoNetworkedField]
    public TransmogrificationEffect Effect = TransmogrificationEffect.None;
}
