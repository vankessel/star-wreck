using Godot;

namespace StarWreck.scripts.xpbd.particles;

[GlobalClass]
public partial class ParticleBatchBuilder2D : BaseParticleBatchBuilder<Vector2>
{
    [Export]
    private Vector3 _startPosition;
    
    [Export]
    private Vector3 _endPosition;
    
    [Export]
    private int _count;
    
    public override BaseParticleBatch<Vector2> Build()
    {
        throw new System.NotImplementedException();
    }
}