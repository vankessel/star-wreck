using Godot;
using StarWreck.scripts.xpbd.constraints;
using StarWreck.scripts.xpbd.particles;
using StarWreck.scripts.xpbd.solvers;
using StarWreck.scripts.xpbd.vector_fields;

namespace StarWreck.scripts.xpbd;

public partial class Rope2D : Node2D
{
    [Export(PropertyHint.Range, "0,64,or_greater")]
    private float _length;

    [Export(PropertyHint.Range, "2,64,or_greater")]
    private int _particleCount;

    [Export(PropertyHint.ResourceType, nameof(ParticleBatch2D))]
    private BaseParticleBatchBuilder<Vector2> _particleBatchBuilder2D;

    private BaseParticleBatch<Vector2> _particleBatch2D;

    [Export(PropertyHint.ResourceType, nameof(VectorField2D))]
    private BaseVectorField<Vector2> _vectorField2D;

    [Export(PropertyHint.ResourceType, nameof(Constraint2D))]
    private BaseConstraint<Vector2> _constraint2D;

    [Export(PropertyHint.ResourceType, nameof(Solver2D))]
    private BaseSolver<Vector2> _solver2D;

    public override void _EnterTree()
    {
        base._EnterTree();

        _particleBatch2D = _particleBatchBuilder2D.Build();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        _solver2D.Update((float)delta, _particleBatch2D, _vectorField2D, _constraint2D);
    }
}
