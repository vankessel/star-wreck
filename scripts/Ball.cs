using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace StarWreck.scripts;

public partial class Ball : TrackedRigidBody2D
{
    private bool _checkedBreakables;
    private List<IBreakable> _breakables = new(4);

    public override void _Ready()
    {
        base._Ready();

        BodyEntered += OnBodyEntered;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        BodyEntered -= OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is not IBreakable breakable) return;
        _breakables.Add(breakable);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        // if (_checkedBreakables) return;
        if (_breakables.Count == 0) return;
        IBreakable[] breakables = _breakables.ToArray();
        _breakables.Clear();
        CheckBreakables(breakables);
        // _checkedBreakables = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        // _checkedBreakables = false;
    }

    public void CheckBreakables(IBreakable[] breakables)
    {
        float[] weights = new float[breakables.Length];

        Vector2 velocityDir = LinearVelocity.Normalized();
        float weightsSum = 0f;

        // Want to distribute damage across bodies most in direct path of velocity.
        for (int i = 0; i < breakables.Length; i++)
        {
            IBreakable breakable = breakables[i];
            Vector2 relativePositionDir = (breakable.GlobalPosition - GlobalPosition).Normalized();
            float weight = Mathf.Max(0f, velocityDir.Dot(relativePositionDir));
            breakables[i] = breakable;
            weights[i] = weight;
            weightsSum += weight;
        }

        if (weightsSum == 0f) return;

        float inverseWeightSum = 1f / weightsSum;
        for (int i = 0; i < breakables.Length; i++)
        {
            IBreakable breakable = breakables[i];
            float normalizedWeight = weights[i] * inverseWeightSum;
            breakable.Damage(normalizedWeight * VelocityChange, this);
        }
    }
}
