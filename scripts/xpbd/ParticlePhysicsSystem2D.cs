using Godot;
using StarWreck.scripts.xpbd.constraints;
using StarWreck.scripts.xpbd.particles;
using StarWreck.scripts.xpbd.solvers;
using StarWreck.scripts.xpbd.vector_fields;

namespace StarWreck.scripts.xpbd;

public partial class ParticlePhysicsSystem2D : Node2D
{
    [Export(PropertyHint.Range, "0,5,or_greater")]
    private int _substeps;

    [Export] private ParticleBatchBuilder2D _particleBatchBuilder2D;

    private ParticleBatch2D _particleBatch2D;

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
        _particleBatch2D.SpaceGlobalTransform = GlobalTransform;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        float dt = (float)(delta / _substeps);

        for (int i = 0; i < _substeps; i++)
        {
            _solver2D.Update(dt, _particleBatch2D, _vectorField2D, _constraint2D);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        QueueRedraw();
    }

    public override void _Draw()
    {
        base._Draw();

        Vector2[] positions = _particleBatch2D.Positions;
        for (int index = 0; index < positions.Length; index++)
        {
            DrawCircle(positions[index], 5f, Colors.Red);
            if (index == positions.Length - 1) return;
            DrawLine(positions[index], positions[index + 1], Colors.Red);
        }
    }
}
