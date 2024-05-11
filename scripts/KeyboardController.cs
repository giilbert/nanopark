using System;
using System.Collections.Generic;
using Godot;

public partial class KeyboardController : Controls, IDisposable
{
    private static readonly Stack<int> _availableIds = new(new[] { 3, 2, 1 });

    private readonly int _id;
    private readonly string _leftName;
    private readonly string _rightName;
    private readonly string _upName;
    private readonly string _downName;
    private readonly string _actionName;

    public KeyboardController()
    {
        int keyboardControlId = _availableIds.Pop();
        _id = keyboardControlId;
        _leftName = "left-" + keyboardControlId;
        _rightName = "right-" + keyboardControlId;
        _upName = "up-" + keyboardControlId;
        _downName = "down-" + keyboardControlId;
        _actionName = "action-" + keyboardControlId;
    }

    public override float GetHorizontalAxis()
    {
        return Input.GetAxis(_leftName, _rightName);
    }

    public override float GetVerticalAxis()
    {
        return Input.GetAxis(_upName, _downName);
    }

    public override bool IsActionPressed()
    {
        return Input.IsActionPressed(_actionName);
    }

    public override void Dispose()
    {
        _availableIds.Push(_id);
        GC.SuppressFinalize(this);
    }
}
