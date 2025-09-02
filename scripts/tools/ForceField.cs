using System.Collections.Generic;
using System.Linq;
using Godot;

namespace StarWreck.scripts.tools;

[Tool]
[GlobalClass]
public partial class ForceField : AnimatableBody2D
{
    [Export]
    public float Radius
    {
        get => _radius;
        set => SetRadius(value);
    }

    [Export]
    public float Thickness
    {
        get => _thickness;
        set => SetThickness(value);
    }

    private float _radius = 1000f;
    private float _thickness = 50f;

    private readonly List<CollisionShape2D> _collisionShapes = new(6);
    private const int Sides = 6;

    public void SetRadius(float radius)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (_radius == radius) return;
        _radius = radius;
        Update();
    }

    public void SetThickness(float thickness)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (_thickness == thickness) return;
        _thickness = thickness;
        Update();
    }

    private const float SideSubtendedRadians = Mathf.Tau / Sides;
    private const float HalfSideSubtendedRadians = 0.5f * SideSubtendedRadians;

    public override void _Ready()
    {
        base._Ready();

        // Being a tool, things act weird. Ready is called the moment scene is dragged over viewport.
        // And again when released and added to editor's scene tree.
        // Neither has node's owner set so it complains. Defer update to another frame.
        CallDeferred(MethodName.UpdateIfOwnerExists);
    }

    private void UpdateIfOwnerExists()
    {
        if (Owner == null) return;
        Update();
    }

    private void Update()
    {
        if (!IsInsideTree() || GetTree().EditedSceneRoot == this) return;

        bool invalid = _collisionShapes.Any(t => !t.IsPartOfEditedScene());
        if (invalid || _collisionShapes.Count != Sides) RecreateChildren();

        for (int i = 0; i < _collisionShapes.Count; i++)
        {
            CollisionShape2D collisionShape2D = _collisionShapes[i];
            float radians = i * SideSubtendedRadians;
            float inRadius = Mathf.Cos(HalfSideSubtendedRadians) * _radius;
            Vector2 position = (Vector2.Right * inRadius).Rotated(radians);
            collisionShape2D.Rotation = radians;
            collisionShape2D.Position = position;
            if (collisionShape2D.Shape is CapsuleShape2D capsuleShape2D)
            {
                capsuleShape2D.Height = _radius + 2f * capsuleShape2D.Radius;
                capsuleShape2D.Radius = 0.5f * _thickness;
            }
            else
            {
                GD.PushError($"Shape2D is not CapsuleShape2D. Iter: {{i}}, Name: {collisionShape2D.Name}");
            }
        }
    }

    private void RecreateChildren()
    {
        _collisionShapes.Clear();

        int childCount = GetChildCount();
        for (int i = 0; i < childCount; i++)
        {
            Node child = GetChild(i);
            if (child is CollisionShape2D collisionShape2D)
            {
                collisionShape2D.QueueFree();
            }
        }

        CapsuleShape2D newSharedShape = new();
        for (int i = 0; i < Sides; i++)
        {
            CollisionShape2D collisionShape = new();
            collisionShape.SetMeta("_edit_lock_", true);
            collisionShape.Shape = newSharedShape;
            AddChild(collisionShape, true);
            collisionShape.SetOwner(GetTree().EditedSceneRoot);
            _collisionShapes.Add(collisionShape);
        }
    }
}
