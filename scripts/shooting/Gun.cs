using System;
using Godot;

namespace StarWreck.scripts.shooting;

[GlobalClass]
public partial class Gun : Node2D
{
    [Export] public float SecondsPerBullet { get; private set; } = 4f;
    [Export] private float _bulletSpeed = 200f;
    [Export] private float _bulletDamage = 10f;
    [Export] private PackedScene _bullet;
    [Export] private RigidBody2D _shooterRigidBody;

    private float _nextShotTime;

    public bool CanShoot { get; private set; }

    [Signal]
    public delegate void CooledDownEventHandler();

    public override void _EnterTree()
    {
        base._EnterTree();

        _nextShotTime = new Random().NextSingle() * SecondsPerBullet;
        CooledDown += OnCooledDown;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        CooledDown -= OnCooledDown;
    }

    private void OnCooledDown()
    {
        CanShoot = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (CanShoot || PauseManager.UnpausedSeconds <= _nextShotTime) return;

        EmitSignal(SignalName.CooledDown);
    }

    public void Shoot(Vector2 direction)
    {
        if (!CanShoot || !_shooterRigidBody.Visible) return;
        CanShoot = false;
        _nextShotTime = PauseManager.UnpausedSeconds + SecondsPerBullet;

        Bullet bullet = _bullet.Instantiate<Bullet>();
        Vector2 dir = direction.Normalized();
        float boost = dir.Dot(_shooterRigidBody.LinearVelocity);
        bullet.Init(GlobalPosition, dir * (_bulletSpeed + boost), _bulletDamage);
        GetTree().Root.AddChild(bullet);
    }

    public void ShootAt(Node2D target)
    {
        Shoot(target.GlobalPosition - GlobalPosition);
    }
}
