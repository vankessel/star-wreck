using Godot;

namespace StarWreck.scripts.xpbd.vector_fields;

public abstract partial class VectorField<T> : Resource
{
    public abstract T Sample(T position);
}