using Godot;
using StarWreck.scripts.xpbd.constraints;
using StarWreck.scripts.xpbd.particles;
using StarWreck.scripts.xpbd.vector_fields;

namespace StarWreck.scripts.xpbd.solvers;

[GlobalClass]
public partial class Solver : BaseSolver
{
    protected override void PredictPositions(float delta, ParticleBatch particleBatch, VectorField vectorField)
    {
        var positions = particleBatch.Positions;
        var predictedPositions = particleBatch.PredictedPositions;
        var halfStepPrevVelocities = particleBatch.HalfStepPrevVelocities;
        for (int i = 0; i < particleBatch.ParticleCount; i++)
        {
            Vector3 accel = vectorField.Sample(positions[i]);
            predictedPositions[i] = positions[i] + (halfStepPrevVelocities[i] + accel * delta) * delta;
        }
    }

    protected override void ApplyConstraints(ParticleBatch particleBatch, Constraint constraint)
    {
        constraint.Constrain(particleBatch);
    }

    protected override void IntegrateVelocities(float delta, ParticleBatch particleBatch)
    {
        var positions = particleBatch.Positions;
        var predictedPositions = particleBatch.PredictedPositions;
        var halfStepPrevVelocities = particleBatch.HalfStepPrevVelocities;
        for (int i = 0; i < particleBatch.ParticleCount; i++)
        {
            halfStepPrevVelocities[i] = (predictedPositions[i] - positions[i]) / delta;
            positions[i] = predictedPositions[i];
        }
    }
}