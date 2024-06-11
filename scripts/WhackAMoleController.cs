using System;
using System.Collections.Generic;
using Godot;

public partial class WhackAMoleController : Node
{
    private PackedScene _mole;
    private float _timer;
    private readonly Dictionary<int, int> _scores = new Dictionary<int, int>();

    [Export]
    public Label ScoreLabel;

    public void SpawnMole()
    {
        var random = GD.RandRange(0, 14);
        var spawnedMole = _mole.Instantiate<Node2D>();
        GetChild<Node2D>(random).AddChild(spawnedMole);
    }

    public override void _Ready()
    {
        _mole = GD.Load<PackedScene>("res://templates/mole.tscn");
    }

    public override void _PhysicsProcess(double delta)
    {
        _timer += (float)delta;
        if (_timer > 1.0f)
        {
            _timer = 0.0f;
            SpawnMole();
        }
    }

    private void UpdateLabel()
    {
        string text = "Scores: ";
        foreach (var score in _scores)
            text += $"#{score.Key} = {score.Value}  ";
        ScoreLabel.Text = text;
    }

    public void Score(int playerId)
    {
        _scores.TryGetValue(playerId, out int score);
        _scores[playerId] = score + 1;
        UpdateLabel();
    }
}
