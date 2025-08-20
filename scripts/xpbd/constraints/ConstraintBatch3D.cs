using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public partial class ConstraintBatch3D : Constraint3D
{
    [Export(PropertyHint.ResourceType, nameof(Constraint3D))]
    private BaseConstraint<Vector3>[] _constraints;
    
    public override void Constrain(BaseParticleBatch<Vector3> particleBatch)
    {
        BaseConstraint<Vector3>[] constraints = _constraints;
        foreach (BaseConstraint<Vector3> constraint in constraints)
        {
            constraint.Constrain(particleBatch);
        }
    }
}