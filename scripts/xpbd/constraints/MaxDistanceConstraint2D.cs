using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public partial class MaxDistanceConstraint2D : Constraint2D
{
    [Export(PropertyHint.Range, "0,100,or_greater")]
    private float _segmentLength;

    [Export]
    private BaseParticleBatch<Vector2> _particleBatch2D;

    [Export(PropertyHint.Range, "0")]
    private int _particle1Index;

    [Export(PropertyHint.Range, "0")]
    private int _particle2Index;

    public MaxDistanceConstraint2D() {}

    public MaxDistanceConstraint2D(float segmentLength) => _segmentLength = segmentLength;

    public override void Constrain()
    {
        Vector2[] positions = _particleBatch2D.Positions;
        float[] inverseMasses = _particleBatch2D.InverseMasses;

        Vector2 p1 = positions[_particle1Index];
        Vector2 p2 = positions[_particle2Index];

        float distance = p1.DistanceTo(p2);

        float constraintError = distance - _segmentLength;
        if (constraintError <= 0f) return;

        float w1 = inverseMasses[_particle1Index];
        float w2 = inverseMasses[_particle2Index];

        Vector2 gradient1 = (p1 - p2) / distance;
        // var gradient2 = -gradient1;

        float lambda = -constraintError / (w1 + w2);

        Vector2 unweightedDelta1 = lambda * gradient1;
        Vector2 d1 = w1 * unweightedDelta1;
        Vector2 d2 = w2 * -unweightedDelta1;

        positions[_particle1Index] += d1;
        positions[_particle2Index] += d2;
    }
}
