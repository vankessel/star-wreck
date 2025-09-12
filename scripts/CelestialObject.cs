using Godot;
using StarWreck.scripts.enemy;
using StarWreck.scripts.enemy.spawning;

namespace StarWreck.scripts;

public partial class CelestialObject : AnimatableBody2D
{
    [Export] private EnemySpawner _enemySpawner;

    public EnemySpawner EnemySpawner => _enemySpawner;

    public override void _Ready()
    {
        base._Ready();
        _enemySpawner.Spawning += EnemySpawnerOnSpawning;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _enemySpawner.Spawning -= EnemySpawnerOnSpawning;
    }

    private static void EnemySpawnerOnSpawning(Enemy enemy)
    {
        enemy.BodyExited += EnemyOnBodyExited;
        enemy.CollisionMask &= ~(uint)PhysicsLayer.Planets;
        return;

        void EnemyOnBodyExited(Node body)
        {
            enemy.CollisionMask |= (uint)PhysicsLayer.Planets;
            enemy.BodyExited -= EnemyOnBodyExited;
        }
    }
}
