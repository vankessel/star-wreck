using System.Diagnostics.CodeAnalysis;
using Godot;
using StarWreck.scripts.input;

namespace StarWreck.scripts;

public partial class PlayerCamera : Camera2D
{
    [Export] private Player _player;

    [Export(PropertyHint.Range, "0,3.1415926536")]
    private float _rotationSpeed = 3f;

    [Export(PropertyHint.Range, "0,4,or_greater")]
    private float _zoomSpeed = 2f;

    [Export(PropertyHint.Range, "0,1,or_greater")]
    private float _zoomMouseWheelSensitivity = 0.05f;

    [Export(PropertyHint.Range, "0,1,or_greater")]
    private float _playerPositionTrackingHalfLife = 0.125f;

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

        HandleInput(dt);
        Track(_player, dt);
    }

    private void HandleInput(float dt)
    {
        float rotationInput = Input.GetAxis(Action.RotateLeft, Action.RotateRight);
        float zoomInput = Input.GetAxis(Action.ZoomOut, Action.ZoomIn);

        float radians = rotationInput * _rotationSpeed * dt;
        Position = _player.GlobalPosition + (GlobalPosition - _player.GlobalPosition).Rotated(radians);
        Rotation += radians;

        Zoom *= Mathf.Max(0f, 1f + zoomInput * _zoomSpeed * dt);
    }

    private void Track(Node2D node2D, float dt)
    {
        GlobalPosition = ExponentialMovingAverage(GlobalPosition, node2D.GlobalPosition, _playerPositionTrackingHalfLife, dt);
    }

    /// <param name="halfLife">Equal to meanLifetime*(-ln(1/2))</param>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    private static Vector2 ExponentialMovingAverage(Vector2 current, Vector2 target, float halfLife, float dt)
    {
        const float lnHalf = -0.693147180559945309417232f;
        float factor = Mathf.Exp(dt * lnHalf / halfLife);
        return current * factor + target * (1f - factor);
    }
}
