using DialogueManagerRuntime;
using Godot;

namespace StarWreck.scripts.progression;

[GlobalClass]
public partial class Cutscene : Node
{
    [Export] private bool _pause = true;
    [Export] private PackedScene _scene;
    [Export(PropertyHint.File, "*.dialogue")] private Resource _dialogue;

    [Signal] public delegate void CutsceneStartedEventHandler();
    [Signal] public delegate void CutsceneFinishedEventHandler();

    private Node _instancedScene;

    public override void _Ready()
    {
        base._Ready();
        DialogueManager.DialogueStarted += DialogueStarted;
        DialogueManager.DialogueEnded += DialogueEnded;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueManager.DialogueStarted -= DialogueStarted;
        DialogueManager.DialogueEnded -= DialogueEnded;
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

    public void Play()
    {
        if (_scene != null && _dialogue != null)
        {
            _instancedScene = _scene.Instantiate();
            _instancedScene.ProcessMode = ProcessModeEnum.Always;
            AddChild(_instancedScene);
            _instancedScene.Owner = GetTree().Root;

            Node node = DialogueManager.ShowDialogueBalloon(_dialogue);
            node.ProcessMode = ProcessModeEnum.Always;

            GetTree().Paused = _pause;
        }
        else
        {
            EmitSignal(SignalName.CutsceneFinished);
            GD.PushWarning($"No dialogue set for {Name}");
        }
    }
}
