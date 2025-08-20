using Godot;

namespace StarWreck.scripts.xpbd.particles;

[GlobalClass]
public partial class ParticleBatch2D(int particleCount) : BaseParticleBatch<Vector2>
{
    public override int ParticleCount { get; } = particleCount;
    public override float[] InverseMasses { get; } = new float[particleCount];
    public override Vector2[] Positions { get; } = new Vector2[particleCount];
    public override Vector2[] PredictedPositions { get; } = new Vector2[particleCount];
    public override Vector2[] HalfStepPrevVelocities { get; } = new Vector2[particleCount];
}