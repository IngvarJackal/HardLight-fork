using Content.Server.Explosion.EntitySystems;
using Content.Shared._Starlight.NullSpace;

namespace Content.Server._Starlight.NullSpace;

public sealed class BluespacePulseOnTriggerSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BluespacePulseOnTriggerComponent, TriggerEvent>(OnTrigger);
    }

    private void OnTrigger(Entity<BluespacePulseOnTriggerComponent> ent, ref TriggerEvent args)
    {
        var pulse = new BluespacePulseActionEvent
        {
            Radius = ent.Comp.Radius,
            StunSeconds = ent.Comp.StunSeconds,
            Performer = ent.Owner,
            Source = ent.Owner,
        };

        RaiseLocalEvent(ent.Owner, pulse);
        args.Handled = true;
    }
}
