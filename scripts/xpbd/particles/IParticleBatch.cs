using Godot;

namespace StarWreck.scripts.xpbd.particles;

public interface IParticleBatch
{
    int ParticleCount { get; }

    public float[] ParticleInverseMasses { get; }
    public Vector3[] ParticlePositions { get; }
    public Vector3[] ParticleVelocities { get; }
    
    public Vector3 ParticleInverseMass(int index);
    public Vector3 ParticlePosition(int index);
    public Vector3 ParticleVelocity(int index);

    public void PredictPositions();
    public void ApplyConstraints();
    public void IntegrateVelocities();
    
    public void Update(double delta)
    {
        this.PredictPositions();
        this.ApplyConstraints();
        this.IntegrateVelocities();
    }
}
