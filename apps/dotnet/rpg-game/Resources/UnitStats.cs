using Godot;
using System;

public enum UnitType
{
	PLAYER,
	ENEMY
}

public partial class UnitStats : Resource
{
	[Export]
	public string Name { get; set; }
	[Export]
	public UnitType Type { get; set; }
	[Export]
	public Texture2D Icon { get; set; }
	[Export]
	public int Health { get; set; } = 1;
	[Export]
	public double HealthIncreasePerWave { get; set; } = 1.0d;
	[Export]
	public int Damage { get; set; } = 1;
	[Export]
	public double DamageIncreasePerWave { get; set; } = 1.0d;
	[Export]
	public int Speed { get; set; } = 300;
	[Export]
	public double Luck { get; set; } = 1.0d;
	[Export]
	public double BlockChance { get; set; } = 0.0d;
	[Export]
	public int GoldDrop { get; set; } = 1;
}
