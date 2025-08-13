using Godot;

namespace StarWreck.scripts.xpbd.particles;

public partial class ParticleBatch(int particleCount) : IParticleBatch
{
    public int ParticleCount { get; } = particleCount;

    public float[] InverseMasses { get; } = new float[particleCount];
    public Vector3[] Positions { get; } = new Vector3[particleCount];
    public Vector3[] PredictedPositions { get; } = new Vector3[particleCount];
    // v_(n-1/2) * dt = x_(n) - x_(n-1)
    public Vector3[] HalfStepPrevVelocities { get; } = new Vector3[particleCount];
}