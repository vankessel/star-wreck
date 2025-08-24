using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd;

public partial class ParticleRigidBody2D : RigidBody2D
{
    [Export]
    private Node2D _anchorPoint;

    [Export]
    private Particle2D _particle2D;

    private PhysicsDirectBodyState2D _directState;

    public override void _Ready()
    {
        base._Ready();
        _directState = PhysicsServer2D.BodyGetDirectState(GetRid());
        CustomIntegrator = true;
        _particle2D.InverseMass = _directState.InverseMass;
        _particle2D.Position = _anchorPoint.GlobalPosition * _particle2D.SpaceGlobalTransform;

    }

    /// <summary>
    /// While the dynamics of the whole body (or the point at the center of mass) can typically be calculated using the usual mass,
    /// the dynamics of a particular point on the body may be different. <br/>
    /// This calculates the effective mass for that point. <br/>
    /// https://physics.stackexchange.com/a/669262 <br/>
    /// https://matthias-research.github.io/pages/tenMinutePhysics/22-rigidBodies.pdf (Slide 20)
    /// </summary>
    /// <param name="distanceFromCenterOfMass"></param>
    /// <returns></returns>
    private float EffectiveInverseMass(float distanceFromCenterOfMass)
    {
        return _directState.InverseMass + distanceFromCenterOfMass * distanceFromCenterOfMass * _directState.InverseInertia;
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        base._IntegrateForces(state);

        Vector2 delta = _particle2D.Position - _anchorPoint.GlobalPosition * _particle2D.SpaceGlobalTransform;
        Vector2 offset = _anchorPoint.GlobalPosition - GlobalPosition;
        ApplyImpulse(delta, offset);
    }
}
