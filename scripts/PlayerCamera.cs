using Godot;
using StarWreck.scripts.input;

namespace StarWreck.scripts;

public partial class PlayerCamera : Camera2D
{
    [Export] private Player _player;
    [Export(PropertyHint.Range, "0,3.1415926536")] private float _rotationSpeed = Mathf.Pi * 0.5f;
    [Export(PropertyHint.Range, "0,4,or_greater")] private float _zoomSpeed = 2f;
    [Export(PropertyHint.Range, "0,1,or_greater")] private float _zoomMouseWheelSensitivity = 0.05f;

    public override void _Ready()
    {
        base._Ready();
        if (_player == null) GD.PushWarning("Player not set.");
        if (IgnoreRotation) GD.PushWarning("Player camera ignoring rotation.");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is not InputEventMouseButton) return;

        if (@event.IsActionPressed(Action.ZoomOut))
        {
            Zoom *= Mathf.Max(0f, 1f - _zoomSpeed * _zoomMouseWheelSensitivity);
        }
        else if (@event.IsActionPressed(Action.ZoomIn))
        {
            Zoom *= Mathf.Max(0f, 1f + _zoomSpeed * _zoomMouseWheelSensitivity);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        float dt = (float)delta;

        float rotationInput = Input.GetAxis(Action.RotateRight, Action.RotateLeft);
        float zoomInput = Input.GetAxis(Action.ZoomOut, Action.ZoomIn);

        float radians = rotationInput * _rotationSpeed * dt;
        Position = _player.GlobalPosition + (GlobalPosition - _player.GlobalPosition).Rotated(radians);
        Rotation += radians;

        Zoom *= Mathf.Max(0f, 1f + zoomInput * _zoomSpeed * dt);
    }
}
