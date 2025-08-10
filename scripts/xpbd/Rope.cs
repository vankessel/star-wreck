using Godot;

namespace StarWreck.scripts.xpbd;

public partial class Rope : Node3D
{
    [Export(PropertyHint.Range, "1,64,or_greater")]
    private int _particleCount;
}