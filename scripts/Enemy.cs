using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.NativeInterop;
using StarWreck.scripts.health;

namespace StarWreck.scripts;

public partial class Enemy : RigidBody2D
{
    [Export] private HealthComponent _healthComponent;
    [Export] private PackedScene _enemyDebris;

    private readonly Queue<Vector2> _velocityHistory = new([Vector2.Zero, Vector2.Zero, Vector2.Zero]);
    // private readonly Queue<Vector2> _calculatedAccelerations = new([Vector2.Zero, Vector2.Zero, Vector2.Zero]);
    private Vector2 _calculatedVelocityChange;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Vector2 vm3 = _velocityHistory.Dequeue();
        Vector2 vm2 = _velocityHistory.Dequeue();
        Vector2 vm1 = _velocityHistory.Dequeue();
        _calculatedVelocityChange = ((-1f / 3f) * vm3 + 1.5f * vm2 - 3f * vm1 + (11f / 6f) * LinearVelocity);
        _velocityHistory.Enqueue(vm2);
        _velocityHistory.Enqueue(vm1);
        _velocityHistory.Enqueue(LinearVelocity);
        TakeKineticDamage();
    }

    private void TakeKineticDamage()
    {
        float impulse = (_calculatedVelocityChange).Length() * Mass;
        float damage = impulse / 200f;
        if (damage > 2f)
        {
            _healthComponent.Hurt(damage);
        }

        if (0f < _healthComponent.Health) return;

        CallDeferred(MethodName.SpawnDebris, Position, LinearVelocity, GetParent(), 3, 5);

        QueueFree();
    }

    private void SpawnDebris(Vector2 position, Vector2 velocity, Node parent, int min = 3, int max = 5)
    {
        RandomNumberGenerator r = new();
        const float velocitySd = 0.1f;
        const float positionSd = 3f;
        const float radiansSd = 10f * Mathf.Pi / 180f;
        int count = r.RandiRange(min, max);
        for (int i = 0; i < count; i++)
        {
            RigidBody2D debris = _enemyDebris.Instantiate<RigidBody2D>();
            parent.AddChild(debris);
            Vector2 positionDelta = Vector2.Right.Rotated(r.Randfn() * Mathf.Pi) * r.Randfn(0f, positionSd);
            debris.Position = position + positionDelta;
            float speedMultiplier = Mathf.Max(0f, 1f + r.Randfn(0f, velocitySd));
            Vector2 newVelocity = speedMultiplier * velocity.Rotated(r.Randfn(0f, radiansSd));
            float angleTo = debris.LinearVelocity.AngleTo(positionDelta);
            debris.LinearVelocity = newVelocity.Rotated(0.5f * angleTo);
        }
    }
}
