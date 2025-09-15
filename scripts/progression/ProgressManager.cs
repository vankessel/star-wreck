using System.Collections.Generic;
using Godot;
using StarWreck.scripts.autoloads;
using StarWreck.scripts.enemy;
using StarWreck.scripts.enemy.spawning;
using StarWreck.scripts.input;
using StarWreck.scripts.player;
using StarWreck.scripts.tools;

namespace StarWreck.scripts.progression;

public partial class ProgressManager : Node
{
	[Export(PropertyHint.File, "*.tscn")] private string _credits;

	[ExportGroup("Cutscenes")] [Export] private Cutscene _immediateCutscene;
	[Export] private Cutscene _uranusAndNeptuneCompletedCutscene;
	[Export] private Cutscene _jupiterAndSaturnCompletedCutscene;
	[Export] private Cutscene _earthAndMarsCompletedCutscene;
	[Export] private Cutscene _mercuryAndVenusCompletedCutscene;
	[Export] private Cutscene _sunCompletedCutscene;

	[ExportGroup("Celestial Objects")] [Export]
	private CelestialObject _neptune;

	[Export] private CelestialObject _uranus;
	[Export] private CelestialObject _saturn;
	[Export] private CelestialObject _jupiter;
	[Export] private CelestialObject _mars;
	[Export] private CelestialObject _earth;
	[Export] private CelestialObject _venus;
	[Export] private CelestialObject _mercury;
	[Export] private CelestialObject _sun;

	[ExportGroup("Force Fields")] [Export] private ForceField _uranusAndNeptuneForceField;
	[Export] private ForceField _jupiterAndSaturnForceField;
	[Export] private ForceField _earthAndMarsForceField;
	[Export] private ForceField _mercuryAndVenusForceField;
	[Export] private ForceField _sunForceField;

	[Signal]
	public delegate void UranusAndNeptuneCompletedEventHandler();

	[Signal]
	public delegate void JupiterAndSaturnCompletedEventHandler();

	[Signal]
	public delegate void EarthAndMarsCompletedEventHandler();

	[Signal]
	public delegate void MercuryAndVenusCompletedEventHandler();

	[Signal]
	public delegate void SunCompletedEventHandler();

	[Signal]
	public delegate void GameCompletedEventHandler();

	[Signal]
	private delegate void ExitTreeEventHandler();

	private readonly HashSet<EnemySpawner> _currentSpawners = [];

	public static ProgressManager Instance { get; private set; }

	public bool NeptuneFinished { get; private set; }
	public bool UranusFinished { get; private set; }
	public bool SaturnFinished { get; private set; }
	public bool JupiterFinished { get; private set; }
	public bool MarsFinished { get; private set; }
	public bool EarthFinished { get; private set; }
	public bool VenusFinished { get; private set; }
	public bool MercuryFinished { get; private set; }
	public bool SunFinished { get; private set; }

	// Planet event handlers
	public void OnNeptuneEnemiesDestroyed()
	{
		NeptuneFinished = true;
		if (UranusFinished) EmitSignal(SignalName.UranusAndNeptuneCompleted);
	}

	public void OnUranusEnemiesDestroyed()
	{
		UranusFinished = true;
		if (NeptuneFinished) EmitSignal(SignalName.UranusAndNeptuneCompleted);
	}

	public void OnSaturnEnemiesDestroyed()
	{
		SaturnFinished = true;
		if (JupiterFinished) EmitSignal(SignalName.JupiterAndSaturnCompleted);
	}

	public void OnJupiterEnemiesDestroyed()
	{
		JupiterFinished = true;
		if (SaturnFinished) EmitSignal(SignalName.JupiterAndSaturnCompleted);
	}

	public void OnMarsEnemiesDestroyed()
	{
		MarsFinished = true;
		if (EarthFinished) EmitSignal(SignalName.EarthAndMarsCompleted);
	}

	public void OnEarthEnemiesDestroyed()
	{
		EarthFinished = true;
		if (MarsFinished) EmitSignal(SignalName.EarthAndMarsCompleted);
	}

	public void OnVenusEnemiesDestroyed()
	{
		VenusFinished = true;
		if (MercuryFinished) EmitSignal(SignalName.MercuryAndVenusCompleted);
	}

	public void OnMercuryEnemiesDestroyed()
	{
		MercuryFinished = true;
		if (VenusFinished) EmitSignal(SignalName.MercuryAndVenusCompleted);
	}

	public void OnSunEnemiesDestroyed()
	{
		SunFinished = true;
		EmitSignal(SignalName.SunCompleted);
	}

	// Combined planet event handlers

	public void OnUranusAndNeptuneCompleted()
	{
		_uranusAndNeptuneCompletedCutscene.Play();
	}

