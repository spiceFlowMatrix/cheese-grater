using Godot;
using System;

public partial class Enemy : Unit
{
	[Export]
	public int FlockPush { get; set; } = 20;
	[Export]
	public Area2D VisionArea { get; set; }

	private bool canMove = true;

	private Vector2 moveDirection
	{
		get
		{
			if (!IsInstanceValid(Global.Instance.Player)) return Vector2.Zero;
			var direction = GlobalPosition.DirectionTo(Global.Instance.Player.GlobalPosition);

			foreach (Node2D neighbor in VisionArea.GetOverlappingAreas())
			{
				if (neighbor != this && neighbor.IsInsideTree())
				{
					var vector = GlobalPosition - neighbor.GlobalPosition;
					direction += FlockPush * vector.Normalized() / vector.Length();
				}
			}

			return direction;
		}
	}

	private bool canMoveTowardsPlayer
	{
		get
		{
			return IsInstanceValid(Global.Instance.Player) &&
	GlobalPosition.DistanceTo(Global.Instance.Player.GlobalPosition) > 60.0;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!canMove) return;
		if (!canMoveTowardsPlayer) return;

		Position += moveDirection * Stats.Speed * (float)delta;
	}

	private void UpdateRotation()
	{
		if (!IsInstanceValid(Global.Instance.Player)) return;

		var playerPos = Global.Instance.Player.GlobalPosition;
		var movingRight = GlobalPosition.X < playerPos.X;
		_visuals.Scale = movingRight ? new Vector2(-0.5f, 0.5f) : new Vector2(0.5f, 0.5f);
	}
}
