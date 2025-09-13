using Godot;
using StarWreck.scripts.enemy;
using StarWreck.scripts.enemy.spawning;
using StarWreck.scripts.shooting;

namespace StarWreck.scripts;

public partial class CelestialObject : AnimatableBody2D
{
    [Export] private EnemySpawner _enemySpawner;
    [Export] private CollisionShape2D _collisionShape;
    private CircleShape2D _circleShape2D;

    public EnemySpawner EnemySpawner => _enemySpawner;

    public override void _Ready()
    {
        base._Ready();
        _enemySpawner.Spawning += EnemySpawnerOnSpawning;
        Shape2D shape = _collisionShape.Shape;
        if (shape is CircleShape2D circleShape2D)
        {
            _circleShape2D = circleShape2D;
            _enemySpawner.radius = _circleShape2D.Radius * _collisionShape.GlobalScale.X;
        }
        else
        {
            GD.PushError($"{Name}: Shape2D needs to be a CircleShape2D!");
        }
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
