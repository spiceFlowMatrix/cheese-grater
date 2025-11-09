using Godot;
using System;

public partial class Unit : Node2D
{
	private Sprite2D _sprite;
	private Node2D _visuals;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Visuals/Sprite");
		_visuals = GetNode<Node2D>("Visuals");

		GD.Print(_sprite.Name);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
