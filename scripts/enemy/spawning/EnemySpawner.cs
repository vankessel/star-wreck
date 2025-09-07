using System.Collections.Generic;
using Godot;

namespace StarWreck.scripts.enemy.spawning;

public partial class EnemySpawner : Node2D
{
    [Export] private float _radius = 100f;

    [Export] private EnemyQueue[] _enemyQueues = new EnemyQueue[1];

    private static readonly RandomNumberGenerator Rng = new();

    private PriorityQueue<PackedScene, float> _currentPriorityQueue = new();
    private EnemyQueue _currentEnemyQueue;
    private int _remainingInQueue = 0;
    private int _index = 0;
    private float _lastSpawnTime = 0f;

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_currentPriorityQueue.Count == 0)
        {
            if (_index >= _enemyQueues.Length) return;
            _currentEnemyQueue = _enemyQueues[_index];
            _currentPriorityQueue = _enemyQueues[_index].GetQueue();
            _index++;
        }
        else
        {
            float currentSeconds = PauseManager.UnpausedSeconds;
            if (currentSeconds <= 1f / _currentEnemyQueue.SpawnRate + _lastSpawnTime) return;
            _lastSpawnTime = currentSeconds;
            PackedScene enemyScene = _currentPriorityQueue.Dequeue();
            Spawn(enemyScene);
        }
    }

    private Enemy Spawn(PackedScene enemyScene)
    {
        float radians = Rng.RandfRange(0f, Mathf.Tau);
        float radius  = Rng.RandfRange(0f, _radius);

        float x = radius * Mathf.Cos(radians);
        float y = radius * Mathf.Sin(radians);

        Enemy enemy = enemyScene.Instantiate<Enemy>();
        enemy.GlobalPosition = GlobalPosition + new Vector2(x, y);
        enemy.CollisionMask &= ~(2u << 7);

        Window root = GetTree().GetRoot();
        root.AddChild(enemy);
        enemy.Owner = root;

        return enemy;
    }
}
