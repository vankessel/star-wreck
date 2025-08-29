using System;
using Godot;

namespace StarWreck.scripts;

[GlobalClass]
public partial class Gun : Node2D
{
    [Export] public float SecondsPerBullet { get; private set; } = 4f;
    [Export] private float _bulletSpeed = 200f;
    [Export] private float _bulletDamage = 10f;
    [Export] private PackedScene _bullet;

    private ulong _nextShotTime;

    public bool CanShoot { get; private set; }

    [Signal]
    public delegate void CooledDownEventHandler();

    public override void _EnterTree()
    {
        base._EnterTree();

        _nextShotTime = (ulong)(new Random().NextSingle() * 1000f * SecondsPerBullet);
        CooledDown += OnCooledDown;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        CooledDown -= OnCooledDown;
    }

    private void OnCooledDown() => CanShoot = true;

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (CanShoot || Time.GetTicksMsec() <= _nextShotTime) return;

        EmitSignal(SignalName.CooledDown);
    }

    public void Shoot(Vector2 direction)
    {
        if (!CanShoot) return;
        CanShoot = false;
        _nextShotTime = Time.GetTicksMsec() + (ulong)(1000f * SecondsPerBullet);

        Bullet bullet = _bullet.Instantiate<Bullet>();
        bullet.Init(GlobalPosition, direction.Normalized() * _bulletSpeed, _bulletDamage);
        GetTree().Root.AddChild(bullet);
    }

    public void ShootAt(Node2D target)
    {
        Shoot(target.GlobalPosition - GlobalPosition);
    }
}
