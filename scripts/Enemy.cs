using Godot;
using StarWreck.scripts.health;

namespace StarWreck.scripts;

public partial class Enemy : TrackedRigidBody2D, IBreakable
{
    [Export] private HealthComponent _healthComponent;
    [Export] private PackedScene _enemyDebris;

    public HealthComponent HealthComponent => _healthComponent;

    private Vector2 _otherVelocityChangeFraction = Vector2.Zero;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Vector2 otherVelocityChangeFraction = _otherVelocityChangeFraction;
        _otherVelocityChangeFraction = Vector2.Zero;
        KineticDamageCheck(otherVelocityChangeFraction);
    }

    public void Damage(Vector2 otherVelocityChangeFraction, TrackedRigidBody2D other)
    {
        _otherVelocityChangeFraction += otherVelocityChangeFraction;

        Vector2 hitVelocity = (GlobalPosition - other.GlobalPosition).Normalized() * otherVelocityChangeFraction.Length();

        // TODO: Increasing mass of ball will decrease change in velocity. Multiply by mass and adjust parameters.
        // TODO: Or pass plain fraction so calculations can be changed? Perhaps to fraction of energy lost?
        KineticDamageCheck(otherVelocityChangeFraction.Length(), hitVelocity);
    }

    private void KineticDamageCheck(Vector2 otherVelocityChange)
    {
        KineticDamageCheck((VelocityChange + otherVelocityChange).Length(), -otherVelocityChange);
    }

    private void KineticDamageCheck(float impulseMagnitude, Vector2 addedDebrisVelocity, float divisor = 100f)
    {
        float multiplier = 1f / (divisor * (1f + PhysicsMaterialOverride?.Bounce ?? 0f));
        float damage = impulseMagnitude * multiplier;
        if (damage > 2f) _healthComponent.Hurt(damage);

        if (0f < _healthComponent.Health) return;

        CallDeferred(MethodName.SpawnDebris, Position, LinearVelocity + addedDebrisVelocity, GetParent(), 3, 5);

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
