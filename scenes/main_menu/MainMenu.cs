using Godot;

namespace StarWreck.scenes.main_menu;

public partial class MainMenu : Node
{
    [Export(PropertyHint.File, "*tscn")] private string _startScene;
    [Export(PropertyHint.File, "*tscn")] private string _creditsScene;
    [Export] private Button _startButton;
    [Export] private Button _creditsButton;
    [Export] private Button _exitButton;

    public override void _Ready()
    {
        base._Ready();
        _startButton.Pressed += StartButtonOnPressed;
        _creditsButton.Pressed += CreditsButtonOnPressed;
        _exitButton.Pressed += ExitButtonOnPressed;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _startButton.Pressed -= StartButtonOnPressed;
        _creditsButton.Pressed -= CreditsButtonOnPressed;
        _exitButton.Pressed -= ExitButtonOnPressed;
    }

    private void StartButtonOnPressed()
    {
        GetTree().ChangeSceneToFile(_startScene);
    }

    private void CreditsButtonOnPressed()
    {
        GetTree().ChangeSceneToFile(_creditsScene);
    }

    private void ExitButtonOnPressed()
    {
        GetTree().Quit();
    }
}
