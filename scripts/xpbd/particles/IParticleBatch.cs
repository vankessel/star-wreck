using Godot;

namespace StarWreck.scripts.xpbd.particles;

public interface IParticleBatch
{
    int ParticleCount { get; }

    public float[] InverseMasses { get; }
    public Vector3[] Positions { get; }
    public Vector3[] HalfStepPrevVelocities { get; }
}
