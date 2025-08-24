using Godot;

namespace StarWreck.scripts.xpbd.particles;

public partial class ParticleBatch3D(int particleCount) : BaseParticleBatch<Vector3>
{
    public override int ParticleCount { get; } = particleCount;
    public override float[] InverseMasses { get; } = new float[particleCount];
    public override Vector3[] Positions { get; } = new Vector3[particleCount];
    public override Vector3[] PreviousPositions { get; } = new Vector3[particleCount];
    public override Vector3[] HalfStepPreviousVelocities { get; } = new Vector3[particleCount];
}
