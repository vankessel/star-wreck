using Godot;
using Godot.NativeInterop;

namespace StarWreck.scripts;

public partial class HealthComponent : Node
{
    [Export] private float _maxHealth = 100f;
    [Export] private float _initialHealth = 100f;

    private float _health = 100f;

    public float MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = SetMaxHealth(value);
    }

    public float Health
    {
        get => _health;
        set => _health = SetHealth(value);
    }

    [Signal]
    public delegate void HealthFullyRestoredEventHandler(HealthComponent healthComponent, float cappedHeal, float overheal);

    [Signal]
    public delegate void HealthFullyDepletedEventHandler(HealthComponent healthComponent, float cappedLoss, float overkill);

    [Signal]
    public delegate void HealthChangedEventHandler(HealthComponent healthComponent, float cappedChange, float excess);

    [Signal]
    public delegate void MaxHealthChangedEventHandler(HealthComponent healthComponent, float oldMaxHealth);

    public override void _EnterTree()
    {
        base._EnterTree();

        _health = _initialHealth;
    }

    public float SetMaxHealth(float maxHealth)
    {
        float oldMaxHealth = _maxHealth;

        if (maxHealth < _health)
        {
            SetHealth(maxHealth);
        }

        _maxHealth = maxHealth;

        EmitSignal(SignalName.MaxHealthChanged, oldMaxHealth);

        return _health;
    }

    public float SetHealth(float newHealth)
    {
        return ChangeHealth(newHealth - _health);
    }

    public float ChangeHealth(float healthDelta)
    {
        return healthDelta switch
        {
            < 0f => Hurt(-healthDelta),
            > 0f => Heal(healthDelta),
            _ => _health
        };
    }

    public float Hurt(float damage)
    {
        float oldHealth = _health;
        float rawNewHealth = _health - damage;
        _health = Mathf.Max(0f, rawNewHealth);
        float excess = Mathf.Min(0f, rawNewHealth);
        float healthDelta = _health - oldHealth;

        if (healthDelta < 0f)
        {
            EmitSignal(SignalName.HealthChanged, this, healthDelta, excess);
        }

        if (_health <= 0f)
        {
            EmitSignal(SignalName.HealthFullyDepleted, this, -healthDelta, -excess);
        }

        return _health;
    }

    public float Heal(float heal)
    {
        float initialHealth = _health;
        float rawNewHealth = _health + heal;
        _health = Mathf.Min(rawNewHealth, _maxHealth);
        float excess = Mathf.Max(rawNewHealth, _maxHealth) - _maxHealth;
        float healthDelta = _health - initialHealth;

        if (0 != healthDelta)
        {
            EmitSignal(SignalName.HealthChanged, this, healthDelta, excess);
        }

        if (_maxHealth <= _health)
        {
            EmitSignal(SignalName.HealthFullyRestored, this, healthDelta, excess);
        }

        return _health;
    }
}
