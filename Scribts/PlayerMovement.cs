using Godot;
using System;

public partial class PlayerMovement : CharacterBody3D
{
	[Export]
	public int Speed { get; set; } = 5;
	[Export]
	public int FallAcceleration { get; set; } = 75;
	public float CamSensitivity { get; set; } = 0.006f;

	private Vector3 _targetVelocity = Vector3.Zero;
	private Node3D _head;
	private Camera3D _cam;
	private Node3D _body;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_head = GetNode<Node3D>("Head");
		_cam = GetNode<Camera3D>("Head/Camera3D");
		_body = GetNode<Node3D>("Pivot");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion m)
		{
			_head.RotateY(-m.Relative.X * CamSensitivity);
			_cam.RotateX(-m.Relative.Y * CamSensitivity);
			

			Vector3 camRot = _cam.Rotation;
			camRot.X = Mathf.Clamp(camRot.X,
				Mathf.DegToRad(-80f), Mathf.DegToRad(80f));
			_cam.Rotation = camRot;
			_body.Rotation = camRot;
			_body.Rotation = new Vector3(0, _head.Rotation.Y, 0); 

		}

		else if (@event is InputEventKey k && k.Keycode == Key.Escape)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		base._Input(@event);
	}


	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		var direction = Vector3.Zero;
		var camera = GetNode<Node3D>("Head");

		if (Input.IsActionPressed("move_right"))
		{
			direction += camera.GlobalTransform.Basis.X;
		}
		if (Input.IsActionPressed("move_left"))
		{
			direction -= camera.GlobalTransform.Basis.X; 
		}
		if (Input.IsActionPressed("move_back"))
		{
			direction += camera.GlobalTransform.Basis.Z; 
		}
		if (Input.IsActionPressed("move_forward"))
		{
			direction -= camera.GlobalTransform.Basis.Z; 
		}

	

		_targetVelocity.X = direction.X * Speed;
		_targetVelocity.Z = direction.Z * Speed;

		if (!IsOnFloor())
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}

		Velocity = _targetVelocity;

		MoveAndSlide();
	}
}
