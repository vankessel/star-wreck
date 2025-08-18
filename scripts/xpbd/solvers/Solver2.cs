using Godot;
using StarWreck.scripts.xpbd.constraints;
using StarWreck.scripts.xpbd.particles;
using StarWreck.scripts.xpbd.vector_fields;

namespace StarWreck.scripts.xpbd.solvers;

[GlobalClass]
public partial class Solver2 : BaseSolver2
{
    protected override void PredictPositions(float delta, Particle2Batch particle2Batch, VectorField<Vector2> vectorField)
    {
        var positions = particle2Batch.Positions;
        var predictedPositions = particle2Batch.PredictedPositions;
        var halfStepPrevVelocities = particle2Batch.HalfStepPrevVelocities;
        for (int i = 0; i < particle2Batch.ParticleCount; i++)
        {
            Vector2 accel = vectorField.Sample(positions[i]);
            predictedPositions[i] = positions[i] + (halfStepPrevVelocities[i] + accel * delta) * delta;
        }
    }

    protected override void ApplyConstraints(Particle2Batch particle2Batch, BaseConstraint2 constraint2)
    {
        constraint2.Constrain(particle2Batch);
    }

    protected override void IntegrateVelocities(float delta, Particle2Batch particle2Batch)
    {
        var positions = particle2Batch.Positions;
        var predictedPositions = particle2Batch.PredictedPositions;
        var halfStepPrevVelocities = particle2Batch.HalfStepPrevVelocities;
        for (int i = 0; i < particle2Batch.ParticleCount; i++)
        {
            halfStepPrevVelocities[i] = (predictedPositions[i] - positions[i]) / delta;
            positions[i] = predictedPositions[i];
        }
    }
}