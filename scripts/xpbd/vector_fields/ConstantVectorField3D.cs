using Godot;

namespace StarWreck.scripts.xpbd.vector_fields;

[GlobalClass]
public partial class ConstantVectorField3D : VectorField3D
{
    [Export] private Vector3 _constant;

    public override Vector3 Sample(Vector3 position)
    {
        return _constant;
    }
}
