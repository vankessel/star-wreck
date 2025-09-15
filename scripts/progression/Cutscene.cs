using System;
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

	[Export(PropertyHint.Range, "0,10,or_greater")]
	private float _waitBefore = 0f;

	[Export(PropertyHint.Range, "0,10,or_greater")]
	private float _holdNoDialogueSceneFor = 5f;

	[Export(PropertyHint.Range, "0,10,or_greater")]
	private float _waitAfter = 0f;

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

		DialogueManager.DialogueEnded += OnDialogueEnded;
		CutsceneFinished += OnCutsceneFinished;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		DialogueManager.DialogueEnded -= OnDialogueEnded;
		CutsceneFinished -= OnCutsceneFinished;
	}

	private void OnDialogueEnded(Resource dialogueResource)
	{
		if (dialogueResource != _dialogue) return;

		AfterCutsceneContent();
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

	private void AfterSeconds(float seconds, Action action)
	{
		if (seconds > 0f)
		{
			Timer timer = new();
			timer.WaitTime = seconds;
			timer.Timeout += () =>
			{
				action.Invoke();
			};
			timer.Autostart = true;
			timer.OneShot = true;
			AddChild(timer);
		}
		else
		{
			action.Invoke();
		}
	}

	public void Play()
	{
		EmitSignal(SignalName.CutsceneStarted);

		AfterSeconds(_waitBefore, Start);
	}

	private void Start()
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
			ProcessModeEnum prevProcessMode = ProcessMode;
			ProcessMode = ProcessModeEnum.Always;
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
				GD.PushError($"No dialogue or scene for Cutscene! {Name}");
				GetTree().Paused = _pause;
			}

			AfterSeconds(_holdNoDialogueSceneFor, () =>
			{
				AfterCutsceneContent();
				ProcessMode = prevProcessMode;
			});
		}
	}

	private void AfterCutsceneContent()
	{
		_instancedScene?.QueueFree();

		GetTree().Paused = false;

		AfterSeconds(_waitAfter, Finish);
	}

	private void Finish()
	{
		EmitSignal(SignalName.CutsceneFinished);
	}
}
