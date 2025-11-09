using System;
using Godot;

public partial class Player : Unit
{
	private Vector2 _moveDir { get; set; }

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		_moveDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		var currentVelocity = _moveDir * 500;

		Position += currentVelocity * (float)delta;

		updateAnimations();
		updateRotation();
	}

	private void updateAnimations()
	{
		if (_moveDir.Length() > 0)
		{
			_animationPlayer.Play("move");
		}
		else
		{
			_animationPlayer.Play("idle");
		}
	}

	private void updateRotation()
	{
		if (_moveDir == Vector2.Zero)
			return;

		if (_moveDir.X >= 0.1)
		{
			_visuals.Scale = new Vector2(-0.5f, 0.5f);
		}
		else
		{
			_visuals.Scale = new Vector2(0.5f, 0.5f);
		}
	}
}
