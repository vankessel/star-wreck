using Godot;

namespace StarWreck.scripts;

public partial class Enemy : RigidBody2D
{
    [Export] private health.HealthComponent _healthComponent;
    [Export] private PackedScene _enemyDebris;

    public void TakeKineticDamage(RigidBody2D other)
    {
        float kineticEnergy = 0.5f * other.LinearVelocity.LengthSquared() * Mass;
        float damage = kineticEnergy / 10000f;
        float health = _healthComponent.Hurt(damage);

        if (0f < health) return;

        CallDeferred(MethodName.SpawnDebris, Position, other.LinearVelocity, GetParent(), 3, 5);

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
