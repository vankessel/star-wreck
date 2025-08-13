using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd.constraints;

[GlobalClass]
public abstract partial class Constraint : Resource
{
    public abstract void Constrain(ParticleBatch particleBatch);
}