using Godot;

namespace StarWreck.scripts.xpbd.vector_fields;

[GlobalClass]
public partial class ConstantVector2Field : VectorField<Vector2>
{
    [Export] private Vector2 _constant;
    
    public override Vector2 Sample(Vector2 position)
    {
        return this._constant;
    }
}