using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Trail : Line2D
{
	[Export]
	public Player Player { get; set; }
	[Export]
	public int TrailLength { get; set; } = 25;
	[Export]
	public double TrailDuration { get; set; } = 1.0;
	[Export]
	public Timer TrailTimer { get; set; }

	private Array<Vector2> pointsArray = [];
	private bool isActive = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TrailTimer.Timeout += OnTrailTimerTimeout;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!isActive) return;

		pointsArray.Add(Player.GlobalPosition);
		if (pointsArray.Count > TrailLength)
		{
			pointsArray.RemoveAt(0);
		}

		Points = pointsArray.ToArray();
	}

	public void StartTrail()
	{
		isActive = true;
		ClearPoints();
		pointsArray.Clear();
		TrailTimer.Start(TrailDuration);
	}

	private void OnTrailTimerTimeout()
	{
		isActive = false;
		ClearPoints();
		pointsArray.Clear();
	}
}
