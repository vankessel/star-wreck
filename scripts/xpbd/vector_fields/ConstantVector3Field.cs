using Godot;

namespace StarWreck.scripts.xpbd.vector_fields;

[GlobalClass]
public partial class ConstantVector3Field : VectorField<Vector3>
{
    [Export] private Vector3 _constant;
    
    public override Vector3 Sample(Vector3 position)
    {
        return this._constant;
    }
}