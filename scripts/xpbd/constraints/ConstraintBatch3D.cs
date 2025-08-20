using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public partial class ConstraintBatch3D : BaseConstraint<Vector3>
{
    [Export]
    private BaseConstraint<Vector3>[] _constraints;
    
    public override void Constrain(BaseParticleBatch<Vector3> particleBatch)
    {
        foreach (BaseConstraint<Vector3> constraint in _constraints)
        {
            constraint.Constrain(particleBatch);
        }
    }
}