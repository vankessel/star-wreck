using Godot;
using StarWreck.scripts.xpbd.constraints;
using StarWreck.scripts.xpbd.particles;
using StarWreck.scripts.xpbd.vector_fields;

namespace StarWreck.scripts.xpbd.solvers;

public abstract partial class BaseSolver2 : Resource
{
    protected abstract void PredictPositions(float delta, Particle2Batch particle2Batch, VectorField<Vector2> vectorField);
    protected abstract void ApplyConstraints(Particle2Batch particle2Batch, BaseConstraint2 constraint2);
    protected abstract void IntegrateVelocities(float delta, Particle2Batch particle2Batch);
    
    public virtual void Update(float delta, Particle2Batch particle2Batch, VectorField<Vector2> vectorField, BaseConstraint2 constraint2)
    {
        this.PredictPositions(delta, particle2Batch, vectorField);
        this.ApplyConstraints(particle2Batch, constraint2);
        this.IntegrateVelocities(delta, particle2Batch);
    }
}