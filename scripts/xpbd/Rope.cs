using Godot;
using StarWreck.scripts.xpbd.constraints;
using StarWreck.scripts.xpbd.particles;
using StarWreck.scripts.xpbd.solvers;
using StarWreck.scripts.xpbd.vector_fields;

namespace StarWreck.scripts.xpbd;

public partial class Rope : Node3D
{
    [Export(PropertyHint.Range, "0,64,or_greater")]
    private float _length;
    
    [Export(PropertyHint.Range, "1,64,or_greater")]
    private int _particleCount;
    
    private Particle3Batch _particle3Batch;
    private VectorField<Vector3> _vectorField;
    private BaseConstraint3 _constraint3;
    private Solver3 _solver3;

    public override void _EnterTree()
    {
        base._EnterTree();

        this._particle3Batch = new Particle3Batch(this._particleCount);
        this._constraint3 = new UniformChainDistanceBaseConstraint3(this._length / (this._particleCount - 1));
        this._solver3 = new Solver3();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        this._solver3.Update((float) delta, this._particle3Batch, this._vectorField, this._constraint3);
    }
}