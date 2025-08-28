using Godot;

namespace StarWreck.scripts;

public partial class TrackedRigidBody2D : RigidBody2D
{
    protected Vector2 VelocityChange { get; private set; }
    private Vector2 _lastVelocity;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        VelocityChange = LinearVelocity - _lastVelocity;
        _lastVelocity = LinearVelocity;
    }
}
