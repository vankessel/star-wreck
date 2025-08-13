using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public partial class UniformChainDistanceConstraint : Constraint
{
    [Export(PropertyHint.Range, "0,100,or_greater")]
    private float _restDistance;
    
    public UniformChainDistanceConstraint()
    {
    }
    
    public UniformChainDistanceConstraint(float restDistance)
    {
        this._restDistance = restDistance;
    }

    public override void Constrain(ParticleBatch particleBatch)
    {
        var positions = particleBatch.Positions;
        var inverseMasses = particleBatch.InverseMasses;
        
        for (int i = 0; i < positions.Length - 1; i++)
        {
            var p1 = positions[i];
            var p2 = positions[i + 1];
            var w1 = inverseMasses[i];
            var w2 = inverseMasses[i + 1];
            
            var distance = p1.DistanceTo(p2);
            var constraintError = distance - this._restDistance;
            var gradient1 = (p1 - p2) / distance;
            // var gradient2 = -gradient1;

            var lambda = -constraintError / (w1 + w2);

            var unweightedDelta = lambda * gradient1;
            var d1 = w1 * unweightedDelta;
            var d2 = -w2 * unweightedDelta;

            positions[i] += d1;
            positions[i + 1] += d2;
        }
    }
}