using Godot;

namespace StarWreck.scripts.xpbd.vector_fields;

[GlobalClass]
public partial class ConstantVectorField2D : VectorField2D
{
    [Export] private Vector2 _constant;
    
    public override Vector2 Sample(Vector2 position)
    {
        return _constant;
    }
}