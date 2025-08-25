using System;
using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public partial class UniformChainedMaxDistanceConstraint2D : Constraint2D
{
    [Export(PropertyHint.Range, "0,100,or_greater")]
    private float _segmentLength;

    [Export] private ConstraintStyle _constraintStyle;

    [Export] private ParticleBatchBuilder2D _particleBatchBuilder2D;

    private enum ConstraintStyle
    {
        Sequential,
        Parallel
    }

    public UniformChainedMaxDistanceConstraint2D()
    {
    }

    public UniformChainedMaxDistanceConstraint2D(float segmentLength) => _segmentLength = segmentLength;

    public override void Constrain()
    {
        Vector2[] positions = _particleBatchBuilder2D.ParticleBatch2D.Positions;
        float[] inverseMasses = _particleBatchBuilder2D.ParticleBatch2D.InverseMasses;

        switch (_constraintStyle)
        {
            case ConstraintStyle.Sequential:
                SequentialConstrain(positions, inverseMasses);
                break;
            case ConstraintStyle.Parallel:
                ParallelConstrain(positions, inverseMasses);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SequentialConstrain(Vector2[] positions, float[] inverseMasses)
    {
        for (int i = 0; i < positions.Length - 1; i++)
        {
            Vector2 p1 = positions[i];
            Vector2 p2 = positions[i + 1];

            float distance = p1.DistanceTo(p2);

            float constraintError = distance - _segmentLength;
            if (constraintError <= 0f) continue;

            float w1 = inverseMasses[i];
            float w2 = inverseMasses[i + 1];

            Vector2 gradient1 = (p1 - p2) / distance;
            // var gradient2 = -gradient1;

            float lambda = -constraintError / (w1 + w2);

            Vector2 unweightedDelta1 = lambda * gradient1;
            Vector2 d1 = w1 * unweightedDelta1;
            Vector2 d2 = w2 * -unweightedDelta1;

            positions[i] += d1;
            positions[i + 1] += d2;
        }
    }

    private void ParallelConstrain(Vector2[] positions, float[] inverseMasses)
    {
        Vector2[] newPositions = new Vector2[_particleBatchBuilder2D.ParticleBatch2D.Positions.Length];

        int segments = positions.Length - 1;
        for (int i = 0; i < segments; i++)
        {
            Vector2 p1 = positions[i];
            Vector2 p2 = positions[i + 1];

            float distance = p1.DistanceTo(p2);

            float constraintError = distance - _segmentLength;
            if (constraintError <= 0f)
            {
                newPositions[i] += p1;
                newPositions[i + 1] += p2;
            }
            else
            {
                float w1 = inverseMasses[i];
                float w2 = inverseMasses[i + 1];

                Vector2 gradient1 = (p1 - p2) / distance;
                // var gradient2 = -gradient1;

                float lambda = -constraintError / (w1 + w2);

                Vector2 unweightedDelta1 = lambda * gradient1;
                Vector2 d1 = w1 * unweightedDelta1;
                Vector2 d2 = w2 * -unweightedDelta1;

                newPositions[i] += p1 + d1;
                newPositions[i + 1] += p2 + d2;
            }
        }

        positions[0] = newPositions[0];
        positions[segments] = newPositions[segments];

        for (int i = 1; i < segments; i++)
        {
            positions[i] = 0.5f * newPositions[i];
        }
    }
}
