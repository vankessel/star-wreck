using Godot;

namespace StarWreck.scripts.xpbd.particles;

[GlobalClass]
public partial class Particle2D : Resource
{
    [Export]
    private ParticleBatchBuilder2D _particleBatchBuilder2D;

    private ParticleBatch2D ParticleBatch2D => _particleBatchBuilder2D.ParticleBatch2D;

    [Export]
    private int _particleIndex;

    private int ParticleIndex
    {
        get => Mathf.PosMod(_particleIndex, ParticleBatch2D.ParticleCount);
        set => _particleIndex = value;
    }

    public float InverseMass
    {
        get => ParticleBatch2D.InverseMasses[ParticleIndex];
        set => ParticleBatch2D.InverseMasses[ParticleIndex] = value;
    }

    public Vector2 Position
    {
        get => ParticleBatch2D.Positions[ParticleIndex];
        set => ParticleBatch2D.Positions[ParticleIndex] = value;
    }

    public Vector2 PreviousPosition
    {
        get => ParticleBatch2D.PreviousPositions[ParticleIndex];
        set => ParticleBatch2D.PreviousPositions[ParticleIndex] = value;
    }

    public Vector2 HalfStepPreviousVelocity
    {
        get => ParticleBatch2D.HalfStepPreviousVelocities[ParticleIndex];
        set => ParticleBatch2D.HalfStepPreviousVelocities[ParticleIndex] = value;
    }

    public Transform2D SpaceGlobalTransform => ParticleBatch2D.SpaceGlobalTransform;

    public Particle2D()
    {
    }

    public Particle2D(ParticleBatchBuilder2D particleBatchBuilder2D2D, int particleIndex)
    {
        _particleBatchBuilder2D = particleBatchBuilder2D2D;
        _particleIndex = particleIndex;
    }
}
