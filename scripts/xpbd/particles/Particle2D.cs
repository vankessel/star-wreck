using Godot;

namespace StarWreck.scripts.xpbd.particles;

[GlobalClass]
public partial class Particle2D : Resource
{
    [Export] public ParticleBatch2D ParticleBatch2D { get; protected set; }

    private int _particleIndex;

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

    public Transform2D SpaceGlobalTransform
    {
        get => ParticleBatch2D.SpaceGlobalTransform;
    }

    [Export]
    public int ParticleIndex
    {
        get => _particleIndex;
        set => _particleIndex = Mathf.PosMod(value, ParticleBatch2D.ParticleCount);
    }

    public Particle2D()
    {
    }

    public Particle2D(ParticleBatch2D particleBatch2D, int particleIndex)
    {
        ParticleBatch2D = particleBatch2D;
        _particleIndex = particleIndex;
    }
}
