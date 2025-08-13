using Godot;
using StarWreck.scripts.xpbd.constraints;
using StarWreck.scripts.xpbd.particles;
using StarWreck.scripts.xpbd.vector_fields;

namespace StarWreck.scripts.xpbd.solvers;

public abstract partial class BaseSolver : Resource
{
    protected abstract void PredictPositions(float delta, ParticleBatch particleBatch, VectorField vectorField);
    protected abstract void ApplyConstraints(ParticleBatch particleBatch, Constraint constraint);
    protected abstract void IntegrateVelocities(float delta, ParticleBatch particleBatch);
    
    public virtual void Update(float delta, ParticleBatch particleBatch, VectorField vectorField, Constraint constraint)
    {
        this.PredictPositions(delta, particleBatch, vectorField);
        this.ApplyConstraints(particleBatch, constraint);
        this.IntegrateVelocities(delta, particleBatch);
    }
}