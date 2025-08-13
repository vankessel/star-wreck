using Godot;

namespace StarWreck.scripts.xpbd.vector_fields;

public partial class ConstantVectorField : VectorField
{
    [Export] private Vector3 _constant;
    
    public override Vector3 Sample(Vector3 position)
    {
        return this._constant;
    }
}