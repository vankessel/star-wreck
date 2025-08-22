using Godot;
using StarWreck.scripts.xpbd.constraints;
using StarWreck.scripts.xpbd.particles;
using StarWreck.scripts.xpbd.vector_fields;

namespace StarWreck.scripts.xpbd.solvers;

[GlobalClass]
public partial class Solver2D : BaseSolver<Vector2>
{
    protected override void PredictPositions(float delta, BaseParticleBatch<Vector2> particleBatch, BaseVectorField<Vector2> vectorField)
    {
        Vector2[] positions = particleBatch.Positions;
        Vector2[] predictedPositions = particleBatch.PredictedPositions;
        Vector2[] halfStepPrevVelocities = particleBatch.HalfStepPrevVelocities;
        for (int i = 0; i < particleBatch.ParticleCount; i++)
        {
            Vector2 accel = vectorField.Sample(positions[i]);
            predictedPositions[i] = positions[i] + (halfStepPrevVelocities[i] + accel * delta) * delta;
        }
    }

    protected override void ApplyConstraints(BaseParticleBatch<Vector2> particleBatch, BaseConstraint<Vector2> constraint2D)
    {
        constraint2D.Constrain(particleBatch);
    }

    protected override void IntegrateVelocities(float delta, BaseParticleBatch<Vector2> particleBatch)
    {
        Vector2[] positions = particleBatch.Positions;
        Vector2[] predictedPositions = particleBatch.PredictedPositions;
        Vector2[] halfStepPrevVelocities = particleBatch.HalfStepPrevVelocities;
        for (int i = 0; i < particleBatch.ParticleCount; i++)
        {
            halfStepPrevVelocities[i] = (predictedPositions[i] - positions[i]) / delta;
            positions[i] = predictedPositions[i];
        }
    }
}
