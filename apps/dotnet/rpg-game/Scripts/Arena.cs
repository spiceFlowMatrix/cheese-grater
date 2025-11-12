using Godot;
using System;

public partial class Arena : Node2D
{
	[Export]
	public Player Player { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Global.Instance.Player = Player;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
