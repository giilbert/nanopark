using System;
using System.Collections.Generic;
using Godot;

public partial class JoystickController : Controls, IDisposable
{
    public static readonly Stack<string> AvailableIds = new(new[] { "1", "2", "3", "4" });
    private const float DEAD_ZONE = 50f;
    private readonly string _id;

    public JoystickController()
    {
        _id = AvailableIds.Pop();
    }

    public override float GetHorizontalAxis()
    {
        float value = Glue.State[_id].x;
        if (Math.Abs(value) < DEAD_ZONE)
            return 0f;
        return value / 128f;
    }

    public override float GetVerticalAxis()
    {
        float value = -Glue.State[_id].y;
        if (Math.Abs(value) < DEAD_ZONE)
            return 0f;
        return value / 128f;
    }

    public override bool IsActionPressed()
    {
        return Glue.State[_id].is_pressed;
    }

    public override void Dispose()
    {
        AvailableIds.Push(_id);
        GC.SuppressFinalize(this);
    }
}
