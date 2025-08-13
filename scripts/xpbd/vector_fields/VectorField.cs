using Godot;

namespace StarWreck.scripts.xpbd.vector_fields;

public abstract partial class VectorField : Resource
{
    public abstract Vector3 Sample(Vector3 position);
}