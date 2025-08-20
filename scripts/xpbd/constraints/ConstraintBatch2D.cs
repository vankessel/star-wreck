using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public partial class ConstraintBatch2D : Constraint2D
{
    [Export]
    private BaseConstraint<Vector2>[] _constraints;
    
    public override void Constrain(BaseParticleBatch<Vector2> particleBatch)
    {
        foreach (BaseConstraint<Vector2> constraint in _constraints)
        {
            constraint.Constrain(particleBatch);
        }
    }
}