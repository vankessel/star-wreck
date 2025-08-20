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
    
    [Export(PropertyHint.Range, "1,64,or_greater")]
    private int _particleCount;
    
    [Export(PropertyHint.ResourceType, nameof(ParticleBatch2D))]
    private BaseParticleBatch<Vector2> _particleBatch2D;
    
    [Export(PropertyHint.ResourceType, nameof(VectorField2D))]
    private BaseVectorField<Vector2> _vectorField;
    
    [Export(PropertyHint.ResourceType, nameof(Constraint2D))]
    private BaseConstraint<Vector2> _constraint;
    private BaseSolver<Vector2> _solver2D;

    public override void _EnterTree()
    {
        base._EnterTree();

        _particleBatch2D = new ParticleBatch2D(_particleCount);
        _constraint = new UniformChainDistanceBaseConstraint2D(_length / (_particleCount - 1));
        _solver2D = new Solver2D();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        _solver2D.Update((float) delta, _particleBatch2D, _vectorField, _constraint);
    }
}