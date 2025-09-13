using DialogueManagerRuntime;
using Godot;

namespace StarWreck.scripts.progression;

[GlobalClass]
public partial class Cutscene : Node
{
    [Export] private bool _pause = true;
    [Export] private PackedScene _scene;

    [Export(PropertyHint.File, "*.dialogue")]
    private Resource _dialogue;

    [Signal]
    public delegate void CutsceneStartedEventHandler();

    [Signal]
    public delegate void CutsceneFinishedEventHandler();

    [Signal]
    public delegate void CutsceneSequenceStartedEventHandler();

    [Signal]
    public delegate void CutsceneSequenceFinishedEventHandler();

    private Node _instancedScene;
    private Cutscene _previousCutsceneInSequence;
    private Cutscene _nextCutsceneInSequence;

    public override void _Ready()
    {
        base._Ready();

        _nextCutsceneInSequence = GetChildOrNull<Cutscene>(0);
        if (_nextCutsceneInSequence != null)
        {
            _nextCutsceneInSequence._previousCutsceneInSequence = this;
        }

        DialogueManager.DialogueStarted += DialogueStarted;
        DialogueManager.DialogueEnded += DialogueEnded;
        CutsceneFinished += OnCutsceneFinished;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueManager.DialogueStarted -= DialogueStarted;
        DialogueManager.DialogueEnded -= DialogueEnded;
        CutsceneFinished -= OnCutsceneFinished;
    }

    private void DialogueStarted(Resource dialogueResource)
    {
        if (dialogueResource != _dialogue) return;
        EmitSignal(SignalName.CutsceneStarted);
    }

    private void DialogueEnded(Resource dialogueResource)
    {
        if (dialogueResource != _dialogue) return;

        _instancedScene?.QueueFree();

        GetTree().Paused = false;

        EmitSignal(SignalName.CutsceneFinished);
    }

    private void OnCutsceneStarted()
    {
        if (_previousCutsceneInSequence != null) return;
        Cutscene currentCutscene = this;
        while (currentCutscene != null)
        {
            currentCutscene.EmitSignal(SignalName.CutsceneSequenceStarted);
            currentCutscene = currentCutscene._nextCutsceneInSequence;
        }
    }

    private void OnCutsceneFinished()
    {
        if (_nextCutsceneInSequence != null)
        {
            _nextCutsceneInSequence.Play();
        }
        else
        {
            Cutscene currentCutscene = this;
            while (currentCutscene != null)
            {
                currentCutscene.EmitSignal(SignalName.CutsceneSequenceFinished);
                currentCutscene = currentCutscene._previousCutsceneInSequence;
            }
        }
    }

    public void Play()
    {
        if (_dialogue != null)
        {
            if (_scene != null)
            {
                _instancedScene = _scene.Instantiate();
                _instancedScene.ProcessMode = ProcessModeEnum.Always;
                AddChild(_instancedScene);
                _instancedScene.Owner = GetTree().Root;

                GetTree().Paused = true;
            }
            else
            {
                GetTree().Paused = _pause;
            }

            Node node = DialogueManager.ShowDialogueBalloon(_dialogue);
            node.ProcessMode = ProcessModeEnum.Always;
        }
        else
        {
            EmitSignal(SignalName.CutsceneFinished);
            GD.PushWarning($"No dialogue set for {Name}");
        }
    }
}
