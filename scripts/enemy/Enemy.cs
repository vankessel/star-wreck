using Godot;
using StarWreck.scripts.health;

namespace StarWreck.scripts.enemy;

public partial class Enemy : TrackedRigidBody2D, IBreakable
{
    [Export] private HealthComponent _healthComponent;
    [Export] private BaseMotor _motor;
    [Export] private PackedScene[] _enemyDebris;
    [Export] private Sprite2D _sprite;

    public HealthComponent HealthComponent => _healthComponent;

    private Vector2 _otherVelocityChangeFraction = Vector2.Zero;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Vector2 otherVelocityChangeFraction = _otherVelocityChangeFraction;
        _otherVelocityChangeFraction = Vector2.Zero;
        KineticDamageCheck(otherVelocityChangeFraction);

        ApplyCentralForce(_motor.GetMotorForce(this));
        ApplyTorque(_motor.GetMotorTorque(this));
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

        SpawnDebris(Position, LinearVelocity + addedDebrisVelocity, GetParent());

        QueueFree();
    }

    private void SpawnDebris(Vector2 position, Vector2 velocity, Node parent)
    {
        RandomNumberGenerator r = new();
        const float velocitySd = 0.1f;
        const float positionSd = 3f;
        const float radiansSd = 10f * Mathf.Pi / 180f;
        foreach (PackedScene debrisScene in _enemyDebris)
        {
            Debris debris = debrisScene.Instantiate<Debris>();
            int childCount = debris.GetChildCount();
            for (int i = 0; i < childCount; i++)
            {
                Node2D child = debris.GetChildOrNull<Node2D>(i);
                if (child != null)
                {
                    child.GlobalScale = _sprite.GlobalScale;
                    child.Position *= _sprite.Scale;
                }
            }
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
