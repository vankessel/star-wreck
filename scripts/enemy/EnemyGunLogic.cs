using Godot;

namespace StarWreck.scripts.enemy;

public partial class EnemyGunLogic : Node
{
    [Export] private shooting.Gun _gun;

    private player.Player _player;


    public override void _Ready()
    {
        base._Ready();

        _player = player.Player.Instance;
        _gun.CooledDown += ShootAtPlayer;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _gun.CooledDown -= ShootAtPlayer;
    }

    private void ShootAtPlayer()
    {
        _gun.ShootAt(_player);
    }
}
