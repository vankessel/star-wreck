using Godot;

namespace StarWreck.scripts.input;

public static class Action
{
    public static StringName Up = new("Up");
    public static StringName Down = new("Down");
    public static StringName Left = new("Left");
    public static StringName Right = new("Right");
    public static StringName RotateLeft = new("RotateLeft");
    public static StringName RotateRight = new("RotateRight");
    public static StringName ZoomIn = new("ZoomIn");
    public static StringName ZoomOut = new("ZoomOut");
}
