using Godot;
using StarWreck.scripts.xpbd.constraints;
using StarWreck.scripts.xpbd.particles;
using StarWreck.scripts.xpbd.vector_fields;

namespace StarWreck.scripts.xpbd.solvers;

[GlobalClass]
public partial class Solver3D : BaseSolver<Vector3>
{
    protected override void PredictPositions(float delta, BaseParticleBatch<Vector3> particleBatch, BaseVectorField<Vector3> vectorField)
    {
        Vector3[] positions = particleBatch.Positions;
        Vector3[] predictedPositions = particleBatch.PredictedPositions;
        Vector3[] halfStepPrevVelocities = particleBatch.HalfStepPrevVelocities;
        for (int i = 0; i < particleBatch.ParticleCount; i++)
        {
            Vector3 accel = vectorField.Sample(positions[i]);
            predictedPositions[i] = positions[i] + (halfStepPrevVelocities[i] + accel * delta) * delta;
        }
    }

    protected override void ApplyConstraints(BaseParticleBatch<Vector3> particleBatch, BaseConstraint<Vector3> constraint3)
    {
        constraint3.Constrain(particleBatch);
    }

    protected override void IntegrateVelocities(float delta, BaseParticleBatch<Vector3> particleBatch)
    {
        Vector3[] positions = particleBatch.Positions;
        Vector3[] predictedPositions = particleBatch.PredictedPositions;
        Vector3[] halfStepPrevVelocities = particleBatch.HalfStepPrevVelocities;
        for (int i = 0; i < particleBatch.ParticleCount; i++)
        {
            halfStepPrevVelocities[i] = (predictedPositions[i] - positions[i]) / delta;
            positions[i] = predictedPositions[i];
        }
    }
}
