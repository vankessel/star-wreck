using Godot;
using StarWreck.scripts.autoloads;
using Action = StarWreck.scripts.input.Action;

namespace StarWreck.scenes.main_menu;

public partial class MainMenu : Node
{
	[Export(PropertyHint.File, "*.tscn")] private string _startScene;
	[Export(PropertyHint.File, "*.tscn")] private string _controlsScene;
	[Export(PropertyHint.File, "*.tscn")] private string _creditsScene;
	[Export] private Button _startButton;
	[Export] private Button _controlsButton;
	[Export] private Button _creditsButton;
	[Export] private Button _exitButton;
	[Export] private Color _focusColor = Color.FromHtml("5cf4ff");

	private Button[] _buttons = new Button[4];
	private int _cursorIndex = 0;
	private bool _usingGamepad;
	private float _lastActionStrength = 0f;
	private float _actionStrengthThreshold = 0.5f;

	public override void _Ready()
	{
		base._Ready();

		AudioManager.Instance.BossPlayer.FadeOut(4);
		AudioManager.Instance.MenuPlayer.FadeIn(4);

		_startButton.Pressed += StartButtonOnPressed;
		_controlsButton.Pressed += ControlsButtonOnPressed;
		_creditsButton.Pressed += CreditsButtonOnPressed;
		_exitButton.Pressed += ExitButtonOnPressed;

		_buttons[0] = _startButton;
		_buttons[1] = _controlsButton;
		_buttons[2] = _creditsButton;
		_buttons[3] = _exitButton;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_startButton.Pressed -= StartButtonOnPressed;
		_creditsButton.Pressed -= CreditsButtonOnPressed;
		_exitButton.Pressed -= ExitButtonOnPressed;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		switch (@event)
		{
			case InputEventKey or InputEventMouse:
				_usingGamepad = false;
				ClearFocus();
				break;
			case InputEventJoypadButton or InputEventJoypadMotion:
				_usingGamepad = true;
				SetFocus();
				break;
		}


		if (@event.IsActionPressed(Action.ProgressDialogue))
		{
			_buttons[_cursorIndex].EmitSignal(BaseButton.SignalName.Pressed);
			return;
		}

		bool cursorChanged = false;
		if (@event.IsActionPressed(Action.Up))
		{
			float currentActionStrength = @event.GetActionStrength(Action.Up);
			if (currentActionStrength > _actionStrengthThreshold && _lastActionStrength <  _actionStrengthThreshold)
			{
				_cursorIndex--;
				cursorChanged = true;
			}
			_lastActionStrength	= currentActionStrength;
		}
		else if (@event.IsActionPressed(Action.Down))
		{
			float currentActionStrength = @event.GetActionStrength(Action.Down);
			if (currentActionStrength > _actionStrengthThreshold && _lastActionStrength <  _actionStrengthThreshold)
			{
				_cursorIndex++;
				cursorChanged = true;
			}
			_lastActionStrength	= currentActionStrength;
		}

		if (!cursorChanged) return;

		_cursorIndex = Mathf.Clamp(_cursorIndex, 0, 3);

		if (!_usingGamepad) return;

		SetFocus();
	}

	private void ClearFocus()
	{
		foreach (Button button in _buttons)
		{
			button.SetModulate(Colors.White);
		}
	}

	private void SetFocus()
	{
		for (int index = 0; index < _buttons.Length; index++)
		{
			Button button = _buttons[index];
			button.SetModulate(index == _cursorIndex ? _focusColor : Colors.White);
		}
	}

	private void StartButtonOnPressed()
	{
		GetTree().ChangeSceneToFile(_startScene);
	}

	private void ControlsButtonOnPressed()
	{
		GetTree().ChangeSceneToFile(_controlsScene);
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
