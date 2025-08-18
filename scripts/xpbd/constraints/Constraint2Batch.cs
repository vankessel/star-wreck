using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public partial class Constraint2Batch : BaseConstraint2
{
    [Export]
    private BaseConstraint2[] _constraints;
    
    public override void Constrain(Particle2Batch particle2Batch)
    {
        foreach (BaseConstraint2 constraint in this._constraints)
        {
            constraint.Constrain(particle2Batch);
        }
    }
}