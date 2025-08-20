using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public partial class UniformChainDistanceBaseConstraint2D : BaseConstraint<Vector2>
{
    [Export(PropertyHint.Range, "0,100,or_greater")]
    private float _restDistance;
    
    public UniformChainDistanceBaseConstraint2D() {}
    
    public UniformChainDistanceBaseConstraint2D(float restDistance) => _restDistance = restDistance;

    public override void Constrain(BaseParticleBatch<Vector2> particle2Batch)
    {
        Vector2[] positions = particle2Batch.Positions;
        float[] inverseMasses = particle2Batch.InverseMasses;
        
        for (int i = 0; i < positions.Length - 1; i++)
        {
            Vector2 p1 = positions[i];
            Vector2 p2 = positions[i + 1];
            float w1 = inverseMasses[i];
            float w2 = inverseMasses[i + 1];
            
            float distance = p1.DistanceTo(p2);
            float constraintError = distance - _restDistance;
            Vector2 gradient1 = (p1 - p2) / distance;
            // var gradient2 = -gradient1;

            float lambda = -constraintError / (w1 + w2);

            Vector2 unweightedDelta = lambda * gradient1;
            Vector2 d1 = w1 * unweightedDelta;
            Vector2 d2 = -w2 * unweightedDelta;

            positions[i] += d1;
            positions[i + 1] += d2;
        }
    }
}