using System;
using Godot;

public partial class Unit : Node2D
{
	[Export]
	private Sprite2D _sprite;

	[Export]
	private Node2D _visuals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print(_sprite.Name);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { }
}
