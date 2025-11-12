using System;
using Godot;

public partial class Unit : Area2D
{
    [Export]
    protected UnitStats Stats { get; set; }
    [Export]
    protected Sprite2D _sprite { get; set; }

    [Export]
    protected Node2D _visuals { get; set; }

    [Export]
    protected AnimationPlayer _animationPlayer { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
