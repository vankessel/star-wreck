using Godot;
using StarWreck.scripts.input;

namespace StarWreck.scenes.credits;

public partial class MainSubmenu : Node
{
    [Export(PropertyHint.File, "*.tscn")] private string _nextScene;
    [Export] private Button _mainMenuButton;

    public override void _Ready()
    {
        base._Ready();
        _mainMenuButton.Pressed += StartButtonOnPressed;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _mainMenuButton.Pressed -= StartButtonOnPressed;
    }

    private void StartButtonOnPressed()
    {
        GetTree().ChangeSceneToFile(_nextScene);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMouseButton) return;

        if (@event.IsActionPressed(Action.ProgressDialogue))
        {
            _mainMenuButton.EmitSignal(BaseButton.SignalName.Pressed);
        }
    }
}
