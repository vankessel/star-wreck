using Godot;

namespace StarWreck.scripts.xpbd.particles;

public abstract partial class BaseParticleBatchBuilder<T> : Resource
{
    public abstract BaseParticleBatch<T> Build();
}