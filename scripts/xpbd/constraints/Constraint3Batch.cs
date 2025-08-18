using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public partial class Constraint3Batch : BaseConstraint3
{
    [Export]
    private BaseConstraint3[] _constraints;
    
    public override void Constrain(Particle3Batch particle3Batch)
    {
        foreach (BaseConstraint3 constraint in this._constraints)
        {
            constraint.Constrain(particle3Batch);
        }
    }
}