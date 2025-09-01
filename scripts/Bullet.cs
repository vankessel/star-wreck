using Godot;

namespace StarWreck.scripts;

public partial class Bullet : Area2D
{
    public float Damage { get; private set; }
    public float Lifetime => (_despawnTime - _spawnTime) / 1000f;

    private ulong _spawnTime;
    private ulong _despawnTime;

    private Vector2 _velocity;

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

    public void Init(Vector2 position, Vector2 velocity, float damage, float lifetimeSeconds = 24f)
    {
        GlobalPosition = position;
        _velocity = velocity;
        Damage = damage;
        _spawnTime = Time.GetTicksMsec();
        _despawnTime = _spawnTime + (ulong)(lifetimeSeconds * 1000f);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        GlobalPosition += _velocity * (float)delta;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_despawnTime < Time.GetTicksMsec())
        {
            QueueFree();
        }
    }
}
