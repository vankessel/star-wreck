using Godot;

namespace StarWreck.scripts.enemy;

[GlobalClass]
public partial class BaseMotor : Node2D
{
    public virtual Vector2 GetMotorForce(RigidBody2D rigidBody2D)
    {
        return Vector2.Zero;
    }

    public virtual float GetMotorTorque(RigidBody2D rigidBody2D)
    {
        return 0f;
    }
}
