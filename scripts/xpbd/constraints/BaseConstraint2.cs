using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public abstract partial class BaseConstraint2 : Resource
{
    public abstract void Constrain(Particle2Batch particle2Batch);
}