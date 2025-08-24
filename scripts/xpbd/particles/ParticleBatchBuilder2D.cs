using Godot;

namespace StarWreck.scripts.xpbd.particles;

[GlobalClass]
public partial class ParticleBatchBuilder2D : BaseParticleBatchBuilder<Vector2>
{
    public ParticleBatch2D ParticleBatch2D { get; protected set; }

    private Transform2D _spaceGlobalTransform;

    [Export] private Vector2 _startPosition;

    [Export] private Vector2 _endPosition;

    [Export(PropertyHint.Range, "2,10,or_greater")] private int _count = 2;

    [Export(PropertyHint.Range, "0,10,or_greater")] private float _inverseMass = 1f;

    public override ParticleBatch2D Build()
    {
        ParticleBatch2D = new ParticleBatch2D(_count, _spaceGlobalTransform);

        Vector2 delta = _endPosition - _startPosition;
        float fraction = 1f / (_count - 1);

        for (int i = 0; i < _count; i++)
        {
            Vector2 nextPosition = _startPosition + delta * (i * fraction);
            ParticleBatch2D.Positions[i] = nextPosition;
        }

        for (int i = 0; i < _count; i++)
        {
            ParticleBatch2D.InverseMasses[i] = _inverseMass;
        }

        return ParticleBatch2D;
    }
}
