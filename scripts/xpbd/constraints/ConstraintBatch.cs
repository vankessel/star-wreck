using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public partial class ConstraintBatch : Constraint
{
    [Export]
    private Constraint[] _constraints;
    
    public override void Constrain(ParticleBatch particleBatch)
    {
        foreach (Constraint constraint in this._constraints)
        {
            constraint.Constrain(particleBatch);
        }
    }
}