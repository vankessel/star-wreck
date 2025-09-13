using Godot;

namespace StarWreck.scripts.input;

public static class Action
{
    public static readonly StringName Up = new("Up");
    public static readonly StringName Down = new("Down");
    public static readonly StringName Left = new("Left");
    public static readonly StringName Right = new("Right");
    public static readonly StringName RotateLeft = new("RotateLeft");
    public static readonly StringName RotateRight = new("RotateRight");
    public static readonly StringName ZoomIn = new("ZoomIn");
    public static readonly StringName ZoomOut = new("ZoomOut");
    public static readonly StringName Pause = new("Pause");
    public static readonly StringName ProgressDialogue = new("ProgressDialogue");
}
