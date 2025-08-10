using Godot;

namespace StarWreck.scripts.xpbd.particles;

public readonly partial struct ParticleBatch(int particleCount) : IParticleBatch
{
    public int ParticleCount { get; } = particleCount;

    public float[] ParticleInverseMasses { get; } = new float[particleCount];

    public Vector3[] ParticlePositions { get; } = new Vector3[particleCount];

    public Vector3[] ParticleVelocities { get; } = new Vector3[particleCount];

    public Vector3 ParticleInverseMass(int index)
    {
        throw new System.NotImplementedException();
    }

    public Vector3 ParticlePosition(int index)
    {
        throw new System.NotImplementedException();
    }

    public Vector3 ParticleVelocity(int index)
    {
        throw new System.NotImplementedException();
    }

    public void Update(double delta)
    {
        this.PredictPositions();
        this.ApplyConstraints();
        this.IntegrateVelocities();
    }

    public void PredictPositions()
    {
        throw new System.NotImplementedException();
    }

    public void ApplyConstraints()
    {
        throw new System.NotImplementedException();
    }

    public void IntegrateVelocities()
    {
        throw new System.NotImplementedException();
    }
}