using Godot;

namespace StarWreck.scripts;

public interface IBreakable
{
    Vector2 GlobalPosition { get; }
    void Damage(Vector2 otherVelocityChangeFraction, TrackedRigidBody2D other);
}
