using Godot;

namespace StarWreck.scripts.xpbd;

public partial class Rope2D : ParticlePhysicsSystem2D
{
    [Export(PropertyHint.Range, "0,64,or_greater")]
    private float _length;
}
