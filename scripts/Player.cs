using System;
using System.Collections.Generic;
using Godot;

public partial class Player : Node
{
    public readonly LevelManager.ControlType ControlType;
    public int Id
    {
        get { return _id; }
    }
    public Controls Controls
    {
        get { return _controls; }
    }
    public Color Color
    {
        get { return _color; }
    }

    private Controls _controls;
    private Color _color;
    private int _id;

    private Node2D _actor;

    public void Initialize(int id, LevelManager.ControlType controls, Color color, Node2D actor)
    {
        if (controls == LevelManager.ControlType.Keyboard)
            _controls = new KeyboardController();
        else
            throw new Exception("unimplemented");

        _color = color;
        _id = id;

        AttachActor(actor);
    }

    public void AttachActor(Node2D actor)
    {
        _actor = actor;
        IActor downcasted = actor as IActor;
        downcasted.Initialize(this);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
            _controls.Dispose();
    }
}

public abstract partial class Controls : IDisposable
{
    private readonly Dictionary<Axis, bool> _axisStates = new();

    public enum Axis
    {
        Up,
        Down,
        Left,
        Right,
        Action
    }

    public abstract float GetHorizontalAxis();
    public abstract float GetVerticalAxis();
    public abstract bool IsActionPressed();

    public bool IsAxisActive(Axis axis)
    {
        return axis switch
        {
            Axis.Up => GetVerticalAxis() < -0.5,
            Axis.Down => GetVerticalAxis() > 0.5,
            Axis.Left => GetHorizontalAxis() < -0.5,
            Axis.Right => GetHorizontalAxis() > 0.5,
            Axis.Action => IsActionPressed(),
            _ => throw new InvalidOperationException(),
        };
    }

    public bool WasJustTriggered(Axis axis)
    {
        bool lastState = _axisStates.ContainsKey(axis) && _axisStates[axis];
        bool currentState = IsAxisActive(axis);
        bool didTrigger = currentState && !lastState;
        _axisStates[axis] = currentState;
        return didTrigger;
    }

    public abstract void Dispose();
}

public interface IActor
{
    public void Initialize(Player player);
}
