using Godot;

namespace StarWreck.scripts.xpbd.particles;

public partial class ParticleBatch2D(int particleCount, Transform2D spaceGlobalTransform) : BaseParticleBatch<Vector2>
{
    public Transform2D SpaceGlobalTransform { get; set; } = spaceGlobalTransform;
    public override int ParticleCount { get; } = particleCount;
    public override float[] InverseMasses { get; } = new float[particleCount];
    public override Vector2[] Positions { get; } = new Vector2[particleCount];
    public override Vector2[] PreviousPositions { get; } = new Vector2[particleCount];
    public override Vector2[] HalfStepPreviousVelocities { get; } = new Vector2[particleCount];
}
