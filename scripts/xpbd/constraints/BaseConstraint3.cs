using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public abstract partial class BaseConstraint3 : Resource
{
    public abstract void Constrain(Particle3Batch particle3Batch);
}