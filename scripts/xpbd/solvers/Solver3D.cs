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
        Vector3[] previousPositions = particleBatch.PreviousPositions;
        Vector3[] halfStepPrevVelocities = particleBatch.HalfStepPreviousVelocities;
        for (int i = 0; i < particleBatch.ParticleCount; i++)
        {
            Vector3 accel = vectorField.Sample(positions[i]);
            previousPositions[i] = positions[i];
            positions[i] += (halfStepPrevVelocities[i] + accel * delta) * delta;
        }
    }

    protected override void ApplyConstraints(BaseConstraint<Vector3> constraint3D)
    {
        constraint3D.Constrain();
    }

    protected override void IntegrateVelocities(float delta, BaseParticleBatch<Vector3> particleBatch)
    {
        Vector3[] positions = particleBatch.Positions;
        Vector3[] previousPositions = particleBatch.PreviousPositions;
        Vector3[] halfStepPrevVelocities = particleBatch.HalfStepPreviousVelocities;
        for (int i = 0; i < particleBatch.ParticleCount; i++)
        {
            halfStepPrevVelocities[i] = (positions[i] - previousPositions[i]) / delta;
        }
    }
}
