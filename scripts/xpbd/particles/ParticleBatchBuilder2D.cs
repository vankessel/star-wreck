using Godot;

namespace StarWreck.scripts.xpbd.particles;

[GlobalClass]
public partial class ParticleBatchBuilder2D : BaseParticleBatchBuilder<Vector2>
{
    [Export] private Transform2D _spaceGlobalTransform;

    [Export] private Vector2 _startPosition;

    [Export] private Vector2 _endPosition;

    [Export(PropertyHint.Range, "2")] private int _count;

    [Export(PropertyHint.Range, "0,10,or_greater")] private float _inverseMass;

    public override ParticleBatch2D Build()
    {
        ParticleBatch2D particleBatch2D = new(_count, _spaceGlobalTransform);

        Vector2 delta = _endPosition - _startPosition;
        float fraction = 1f / (_count - 1);

        for (int i = 0; i < _count; i++)
        {
            Vector2 nextPosition = _startPosition + delta * (i * fraction);
            particleBatch2D.Positions[i] = nextPosition;
        }

        for (int i = 0; i < _count; i++)
        {
            particleBatch2D.InverseMasses[i] = _inverseMass;
        }

        return particleBatch2D;
    }
}
