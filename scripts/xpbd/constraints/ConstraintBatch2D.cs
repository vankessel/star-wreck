using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public partial class ConstraintBatch2D : Constraint2D
{
    [Export(PropertyHint.ResourceType, nameof(Constraint2D))]
    private BaseConstraint<Vector2>[] _constraints;
    
    public override void Constrain(BaseParticleBatch<Vector2> particleBatch)
    {
        BaseConstraint<Vector2>[] constraints = _constraints;
        foreach (BaseConstraint<Vector2> constraint in constraints)
        {
            constraint.Constrain(particleBatch);
        }
    }
}