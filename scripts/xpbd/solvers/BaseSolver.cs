using Godot;
using StarWreck.scripts.xpbd.constraints;
using StarWreck.scripts.xpbd.particles;
using StarWreck.scripts.xpbd.vector_fields;

namespace StarWreck.scripts.xpbd.solvers;

public abstract partial class BaseSolver<T> : Resource
{
    protected abstract void PredictPositions(float delta, BaseParticleBatch<T> particleBatch, BaseVectorField<T> vectorField);
    protected abstract void ApplyConstraints(BaseParticleBatch<T> particleBatch, BaseConstraint<T> constraint);
    protected abstract void IntegrateVelocities(float delta, BaseParticleBatch<T> particleBatch);
    
    public virtual void Update(float delta, BaseParticleBatch<T> particleBatch, BaseVectorField<T> vectorField, BaseConstraint<T> constraint)
    {
        PredictPositions(delta, particleBatch, vectorField);
        ApplyConstraints(particleBatch, constraint);
        IntegrateVelocities(delta, particleBatch);
    }
}