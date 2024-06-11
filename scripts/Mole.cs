using System;
using Godot;

public partial class Mole : Node2D
{
    const float DURATION = 3.0f;
    const float MINIMUM_ANGULAR_VELOCITY = 5.0f; // degrees per second
    private float _timer;
    private Area2D _area;
    private WhackAMoleController _controller;

    public override void _Ready()
    {
        _area = GetNode<Area2D>("Area2D");
        _controller = GetNode<WhackAMoleController>("../..");
    }

    public override void _PhysicsProcess(double delta)
    {
        _timer += (float)delta;
        Godot.Collections.Array<Node2D> bodies = _area.GetOverlappingBodies();
        if (bodies.Count > 0)
        {
            WhackAMolePlayer player = bodies[0] as WhackAMolePlayer;
            if (player.AngularVelocity > MINIMUM_ANGULAR_VELOCITY)
            {
                OnDelete(player);
            }
        }

        if (_timer > DURATION)
            QueueFree();
    }

    private void OnDelete(WhackAMolePlayer player)
    {
        QueueFree();
        _controller.Score(player.Id);
    }
}
