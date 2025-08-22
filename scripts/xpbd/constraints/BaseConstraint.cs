using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

public abstract partial class BaseConstraint<T> : Resource
{
    public abstract void Constrain();
}
