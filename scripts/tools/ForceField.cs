using System;
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

    [Export]
    public Color Color
    {
        get => _color;
        set => SetColor(value);
    }

    private Color _color = new(0f, 1f, 1f, 0.5f);

    private float _radius = 1000f;
    private float _thickness = 50f;

    private readonly List<CollisionShape2D> _collisionShapes = new(Sides);
    private readonly List<Polygon2D> _polygons = new(Sides);
    private CapsuleShape2D _sharedCapsuleShape;

    private const int Sides = 6;
    private const float SideSubtendedRadians = Mathf.Tau / Sides;
    private const float HalfSideSubtendedRadians = 0.5f * SideSubtendedRadians;

    private void SetRadius(float radius)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (_radius == radius) return;
        _radius = radius;
        Update();
    }

    private void SetThickness(float thickness)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (_thickness == thickness) return;
        _thickness = thickness;
        Update();
    }

    private void SetColor(Color color)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (_color == color) return;
        _color = color;
        Update();
    }

    public override void _Ready()
    {
        base._Ready();

        _sharedCapsuleShape ??= new CapsuleShape2D();
        // Being a tool, things act weird. Ready is called the moment scene is dragged over viewport.
        // And again when released and added to editor's scene tree.
        // Neither has node's owner set so it complains. Defer update to another frame.
        CallDeferred(MethodName.ToolReady);
    }

    private void ToolReady()
    {
        if (Owner == null) return;

        _sharedCapsuleShape ??= new CapsuleShape2D();
        Update();
    }

    private void Update()
    {
        if (!IsInsideTree() || GetTree().EditedSceneRoot == this) return;

        bool invalid = _collisionShapes.Any(t => !t.IsPartOfEditedScene())
                       || _polygons.Any(t => !t.IsPartOfEditedScene())
                       || _collisionShapes.Count != Sides
                       || _collisionShapes.Count != _polygons.Count;
        if (invalid) RecreateChildren();

        // Polygon will be child of collider so the data only needs to be set for the initial position.
        // The transform inheritance of the parent collider will do the rest.
        float wallRadius = 0.5f * _thickness;
        float cos = MathF.Cos(HalfSideSubtendedRadians);
        Vector2[] polygonData;
        {
            float innerRadius = _radius - wallRadius;
            float outerRadius = _radius + wallRadius;
            float sinEnd = MathF.Sin(HalfSideSubtendedRadians);
            float outerX = wallRadius * cos;
            float innerY = innerRadius * sinEnd;
            float outerY = outerRadius * sinEnd;
            polygonData =
            [
                new Vector2(outerX, -outerY),
                new Vector2(-outerX, -innerY),
                new Vector2(-outerX, innerY),
                new Vector2(outerX, outerY)
            ];
        }

        float inradius = cos * _radius;
        Vector2 firstPosition = Vector2.Right * inradius;
        for (int i = 0; i < _collisionShapes.Count; i++)
        {
            Polygon2D polygon2D = _polygons[i];
            polygon2D.Polygon = polygonData;
            polygon2D.Color = _color;

            CollisionShape2D collisionShape2D = _collisionShapes[i];
            float radians = i * SideSubtendedRadians;
            Vector2 position = firstPosition.Rotated(radians);
            collisionShape2D.Rotation = radians;
            collisionShape2D.Position = position;
            if (collisionShape2D.Shape is CapsuleShape2D capsuleShape2D)
            {
                capsuleShape2D.Height = _radius + 2f * capsuleShape2D.Radius;
                capsuleShape2D.Radius = wallRadius;
            }
            else
            {
                GD.PushError($"Shape2D is not CapsuleShape2D. Iter: {i}, Name: {collisionShape2D.Name}");
                GD.PushError(collisionShape2D);
            }
        }
    }

    private void RecreateChildren()
    {
        _collisionShapes.Clear();
        _polygons.Clear();

        int childCount = GetChildCount();
        for (int i = 0; i < childCount; i++)
        {
            Node child = GetChild(i);
            if (child is CollisionShape2D or Polygon2D)
            {
                child.QueueFree();
            }
        }

        Vector2[] polygonData = [Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero];
        for (int i = 0; i < Sides; i++)
        {
            CollisionShape2D collisionShape = new();
            collisionShape.SetMeta("_edit_lock_", true);
            collisionShape.Shape = _sharedCapsuleShape;
            AddChild(collisionShape);
            collisionShape.SetOwner(GetTree().EditedSceneRoot);
            collisionShape.Name = nameof(CollisionShape2D) + i;
            _collisionShapes.Add(collisionShape);

            Polygon2D polygon2D = new();
            polygon2D.SetMeta("_edit_lock_", true);
            polygon2D.Polygon = polygonData;
            collisionShape.AddChild(polygon2D, true);
            polygon2D.SetOwner(GetTree().EditedSceneRoot);
            _polygons.Add(polygon2D);
        }
    }
}
