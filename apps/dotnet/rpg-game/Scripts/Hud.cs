using Godot;
using System;
using System.Threading.Tasks;

public partial class HUD : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();

	public override void _Ready()
	{
		// Optional: connect signals automatically
		GetNode<Button>("StartButton").Pressed += _OnStartButtonPressed;
		GetNode<Timer>("MessageTimer").Timeout += _OnMessageTimerTimeout;
	}

	public override void _Process(double delta)
	{
		// No per-frame logic required
	}

	public void ShowMessage(string text)
	{
		var messageLabel = GetNode<Label>("Message");
		messageLabel.Text = text;
		messageLabel.Show();
		GetNode<Timer>("MessageTimer").Start();
	}

	public async void ShowGameOver()
	{
		ShowMessage("Game Over");

		// Wait for message timer timeout
		await ToSignal(GetNode<Timer>("MessageTimer"), Timer.SignalName.Timeout);

		var messageLabel = GetNode<Label>("Message");
		messageLabel.Text = "Dodge the Creeps!";
		messageLabel.Show();

		// Wait one second before showing Start button
		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		GetNode<Button>("StartButton").Show();
	}

	public void UpdateScore(int score)
	{
		GetNode<Label>("ScoreLabel").Text = score.ToString();
	}

	private void _OnStartButtonPressed()
	{
		GetNode<Button>("StartButton").Hide();
		EmitSignal(SignalName.StartGame);
	}

	private void _OnMessageTimerTimeout()
	{
		GetNode<Label>("Message").Hide();
	}
}
