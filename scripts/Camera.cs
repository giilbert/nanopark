using System;
using System.Collections.Generic;
using Godot;

public partial class Camera : Camera2D
{
    private readonly List<Node2D> _pointsOfInterest = new();

    public void AddPointOfInterest(Node2D Node)
    {
        _pointsOfInterest.Add(Node);
    }

    public void RemovePointOfInterest(Node2D Node)
    {
        _pointsOfInterest.Remove(Node);
    }

    public override void _PhysicsProcess(double delta)
    {
        float topLeftX = float.MaxValue;
        float topLeftY = float.MaxValue;

        float bottomRightX = float.MinValue;
        float bottomRightY = float.MinValue;

        foreach (Node2D pointOfInterest in _pointsOfInterest)
        {
            float px = pointOfInterest.GlobalPosition.X;
            float py = pointOfInterest.GlobalPosition.Y;

            topLeftX = Math.Min(topLeftX, px);
            topLeftY = Math.Min(topLeftY, py);
            bottomRightX = Math.Max(bottomRightX, px);
            bottomRightY = Math.Max(bottomRightY, py);
        }

        Vector2 topLeft = new(topLeftX, topLeftY);
        Vector2 bottomRight = new(bottomRightX, bottomRightY);

        Vector2 center = (topLeft + bottomRight) / 2;

        Vector2 size = bottomRight - topLeft;
        Vector2 zoomUnclamped = size / GetViewportRect().Size;
        float zoomAmount =
            1 / Mathf.Clamp(Mathf.Max(zoomUnclamped.X, zoomUnclamped.Y) + 0.4f, 0.5f, 6.0f);

        Zoom = Zoom.Lerp(new Vector2(zoomAmount, zoomAmount), 0.05f);
        Position = Position.Lerp(center, 0.1f);
    }
}
