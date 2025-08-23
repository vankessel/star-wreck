using Godot;
using Action = StarWreck.scripts.input.Action;

namespace StarWreck.scripts;

public partial class Player : RigidBody2D
{
    [Export]
    private PlayerCamera _camera;

    [Export(PropertyHint.Range, "0,2048,16,or_greater")]
    private float _maxSpeed = 1024f;

    [Export(PropertyHint.Range, "0,4096,16,or_greater")]
    private float _maxAcceleration = 2048f;

    [Export(PropertyHint.Range, "0,3")] private int _movementType = 0;

    /// <summary>
    /// For movementType 0.
    /// Lower values of q give larger boost when changing direction.
    /// q > 0 and higher values give less boost and more closely approximate linear acceleration.
    /// q = 0 is like -log_2(x) shifted up and left 1 unit each.
    /// q from -2 to -1 approximates square root curve for acceleration with even larger boost
    /// </summary>
    [Export(PropertyHint.Range, "-2,2,1,or_greater,or_less")]
    private float _movementType0Factor = -2f;

    [Export(PropertyHint.Range, "0,1024,64,or_greater")]
    private float _friction = 256f;

    [Export(PropertyHint.Range, "0,10,or_greater")] private float _rotationSpringInertia = 1f;

    [Export(PropertyHint.Range, "0,256,or_greater")] private float _rotationSpringConstant = 32f;

    [Export(PropertyHint.Range, "0,2,or_greater")] private float _rotationDampingRatio = 0.3333333333f;

    public override void _Ready()
    {
        base._Ready();
        if (_camera == null) GD.PushWarning("Player camera not set.");
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        base._IntegrateForces(state);

        Vector2 force = MovementForce();
        ApplyCentralForce(force);

        float torque = CameraAlignmentTorque();
        ApplyTorque(torque);
    }

    private float CameraAlignmentTorque()
    {
        float inertia = (float)PhysicsServer2D.BodyGetParam(GetRid(), PhysicsServer2D.BodyParameter.Inertia);
        GD.Print(inertia);
        GD.Print(Mass);
        // Spring acceleration calculated with fake inertia
        float damping = _rotationDampingRatio * 2f * Mathf.Sqrt(_rotationSpringConstant / _rotationSpringInertia);
        float angleDelta = Transform[0].AngleTo(_camera.Transform[0]);
        // Multiply spring acceleration by real inertia to get force
        float torque = inertia * (_rotationSpringConstant / _rotationSpringInertia * angleDelta - damping * AngularVelocity);
        return torque;
    }

    private Vector2 MovementForce()
    {
        Vector2 movementInput = MovementInput();

        Vector2 force;
        if (movementInput == Vector2.Zero)
        {
            force = -Mass * _friction * LinearVelocity.Normalized();
        }
        else
        {
            // We want acceleration to slow to 0 as we approach max speed.
            // Must consider speed against attempted movement direction.
            // It would otherwise be hard to change direction due to little acceleration near max speed.
            float speedInMovementDirection = LinearVelocity.Dot(movementInput);

            // If we direct movement against velocity, this speed will be negative, allowing a greater acceleration than intended in not clamped.
            // This might provide snappy movement when changing direction. Worth testing.
            // (Even powers need factor of Mathf.Sign(x) or else they are never negative.)
            // If velocity in the input direction ends up greater than the max speed, by being pushed by an explosion for example,
            // acceleration would become negative and slow us down, which definitely is not what we want. Need to at least cap with a max value.
            // Acceleration curves can be seen here: https://www.desmos.com/calculator/7vwo7fisvi
            float speedFactor;
            float acceleration;
            switch (_movementType)
            {
                // Fancy curve (See above desmos link and doc comment for _movementType0Factor)
                case 0:
                    speedFactor = Mathf.Clamp(speedInMovementDirection / _maxSpeed, -1f + Mathf.Epsilon, 1f);

                    if (_movementType0Factor != 0f)
                    {
                        float inner = Mathf.Exp(2f * _movementType0Factor) - 1f;
                        float numerator = Mathf.Log(inner / (Mathf.Exp((speedFactor + 1f) * _movementType0Factor) - 1f));
                        float denominator = Mathf.Log(inner / (Mathf.Exp(_movementType0Factor) - 1f));
                        acceleration = _maxAcceleration * numerator / denominator;
                    }
                    else
                    {
                        acceleration = _maxAcceleration * (1f - float.Log2P1(speedFactor));
                    }

                    break;
                // Square root (Slow accel, faster changes in direction)
                case 1:
                    speedFactor = Mathf.Min(speedInMovementDirection / _maxSpeed, 1f);
                    acceleration = _maxAcceleration * (1f - Mathf.Sign(speedFactor) * Mathf.Sqrt(Mathf.Abs(speedFactor)));
                    break;
                // Linear
                case 2:
                    speedFactor = Mathf.Min(speedInMovementDirection / _maxSpeed, 1f);
                    acceleration = _maxAcceleration * (1f - speedFactor);
                    break;
                // Power of 2 (Fast accel, slower changes in direction)
                case 3:
                    speedFactor = Mathf.Min(speedInMovementDirection / _maxSpeed, 1f);
                    acceleration = _maxAcceleration * (1f - Mathf.Sign(speedFactor) * speedFactor * speedFactor);
                    break;
                default:
                    acceleration = 0f;
                    break;
            }

            force = Mass * acceleration * movementInput;
        }

        return force;
    }

    private Vector2 MovementInput()
    {
        Vector2 movementInput = new Vector2(Input.GetAxis(Action.Left, Action.Right), Input.GetAxis(Action.Up, Action.Down)).LimitLength();
        return _camera.Transform.BasisXform(movementInput);
    }
}
