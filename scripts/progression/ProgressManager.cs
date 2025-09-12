using Godot;
using StarWreck.scripts.tools;

namespace StarWreck.scripts.progression;

public partial class ProgressManager : Node
{
    [Export(PropertyHint.File, "*.tscn")] private string _credits;

    [ExportGroup("Cutscenes")]
    [Export] private Cutscene _immediateCutscene;
    [Export] private Cutscene _uranusAndNeptuneCompletedCutscene;
    [Export] private Cutscene _jupiterAndSaturnCompletedCutscene;
    [Export] private Cutscene _earthAndMarsCompletedCutscene;
    [Export] private Cutscene _mercuryAndVenusCompletedCutscene;
    [Export] private Cutscene _sunCompletedCutscene;

    [ExportGroup("Celestial Objects")]
    [Export] private CelestialObject _neptune;
    [Export] private CelestialObject _uranus;
    [Export] private CelestialObject _saturn;
    [Export] private CelestialObject _jupiter;
    [Export] private CelestialObject _mars;
    [Export] private CelestialObject _earth;
    [Export] private CelestialObject _venus;
    [Export] private CelestialObject _mercury;
    [Export] private CelestialObject _sun;

    [ExportGroup("Force Fields")]
    [Export] private ForceField _uranusAndNeptuneForceField;
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

    private void ImmediateCutsceneOnCutsceneFinished()
    {
        _neptune.EnemySpawner.StartSpawning();
        _uranus.EnemySpawner.StartSpawning();
    }

    private void UranusAndNeptuneCompletedCutsceneOnCutsceneFinished()
    {
        _jupiterAndSaturnForceField.Disable();
        _saturn.EnemySpawner.StartSpawning();
        _jupiter.EnemySpawner.StartSpawning();
    }

    private void JupiterAndSaturnCompletedCutsceneOnCutsceneFinished()
    {
        _earthAndMarsForceField.Disable();
        _mars.EnemySpawner.StartSpawning();
        _earth.EnemySpawner.StartSpawning();
    }

    private void EarthAndMarsCompletedCutsceneOnCutsceneFinished()
    {
        _mercuryAndVenusForceField.Disable();
        _venus.EnemySpawner.StartSpawning();
        _mercury.EnemySpawner.StartSpawning();
    }

    private void MercuryAndVenusCompletedCutsceneOnCutsceneFinished()
    {
        _sunForceField.Disable();
        _sun.EnemySpawner.StartSpawning();
    }

    private void SunCompletedCutsceneOnCutsceneFinished()
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

        _immediateCutscene.CutsceneFinished += ImmediateCutsceneOnCutsceneFinished;
        _uranusAndNeptuneCompletedCutscene.CutsceneFinished += UranusAndNeptuneCompletedCutsceneOnCutsceneFinished;
        _jupiterAndSaturnCompletedCutscene.CutsceneFinished += JupiterAndSaturnCompletedCutsceneOnCutsceneFinished;
        _earthAndMarsCompletedCutscene.CutsceneFinished += EarthAndMarsCompletedCutsceneOnCutsceneFinished;
        _mercuryAndVenusCompletedCutscene.CutsceneFinished += MercuryAndVenusCompletedCutsceneOnCutsceneFinished;
        _sunCompletedCutscene.CutsceneFinished += SunCompletedCutsceneOnCutsceneFinished;

        _immediateCutscene.Play();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
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

        _immediateCutscene.CutsceneFinished -= ImmediateCutsceneOnCutsceneFinished;
        _uranusAndNeptuneCompletedCutscene.CutsceneFinished -= UranusAndNeptuneCompletedCutsceneOnCutsceneFinished;
        _jupiterAndSaturnCompletedCutscene.CutsceneFinished -= JupiterAndSaturnCompletedCutsceneOnCutsceneFinished;
        _earthAndMarsCompletedCutscene.CutsceneFinished -= EarthAndMarsCompletedCutsceneOnCutsceneFinished;
        _mercuryAndVenusCompletedCutscene.CutsceneFinished -= MercuryAndVenusCompletedCutsceneOnCutsceneFinished;
        _sunCompletedCutscene.CutsceneFinished -= SunCompletedCutsceneOnCutsceneFinished;
    }
}
