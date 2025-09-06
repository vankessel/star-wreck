using Godot;
using StarWreck.scripts.input;

namespace StarWreck.scripts;

public partial class PauseManager : Node
{
    public static PauseManager Instance;
    public static ulong UnpausedPhysicsTicks { get; private set; } = 0ul;
    public static float UnpausedSeconds => UnpausedPhysicsTicks / (float)Engine.PhysicsTicksPerSecond;

    public override void _EnterTree()
    {
        base._EnterTree();
        if (Instance != null) GD.PushError("More than one PauseManager!");
        Instance = this;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (!@event.IsActionPressed(Action.Pause)) return;
        GetTree().Paused = !GetTree().IsPaused();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (GetTree().IsPaused()) return;
        UnpausedPhysicsTicks += 1ul;
    }
}
