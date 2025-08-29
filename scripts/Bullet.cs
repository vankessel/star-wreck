using Godot;

namespace StarWreck.scripts;

public partial class Bullet : Area2D
{
    private Vector2 _velocity;
    public float Damage { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        BodyEntered += OnBodyEntered;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        BodyEntered -= OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is not IShootable shootable) return;
        shootable.GetShot(this);
        QueueFree();
    }

    public void Init(Vector2 position, Vector2 velocity, float damage)
    {
        GlobalPosition = position;
        _velocity = velocity;
        Damage = damage;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        GlobalPosition += _velocity * (float)delta;
    }
}
