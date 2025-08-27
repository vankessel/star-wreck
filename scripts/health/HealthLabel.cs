using Godot;

namespace StarWreck.scripts.health;

public partial class HealthLabel : Label
{
    [Export] private HealthComponent _healthComponent;

    public override void _Ready()
    {
        base._Ready();
        UpdateLabel();
        _healthComponent.HealthChanged += OnHealthChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _healthComponent.HealthChanged -= OnHealthChanged;
    }

    public void UpdateLabel()
    {
        Text = $"{_healthComponent.Health:f2}";
    }

    private void OnHealthChanged(HealthComponent healthComponent, float cappedChange, float excess)
    {
        UpdateLabel();
    }
}
