using Godot;
using StarWreck.scripts.xpbd.constraints;
using StarWreck.scripts.xpbd.particles;
using StarWreck.scripts.xpbd.solvers;

namespace StarWreck.scripts.xpbd;

public partial class Rope : Node3D
{
    [Export(PropertyHint.Range, "0,64,or_greater")]
    private float _length;
    
    [Export(PropertyHint.Range, "1,64,or_greater")]
    private int _particleCount;
    
    private ParticleBatch _particleBatch;
    
    private Constraint _constraint;

    private Solver _solver;

    public override void _EnterTree()
    {
        base._EnterTree();

        this._particleBatch = new ParticleBatch(this._particleCount);
        this._constraint = new UniformChainDistanceConstraint(this._length / (this._particleCount - 1));
        this._solver = new Solver();
    }

    public override void _Ready()
    {
        base._Ready();
    }
}