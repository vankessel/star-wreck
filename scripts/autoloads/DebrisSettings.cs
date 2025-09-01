using Godot;

namespace StarWreck.scripts.autoloads;

public partial class DebrisSettings : Node
{
    [Export(PropertyHint.Range, "1,64,or_greater")]
    public int MaxInstances { get; private set; } = 64;

    public static DebrisSettings Instance { get; private set; }

    public override void _EnterTree()
    {
        base._EnterTree();

        Instance = this;
    }
}
