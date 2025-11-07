using Godot;
using System;

public partial class Mob : RigidBody2D
{
	public override void _Ready()
	{
		var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		var mobTypes = animatedSprite.SpriteFrames.GetAnimationNames();

		// Pick a random animation name
		var randomIndex = GD.Randi() % mobTypes.Length;
		animatedSprite.Animation = mobTypes[randomIndex];
		animatedSprite.Play();

		// Optional: connect signal programmatically
		GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D")
			.ScreenExited += _OnVisibleOnScreenNotifier2DScreenExited;
	}

	public override void _Process(double delta)
	{
		// No per-frame logic needed for now
	}

	private void _OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree();
	}
}
