using System;
using Godot;

public partial class Player : Unit
{
    [ExportGroup("Dash Settings")]
    [Export]
    public double DashDuration { get; set; } = 0.5;
    [Export]
    public double DashSpeedMultiplier { get; set; } = 2.7;
    [Export]
    public double DashCooldown { get; set; } = 1.5;
    [Export]
    public Timer DashTimer { get; set; }
    [Export]
    public Timer DashCooldownTimer { get; set; }

	private Vector2 _moveDir { get; set; }
    private bool _isDashing { get; set; } = false;

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
