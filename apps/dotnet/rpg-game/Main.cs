using CheeseGrater.Core.Domain.Entities;
using CheeseGrater.Core.Domain.Enums;
using Godot;
using rpggame;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Main : Node
{
	private GameClient _gameClient;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameClient = new GameClient("http://localhost:7007");
		_ = TestEquipChange();
		//GD.Print("Hello, Godot with C# 13.0!");
	}

	private async Task TestEquipChange()
	{
		await _gameClient.NotifyEquipChangeAsync("player-1", "sword-iron");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { }
}
