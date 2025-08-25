using Godot;
using StarWreck.scripts.xpbd.particles;

namespace StarWreck.scripts.xpbd;

public partial class ParticleRigidBody2D : RigidBody2D
{
    [Export] private Node2D _anchorPoint;

    [Export] private Particle2D _particle2D;

    [Export(PropertyHint.Range, "0,10,or_greater")]
    private int _substeps = 1;

    private PhysicsDirectBodyState2D _directState;

    public override void _Ready()
    {
        base._Ready();
        _directState = PhysicsServer2D.BodyGetDirectState(GetRid());
        CustomIntegrator = false;
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
    /// <param name="forceOffset">Position of force relative to center of mass in global coordinates</param>
    /// <param name="forceDirection">Force direction unit vector in global basis</param>
    /// <returns></returns>
    private float EffectiveInverseMass(Vector2 forceOffset, Vector2 forceDirection)
    {
        float unitTorque = forceOffset.Cross(forceDirection);
        return _directState.InverseMass + unitTorque * unitTorque * _directState.InverseInertia;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        // Seems to work here
        // Can't use _IntegrateForces because we need it running after ParticlePhysicsSystem2D's _PhysicsProcess
        UpdateParticles();
    }

    private void PinPosition()
    {
        _particle2D.Position = _anchorPoint.GlobalPosition * _particle2D.SpaceGlobalTransform;
        _particle2D.PreviousPosition = _particle2D.Position;
        QueueRedraw();
    }

    private void UpdateParticles()
    {
        // float dt = 1f / Engine.PhysicsTicksPerSecond;
        float dtInv = _substeps * Engine.PhysicsTicksPerSecond;

        Vector2 localPrevPos = _particle2D.PreviousPosition;

        for (int i = 0; i < _substeps; i++)
        {
            Vector2 delta = _particle2D.SpaceGlobalTransform * _particle2D.Position - _anchorPoint.GlobalPosition;
            Vector2 anchorPointCenterOfMassOffset = _anchorPoint.GlobalPosition - GlobalPosition;

            Vector2 deltaDir = delta.Normalized();
            float constraintError = delta.Length();

            float effectiveInverseMass = EffectiveInverseMass(anchorPointCenterOfMassOffset, deltaDir);
            float lambda = -constraintError / (_particle2D.InverseMass + effectiveInverseMass);

            float f1 = -lambda * effectiveInverseMass;
            float f2 = lambda * _particle2D.InverseMass;

            Vector2 d1 = f1 * deltaDir; // This
            Vector2 d2 = f2 * deltaDir; // Particle

            Position += d1;
            _particle2D.Position += d2;

            ApplyImpulse(d1 * dtInv, anchorPointCenterOfMassOffset);
            _particle2D.HalfStepPreviousVelocity = (_particle2D.Position - localPrevPos) * dtInv;

            localPrevPos = _particle2D.Position;
        }
        _particle2D.PreviousPosition = localPrevPos;

        QueueRedraw();
    }

    public override void _Draw()
    {
        base._Draw();

        DrawCircle(_anchorPoint.GlobalPosition * GlobalTransform, 5f, Colors.Green);
        DrawCircle((_particle2D.SpaceGlobalTransform * _particle2D.Position) * GlobalTransform, 10f, Colors.Purple);
    }
}
