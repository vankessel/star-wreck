using Godot;

namespace StarWreck.scripts.enemy.spawning;

[GlobalClass]
public partial class EnemyBatch : Resource
{
    [Export]
    public PackedScene Enemy { get; private set; }

    [Export(PropertyHint.Range, "1,100,or_greater")]
    public int Count { get; private set; }
}
