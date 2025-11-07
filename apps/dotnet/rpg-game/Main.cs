using CheeseGrater.Core.Domain.Entities;
using CheeseGrater.Core.Domain.Enums;
using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Hello, Godot with C# 13.0!");

		List<TodoItem> items = new()
		{
			new() { Title = "Buy groceries", Note = "Milk, Bread, Eggs", Priority = PriorityLevel.Medium, Done = false },
			new() { Title = "Buy groceries", Note = "Milk, Bread, Eggs", Priority = PriorityLevel.Medium, Done = false },
			new() { Title = "Buy groceries", Note = "Milk, Bread, Eggs", Priority = PriorityLevel.Medium, Done = false },
		};

		foreach (var item in items)
		{
			GD.Print($"- {item.Title} [{item.Priority}] - Done: {item.Done}");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
