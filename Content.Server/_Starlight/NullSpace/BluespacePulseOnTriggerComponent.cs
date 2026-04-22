namespace Content.Server._Starlight.NullSpace;

[RegisterComponent]
public sealed partial class BluespacePulseOnTriggerComponent : Component
{
    [DataField]
    public float Radius = 10f;

    [DataField]
    public float StunSeconds = 4f;
}
