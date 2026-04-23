using Content.Server._Starlight;
using Content.Shared._HL.Traits.Physical;
using Content.Shared._Starlight;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.Movement.Systems;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._HL.Traits.Physical;

/// <summary>
/// Handles light sensitivity burn damage and movement penalty for non-shadekin entities.
/// Shadekin entities are handled by ShadekinSystem instead.
/// </summary>
public sealed class LightSensitivitySystem : EntitySystem
{
    private static readonly ProtoId<AlertPrototype> LightExposureAlert = "Shadekin";

    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly ILogManager _logManager = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _speed = default!;
    [Dependency] private readonly ShadekinSystem _shadekin = default!;
    private ISawmill _sawmill = default!;

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = _logManager.GetSawmill("shadekin.debug");
        SubscribeLocalEvent<LightSensitivityComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshSpeedModifiers);
    }

    private void OnRefreshSpeedModifiers(EntityUid uid, LightSensitivityComponent comp, RefreshMovementSpeedModifiersEvent args)
    {
        if (HasComp<ShadekinComponent>(uid))
            return; // ShadekinSystem handles shadekins

        if (comp.CurrentLightExposure < comp.SlowdownThreshold)
            return;

        args.ModifySpeed(comp.SpeedMultiplier, comp.SpeedMultiplier);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;
        var query = EntityQueryEnumerator<LightSensitivityComponent, DamageableComponent>();

        while (query.MoveNext(out var uid, out var comp, out var _))
        {
            if (HasComp<ShadekinComponent>(uid))
                continue; // ShadekinSystem handles shadekins

            if (curTime < comp.NextUpdate)
                continue;

            comp.NextUpdate = curTime + comp.UpdateCooldown;

            comp.CurrentLightExposure = _shadekin.GetLightExposure(uid);

            ApplyBurnDamage(uid, comp);
            _speed.RefreshMovementSpeedModifiers(uid);
            UpdateLightAlert(uid, comp.CurrentLightExposure);
        }
    }

    private void UpdateLightAlert(EntityUid uid, float rawExposure)
    {
        short level;
        if (rawExposure >= 15f) level = 4;
        else if (rawExposure >= 10f) level = 3;
        else if (rawExposure >= 5f) level = 2;
        else if (rawExposure >= 0.8f) level = 1;
        else level = 0;

        _alerts.ShowAlert(uid, LightExposureAlert, level);
    }

    private void ApplyBurnDamage(EntityUid uid, LightSensitivityComponent comp)
    {
        if (comp.CurrentLightExposure < comp.BurnThreshold)
            return;

        var multiplier = (int) comp.CurrentLightExposure - comp.BurnThreshold + 1;
        var damage = new DamageSpecifier();
        damage.DamageDict.Add("Heat", multiplier);
        _damageable.TryChangeDamage(uid, damage, true, false);
        _sawmill.Error($"[LightSensitivity.Burn] {ToPrettyString(uid)} exposure={comp.CurrentLightExposure} clause=LightSensitivityComponent(burnThreshold={comp.BurnThreshold}, slowdownThreshold={comp.SlowdownThreshold}, speedMult={comp.SpeedMultiplier}) Heat+{multiplier}");
    }
}
