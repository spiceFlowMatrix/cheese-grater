using Godot;

public partial class Main : Node
{
	[Signal]
	public delegate void SrvTickEventHandler(Godot.Collections.Dictionary jsonMsg);

	[Export]
	public PackedScene MobScene { get; set; }

	private int _score;
	private WebSocketPeer _socket = new WebSocketPeer();
	private bool _handshakeComplete = false;

	public override async void _Ready()
	{
		var hud = GetNode<HUD>("HUD");
		hud.StartGame += NewGame;

		NewGame();
	}

	public override void _Process(double delta)
	{
	}

	public void GameOver()
	{
		GetNode<Timer>("ScoreTimer").Stop();
		GetNode<Timer>("MobTimer").Stop();

		GetNode<CanvasLayer>("HUD").Call("show_game_over");

		GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("DeathSound").Play();
	}

	public void NewGame()
	{
		_score = 0;
		GetNode<Node2D>("Player").Call("start", GetNode<Node2D>("StartPosition").Position);
		GetNode<Timer>("StartTimer").Start();

		GetNode("HUD").Call("update_score", _score);
		GetNode("HUD").Call("show_message", "Get Ready");

		GetTree().CallGroup("mobs", "queue_free");
		GetNode<AudioStreamPlayer>("Music").Play();
	}

	private void _OnMobTimerTimeout()
	{
		var mob = (RigidBody2D)MobScene.Instantiate();

		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();

		mob.Position = mobSpawnLocation.Position;
		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2f;
		direction += (float)GD.RandRange(-Mathf.Pi / 4f, Mathf.Pi / 4f);
		mob.Rotation = direction;

		Vector2 velocity = new Vector2((float)GD.RandRange(150.0f, 250.0f), 0.0f);
		mob.LinearVelocity = velocity.Rotated(direction);

		AddChild(mob);
	}

	private void _OnScoreTimerTimeout()
	{
		_score += 1;
		GetNode("HUD").Call("update_score", _score);
	}

	private void _OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}
}
