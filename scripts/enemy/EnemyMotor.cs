using Godot;

namespace StarWreck.scripts.enemy;

[GlobalClass]
public partial class EnemyMotor : BaseMotor
{
	[Export(PropertyHint.Range, "0,1024,16,or_greater")]
	private float _force = 512f;

	[Export(PropertyHint.Range, "0,2048,32,or_greater")]
	private float _maxSpeed = 1024f;

	[Export(PropertyHint.Range, "0,2048,32,or_greater")]
	private float _desiredDistance = 1024f;

	[Export(PropertyHint.Range, "0,256,8,or_greater")]
	private float _friction = 128f;

	[Export(PropertyHint.Range, "0,256,8,or_greater")]
	private float _minFrictionSpeed = 128f;

	private player.Player _player;

	public override void _Ready()
	{
		base._Ready();
		_player = player.Player.Instance;
	}

	public override Vector2 GetMotorForce(RigidBody2D rigidBody2D)
	{
		Vector2 offsetToPlayer = _player.GlobalPosition - GlobalPosition;
		Vector2 dirToPlayer = offsetToPlayer.Normalized();
		Vector2 velocity = rigidBody2D.LinearVelocity;
		float speedInDirection = dirToPlayer.Dot(velocity);
		float forcePercent = (_maxSpeed - speedInDirection) / _maxSpeed;

		Vector2 motorForce = offsetToPlayer.Length() < _desiredDistance ? Vector2.Zero : dirToPlayer * forcePercent * _force;

		Vector2 frictionForce;
		if (velocity.Length() > _minFrictionSpeed)
		{
			Vector2 heading = velocity.Normalized();
			frictionForce = -_friction * heading;
			Vector2 negativeParallelComponent = frictionForce.Normalized() * Mathf.Min(0f, dirToPlayer.Dot(frictionForce));
			frictionForce += negativeParallelComponent;
		}
		else
		{
			frictionForce = Vector2.Zero;
		}

		return motorForce + frictionForce;
	}
}
