using Godot;
using StarWreck.scripts.xpbd.constraints;
using StarWreck.scripts.xpbd.particles;
using StarWreck.scripts.xpbd.vector_fields;

namespace StarWreck.scripts.xpbd.solvers;

public abstract partial class BaseSolver3 : Resource
{
    protected abstract void PredictPositions(float delta, Particle3Batch particle3Batch, VectorField<Vector3> vectorField);
    protected abstract void ApplyConstraints(Particle3Batch particle3Batch, BaseConstraint3 constraint3);
    protected abstract void IntegrateVelocities(float delta, Particle3Batch particle3Batch);
    
    public virtual void Update(float delta, Particle3Batch particle3Batch, VectorField<Vector3> vectorField, BaseConstraint3 constraint3)
    {
        this.PredictPositions(delta, particle3Batch, vectorField);
        this.ApplyConstraints(particle3Batch, constraint3);
        this.IntegrateVelocities(delta, particle3Batch);
    }
}