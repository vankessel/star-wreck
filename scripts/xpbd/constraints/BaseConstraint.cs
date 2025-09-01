using Godot;

namespace StarWreck.scripts.xpbd.constraints;

public abstract partial class BaseConstraint<T> : Resource
{
    public abstract void Constrain();
}
