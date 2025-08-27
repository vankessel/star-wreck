using Godot;

namespace StarWreck.scripts.health;

public partial class HealthBar : ProgressBar
{
    [Export] private HealthComponent _healthComponent;

    private float _inverseMaxHealthPercent;

    public override void _Ready()
    {
        base._Ready();
        MaxHealthChanged();
        _healthComponent.MaxHealthChanged += OnMaxHealthChanged;
        _healthComponent.HealthChanged += OnHealthChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _healthComponent.MaxHealthChanged -= OnMaxHealthChanged;
        _healthComponent.HealthChanged -= OnHealthChanged;
    }

    private void MaxHealthChanged()
    {
        _inverseMaxHealthPercent = 100f / _healthComponent.MaxHealth;
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        Value = _healthComponent.Health * _inverseMaxHealthPercent;
        Visible = Value < 100f - Mathf.Epsilon;
    }

    private void OnMaxHealthChanged(HealthComponent healthComponent, float oldMaxHealth)
    {
        MaxHealthChanged();
    }

    private void OnHealthChanged(HealthComponent healthComponent, float cappedChange, float excess)
    {
        UpdateProgress();
    }
}