	public void OnJupiterAndSaturnCompleted()
	{
		_jupiterAndSaturnCompletedCutscene.Play();
	}

	public void OnEarthAndMarsCompleted()
	{
		_earthAndMarsCompletedCutscene.Play();
	}

	public void OnMercuryAndVenusCompleted()
	{
		_mercuryAndVenusCompletedCutscene.Play();
	}

	public void OnSunCompleted()
	{
		_sunCompletedCutscene.Play();
	}

	public void OnGameCompleted()
	{
		GetTree().ChangeSceneToFile(_credits);
	}

	private void ImmediateCutsceneOnCutsceneSequenceFinished()
	{
		StartSpawner(_neptune.EnemySpawner);
		StartSpawner(_uranus.EnemySpawner);
	}

	private void UranusAndNeptuneCompletedCutsceneOnCutsceneSequenceFinished()
	{
		_jupiterAndSaturnForceField.Disable();
		Player.Instance.HealthComponent.Health = Player.Instance.HealthComponent.MaxHealth;
		StartSpawner(_saturn.EnemySpawner);
		StartSpawner(_jupiter.EnemySpawner);
	}

	private void JupiterAndSaturnCompletedCutsceneOnCutsceneSequenceFinished()
	{
		_earthAndMarsForceField.Disable();
		Player.Instance.HealthComponent.Health = Player.Instance.HealthComponent.MaxHealth;
		StartSpawner(_mars.EnemySpawner);
		StartSpawner(_earth.EnemySpawner);
	}

	private void EarthAndMarsCompletedCutsceneOnCutsceneSequenceFinished()
	{
		_mercuryAndVenusForceField.Disable();
		Player.Instance.HealthComponent.Health = Player.Instance.HealthComponent.MaxHealth;
		StartSpawner(_venus.EnemySpawner);
		StartSpawner(_mercury.EnemySpawner);
	}

	private static void MercuryAndVenusCompletedCutsceneOnCutsceneStarted()
	{
		AudioManager.Instance.GameplayPlayer.FadeOut(12f);
	}

	private void MercuryAndVenusCompletedCutsceneOnCutsceneSequenceFinished()
	{
		AudioManager.Instance.BossPlayer.FadeIn(4f);
		_sunForceField.Disable();
		Player.Instance.HealthComponent.Health = Player.Instance.HealthComponent.MaxHealth;
		StartSpawner(_sun.EnemySpawner);
	}

	private void SunCompletedCutsceneOnCutsceneSequenceFinished()
	{
		EmitSignal(SignalName.GameCompleted);
	}

	public override void _Ready()
	{
		base._Ready();
		_neptune.EnemySpawner.AllEnemiesDestroyed += OnNeptuneEnemiesDestroyed;
		_uranus.EnemySpawner.AllEnemiesDestroyed += OnUranusEnemiesDestroyed;
		_saturn.EnemySpawner.AllEnemiesDestroyed += OnSaturnEnemiesDestroyed;
		_jupiter.EnemySpawner.AllEnemiesDestroyed += OnJupiterEnemiesDestroyed;
		_mars.EnemySpawner.AllEnemiesDestroyed += OnMarsEnemiesDestroyed;
		_earth.EnemySpawner.AllEnemiesDestroyed += OnEarthEnemiesDestroyed;
		_venus.EnemySpawner.AllEnemiesDestroyed += OnVenusEnemiesDestroyed;
		_mercury.EnemySpawner.AllEnemiesDestroyed += OnMercuryEnemiesDestroyed;
		_sun.EnemySpawner.AllEnemiesDestroyed += OnSunEnemiesDestroyed;

		UranusAndNeptuneCompleted += OnUranusAndNeptuneCompleted;
		JupiterAndSaturnCompleted += OnJupiterAndSaturnCompleted;
		EarthAndMarsCompleted += OnEarthAndMarsCompleted;
		MercuryAndVenusCompleted += OnMercuryAndVenusCompleted;
		SunCompleted += OnSunCompleted;
		GameCompleted += OnGameCompleted;

		_immediateCutscene.CutsceneSequenceFinished += ImmediateCutsceneOnCutsceneSequenceFinished;
		_uranusAndNeptuneCompletedCutscene.CutsceneSequenceFinished += UranusAndNeptuneCompletedCutsceneOnCutsceneSequenceFinished;
		_jupiterAndSaturnCompletedCutscene.CutsceneSequenceFinished += JupiterAndSaturnCompletedCutsceneOnCutsceneSequenceFinished;
		_earthAndMarsCompletedCutscene.CutsceneSequenceFinished += EarthAndMarsCompletedCutsceneOnCutsceneSequenceFinished;
		_mercuryAndVenusCompletedCutscene.CutsceneStarted += MercuryAndVenusCompletedCutsceneOnCutsceneStarted;
		_mercuryAndVenusCompletedCutscene.CutsceneSequenceFinished += MercuryAndVenusCompletedCutsceneOnCutsceneSequenceFinished;
		_sunCompletedCutscene.CutsceneSequenceFinished += SunCompletedCutsceneOnCutsceneSequenceFinished;

		AudioManager audioManager = AudioManager.Instance;
		Instance = this;
		audioManager.MenuPlayer.FadeOut(4f);
		audioManager.GameplayPlayer.FadeOut(0.1f);
		audioManager.BossPlayer.FadeOut(4f);
		_immediateCutscene.NextCutsceneInSequence.CutsceneStarted += SecondCutsceneInImmediateSequenceOnCutsceneStarted;
		_immediateCutscene.Play();
	}

