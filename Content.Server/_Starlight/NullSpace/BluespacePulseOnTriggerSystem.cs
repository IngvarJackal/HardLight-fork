using Content.Server.Explosion.EntitySystems;
using Content.Server.Stack;
using Content.Shared._Starlight.NullSpace;
using Content.Shared._Starlight;
using Content.Shared.Stacks;
using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;

namespace Content.Server._Starlight.NullSpace;

/// <summary>
/// Handles BluespaceFlasher (continuous polling) and BluespacecryStal items (single trigger-event).
/// NullSpace entities cancel all physics contacts so TriggerOnProximity can't detect them;
/// continuous entities use an Update loop with EntityLookupSystem instead.
/// </summary>
public sealed class BluespacePulseOnTriggerSystem : EntitySystem
{
    private float _updateAccumulator;
    private const float UpdateInterval = 2f;

    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly TriggerSystem _trigger = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly StackSystem _stack = default!;

    private static readonly SoundPathSpecifier NullSpaceCutoffSound = new("/Audio/_HL/Effects/ma cutoff.ogg");

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BluespacePulseOnTriggerComponent, TriggerEvent>(OnTrigger);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _updateAccumulator += frameTime;
        if (_updateAccumulator < UpdateInterval)
            return;
        _updateAccumulator -= UpdateInterval;

        var curTime = _timing.CurTime;
        var query = EntityQueryEnumerator<BluespacePulseOnTriggerComponent, TransformComponent>();

        while (query.MoveNext(out var uid, out var comp, out _))
        {
            if (!comp.Continuous)
                continue;

            if (curTime < comp.NextTrigger)
                continue;

            var found = new List<EntityUid>();
            foreach (var ent in _lookup.GetEntitiesInRange(uid, comp.Radius))
            {
                if (HasComp<NullSpaceComponent>(ent))
                    found.Add(ent);
            }

            if (found.Count == 0)
                continue;

            comp.NextTrigger = curTime + comp.Cooldown;
            _trigger.Trigger(uid);
            PulseEntities(found, comp.StunSeconds);
        }
    }

    private void OnTrigger(EntityUid uid, BluespacePulseOnTriggerComponent comp, TriggerEvent args)
    {
        if (comp.Continuous)
            return; // handled by Update loop

        // Consume one crystal from the stack; if it's depleted the entity gets deleted.
        if (TryComp<StackComponent>(uid, out var stackComp))
            _stack.Use(uid, 1, stackComp);

        var found = new List<EntityUid>();
        foreach (var ent in _lookup.GetEntitiesInRange(uid, comp.Radius))
        {
            if (HasComp<NullSpaceComponent>(ent))
                found.Add(ent);
        }

        PulseEntities(found, comp.StunSeconds);
    }

    private void PulseEntities(List<EntityUid> entities, float stunSeconds)
    {
        var stunTime = TimeSpan.FromSeconds(stunSeconds);
        foreach (var ent in entities)
        {
            if (HasComp<ShadekinComponent>(ent))
                _audio.PlayPvs(NullSpaceCutoffSound, ent);
            RemComp<NullSpaceComponent>(ent);
            _stun.TryParalyze(ent, stunTime, true);
        }
    }
}
