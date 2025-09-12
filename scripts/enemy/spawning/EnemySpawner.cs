using System.Collections.Generic;
using System.Linq;
using Godot;
using StarWreck.scripts.health;

namespace StarWreck.scripts.enemy.spawning;

public partial class EnemySpawner : Node2D
{
    [Export] private bool _startSpawningOnReady = false;
    [Export] private float _radius = 100f;
    [Export] private float _ejectionSpeed = 300f;

    [Export] private EnemyQueue[] _enemyQueues = new EnemyQueue[1];

    /// <summary>
    /// Emitted immediately after enemy instantiation.
    /// </summary>
    [Signal] public delegate void SpawningEventHandler(Enemy enemy);
    /// <summary>
    /// Emitted after enemy instantiation and initialization.
    /// </summary>
    [Signal] public delegate void SpawnedEventHandler(Enemy enemy);
    [Signal] public delegate void SpawningStartedEventHandler();
    [Signal] public delegate void SpawningFinishedEventHandler();
    /// <summary>
    /// Triggers when all spawned enemies are destroyed. Spawner may still be spawning more.
    /// </summary>
    [Signal] public delegate void SpawnedEnemiesDestroyedEventHandler();
    /// <summary>
    /// Triggers when all enemies in queue have spawned and are destroyed.
    /// </summary>
    [Signal] public delegate void AllEnemiesDestroyedEventHandler();

    public bool IsSpawning { get; private set; } = false;
    public bool IsFinished { get; private set; } = false;
    public bool AreAllEnemiesDestroyed { get; private set; } = false;

    private static readonly RandomNumberGenerator Rng = new();

    private PriorityQueue<PackedScene, float> _currentPriorityQueue = new();
    private EnemyQueue _currentEnemyQueue;
    private int _remainingInQueue = 0;
    private int _index = 0;
    private float _lastSpawnTime = float.NegativeInfinity;

    private bool _queueFinished = false;
    private readonly HashSet<Enemy> _spawnedEnemies = [];

    public int Count => _enemyQueues.Sum(queue => queue.Count);

    public override void _EnterTree()
    {
        base._EnterTree();
        SpawningStarted += OnSpawningStarted;
        SpawningFinished += OnSpawningFinished;
        SpawnedEnemiesDestroyed += OnSpawnedEnemiesDestroyed;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        SpawningStarted -= OnSpawningStarted;
        SpawningFinished -= OnSpawningFinished;
        SpawnedEnemiesDestroyed -= OnSpawnedEnemiesDestroyed;
    }

    public override void _Ready()
    {
        base._Ready();

        if (!_startSpawningOnReady) return;
        StartSpawning();
    }

    private void OnSpawningStarted()
    {
        IsSpawning = true;
    }

    private void OnSpawningFinished()
    {
        IsSpawning = false;
        IsFinished = true;
    }

    private void OnSpawnedEnemiesDestroyed()
    {
        if (!IsFinished) return;
        AreAllEnemiesDestroyed = true;
        EmitSignal(SignalName.AllEnemiesDestroyed);
    }

    public void StartSpawning() => EmitSignal(SignalName.SpawningStarted);

    private void FinishSpawning() => EmitSignal(SignalName.SpawningFinished);

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!IsSpawning) return;

        if (_currentPriorityQueue.Count == 0)
        {
            if (_index < _enemyQueues.Length)
            {
                _currentEnemyQueue = _enemyQueues[_index];
                _currentPriorityQueue = _enemyQueues[_index].GetQueue();
                _index++;
            }
            else
            {
                FinishSpawning();
            }
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
        Enemy enemy = enemyScene.Instantiate<Enemy>();
        EmitSignal(SignalName.Spawning);

        _spawnedEnemies.Add(enemy);

        enemy.HealthComponent.HealthFullyDepleted += EnemyOnHealthDepleted;

        float radians = Rng.RandfRange(0f, Mathf.Tau);
        float radius  = Rng.RandfRange(0f, _radius);
        Vector2 offsetDir = new(Mathf.Cos(radians), Mathf.Sin(radians));
        Vector2 offset = radius * offsetDir;
        enemy.GlobalPosition = GlobalPosition + offset;
        enemy.LinearVelocity += _ejectionSpeed * offsetDir;

        Window root = GetTree().GetRoot();
        root.AddChild(enemy);
        enemy.Owner = root;

        EmitSignal(SignalName.Spawned);

        return enemy;

        void EnemyOnHealthDepleted(HealthComponent _, float __, float ___)
        {
            _spawnedEnemies.Remove(enemy);
            if (_spawnedEnemies.Count == 0)
            {
                EmitSignal(SignalName.SpawnedEnemiesDestroyed);
            }
            enemy.HealthComponent.HealthFullyDepleted -= EnemyOnHealthDepleted;
        }
    }
}
