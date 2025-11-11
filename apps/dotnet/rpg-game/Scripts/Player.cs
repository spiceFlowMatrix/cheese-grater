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
	[Export]
	public CollisionShape2D Collision { get; set; }

	private Vector2 _moveDir { get; set; }
	private bool _isDashing { get; set; } = false;
	private bool _dashAvailable { get; set; } = true;

	public override void _Ready()
	{
		DashTimer.WaitTime = DashDuration;
		DashCooldownTimer.WaitTime = DashCooldown;

		DashTimer.Timeout += OnDashTimerTimeout;
		DashCooldownTimer.Timeout += OnDashCooldownTimerTimeout;
	}

	public override void _Process(double delta)
	{
		_moveDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		var currentVelocity = _moveDir * Stats.Speed;

		if (_isDashing) currentVelocity *= (float)DashSpeedMultiplier;

		Position += currentVelocity * (float)delta;

		if (canDash())
		{
			startDash();
		}

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

	private void startDash()
	{
		if (!_dashAvailable)
			return;
		_isDashing = true;
		_dashAvailable = false;
		Collision.SetDeferred("disabled", true);
		_visuals.Modulate = new Color(1, 1, 1, 0.5f);
		DashTimer.Start();
	}

	private bool canDash()
	{
		return !_isDashing && DashCooldownTimer.IsStopped() && Input.IsActionJustPressed("dash") && _moveDir != Vector2.Zero;
	}

	private void OnDashTimerTimeout()
	{
		_isDashing = false;
		_visuals.Modulate = new Color(1, 1, 1, 1);
		_moveDir = Vector2.Zero;
		Collision.SetDeferred("disabled", false);
		DashCooldownTimer.Start();
	}

	private void OnDashCooldownTimerTimeout()
	{
		_dashAvailable = true;
	}
}
