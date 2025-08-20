using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public partial class UniformChainedMaxDistanceConstraint2D : Constraint2D
{
    [Export(PropertyHint.Range, "0,100,or_greater")]
    private float _segmentLength;
    
    public UniformChainedMaxDistanceConstraint2D() {}
    
    public UniformChainedMaxDistanceConstraint2D(float segmentLength) => _segmentLength = segmentLength;

    public override void Constrain(BaseParticleBatch<Vector2> particle2Batch)
    {
        Vector2[] positions = particle2Batch.Positions;
        float[] inverseMasses = particle2Batch.InverseMasses;

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
}