	private void SecondCutsceneInImmediateSequenceOnCutsceneStarted()
	{
		AudioManager.Instance.GameplayPlayer.FadeIn(4f);
	}


	public override void _ExitTree()
	{
		base._ExitTree();

		EmitSignal(SignalName.ExitTree);

		_neptune.EnemySpawner.AllEnemiesDestroyed -= OnNeptuneEnemiesDestroyed;
		_uranus.EnemySpawner.AllEnemiesDestroyed -= OnUranusEnemiesDestroyed;
		_saturn.EnemySpawner.AllEnemiesDestroyed -= OnSaturnEnemiesDestroyed;
		_jupiter.EnemySpawner.AllEnemiesDestroyed -= OnJupiterEnemiesDestroyed;
		_mars.EnemySpawner.AllEnemiesDestroyed -= OnMarsEnemiesDestroyed;
		_earth.EnemySpawner.AllEnemiesDestroyed -= OnEarthEnemiesDestroyed;
		_venus.EnemySpawner.AllEnemiesDestroyed -= OnVenusEnemiesDestroyed;
		_mercury.EnemySpawner.AllEnemiesDestroyed -= OnMercuryEnemiesDestroyed;
		_sun.EnemySpawner.AllEnemiesDestroyed -= OnSunEnemiesDestroyed;

		UranusAndNeptuneCompleted -= OnUranusAndNeptuneCompleted;
		JupiterAndSaturnCompleted -= OnJupiterAndSaturnCompleted;
		EarthAndMarsCompleted -= OnEarthAndMarsCompleted;
		MercuryAndVenusCompleted -= OnMercuryAndVenusCompleted;
		SunCompleted -= OnSunCompleted;
		GameCompleted -= OnGameCompleted;

		_immediateCutscene.CutsceneSequenceFinished -= ImmediateCutsceneOnCutsceneSequenceFinished;
		_uranusAndNeptuneCompletedCutscene.CutsceneSequenceFinished -= UranusAndNeptuneCompletedCutsceneOnCutsceneSequenceFinished;
		_jupiterAndSaturnCompletedCutscene.CutsceneSequenceFinished -= JupiterAndSaturnCompletedCutsceneOnCutsceneSequenceFinished;
		_earthAndMarsCompletedCutscene.CutsceneSequenceFinished -= EarthAndMarsCompletedCutsceneOnCutsceneSequenceFinished;
		_mercuryAndVenusCompletedCutscene.CutsceneSequenceFinished -= MercuryAndVenusCompletedCutsceneOnCutsceneSequenceFinished;
		_sunCompletedCutscene.CutsceneSequenceFinished -= SunCompletedCutsceneOnCutsceneSequenceFinished;

		_immediateCutscene.NextCutsceneInSequence.CutsceneStarted -= SecondCutsceneInImmediateSequenceOnCutsceneStarted;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (!@event.IsActionPressed(Action.DebugSkip) || !OS.HasFeature("editor")) return;

		DestroyEnemies();
	}

	public void DestroyEnemies(bool finishSpawning = true)
	{
		foreach (EnemySpawner spawner in _currentSpawners)
		{
			if (finishSpawning)
			{
				spawner.FinishSpawning();
			}
			foreach (Enemy enemy in spawner.spawnedEnemies)
			{
				enemy.HealthComponent.Health = 0f;
			}
		}
	}

	private void StartSpawner(EnemySpawner spawner)
	{
		spawner.StartSpawning();
		_currentSpawners.Add(spawner);
		spawner.AllEnemiesDestroyed += SpawnerOnAllEnemiesDestroyed;
		ExitTree += () => spawner.AllEnemiesDestroyed -= SpawnerOnAllEnemiesDestroyed;
		return;

		void SpawnerOnAllEnemiesDestroyed()
		{
			_currentSpawners.Remove(spawner);
		}
	}
}
