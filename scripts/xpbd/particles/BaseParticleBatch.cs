using Godot;

namespace StarWreck.scripts.xpbd.particles;

public abstract partial class BaseParticleBatch<T> : Resource
{
    public abstract int ParticleCount { get; }

    public abstract float[] InverseMasses { get; }
    public abstract T[] Positions { get; }
    public abstract T[] PredictedPositions { get; }

    /// <summary>
    /// <para>
    ///     XPBD is similar to Verlet integration. That means these velocities are a half-step behind.
    /// </para>
    ///     v_(n-1/2) = ( x_n - x_(n-1) ) / dt
    /// </summary>
    public abstract T[] HalfStepPrevVelocities { get; }
}