using Godot;

namespace StarWreck.scenes.credits;

public partial class CreditsMenu : Node
{
    [Export(PropertyHint.File, "*.tscn")] private string _mainMenuScene;
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
        GetTree().ChangeSceneToFile(_mainMenuScene);
    }
}
