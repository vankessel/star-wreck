using Godot;
using StarWreck.scripts.xpbd.constraints;
using StarWreck.scripts.xpbd.particles;
using StarWreck.scripts.xpbd.vector_fields;

namespace StarWreck.scripts.xpbd.solvers;

[GlobalClass]
public partial class Solver3 : BaseSolver3
{
    protected override void PredictPositions(float delta, Particle3Batch particle3Batch, VectorField<Vector3> vectorField)
    {
        var positions = particle3Batch.Positions;
        var predictedPositions = particle3Batch.PredictedPositions;
        var halfStepPrevVelocities = particle3Batch.HalfStepPrevVelocities;
        for (int i = 0; i < particle3Batch.ParticleCount; i++)
        {
            Vector3 accel = vectorField.Sample(positions[i]);
            predictedPositions[i] = positions[i] + (halfStepPrevVelocities[i] + accel * delta) * delta;
        }
    }

    protected override void ApplyConstraints(Particle3Batch particle3Batch, BaseConstraint3 constraint3)
    {
        constraint3.Constrain(particle3Batch);
    }

    protected override void IntegrateVelocities(float delta, Particle3Batch particle3Batch)
    {
        var positions = particle3Batch.Positions;
        var predictedPositions = particle3Batch.PredictedPositions;
        var halfStepPrevVelocities = particle3Batch.HalfStepPrevVelocities;
        for (int i = 0; i < particle3Batch.ParticleCount; i++)
        {
            halfStepPrevVelocities[i] = (predictedPositions[i] - positions[i]) / delta;
            positions[i] = predictedPositions[i];
        }
    }
}