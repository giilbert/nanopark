using System;
using Godot;

public partial class MainMenu : Node2D
{
    private Label _keyboardPlayerCountLabel;
    private Button _addKeyboardPlayerButton;
    private Button _removeKeyboardPlayerButton;

    private Label _joystickPlayerCountLabel;
    private Button _addJoystickPlayerButton;
    private Button _removeJoystickPlayerButton;

    public override void _Ready()
    {
        _keyboardPlayerCountLabel = GetNode<Label>(
            "UI/CenterContainer/VBoxContainer/Keyboard Players/Label"
        );
        _addKeyboardPlayerButton = GetNode<Button>(
            "UI/CenterContainer/VBoxContainer/Keyboard Players/Add"
        );
        _removeKeyboardPlayerButton = GetNode<Button>(
            "UI/CenterContainer/VBoxContainer/Keyboard Players/Remove"
        );

        _joystickPlayerCountLabel = GetNode<Label>(
            "UI/CenterContainer/VBoxContainer/Joystick Players/Label"
        );
        _addJoystickPlayerButton = GetNode<Button>(
            "UI/CenterContainer/VBoxContainer/Joystick Players/Add"
        );
        _removeJoystickPlayerButton = GetNode<Button>(
            "UI/CenterContainer/VBoxContainer/Joystick Players/Remove"
        );
    }

    private void UpdateUI()
    {
        LevelManager levelManager = GetTree().Root.GetNode<LevelManager>("LevelManager");
        int numberOfKeyboardPlayers = levelManager.GetNumberOfPlayers(
            LevelManager.ControlType.Keyboard
        );
        int numberOfJoystickPlayers = levelManager.GetNumberOfPlayers(
            LevelManager.ControlType.Joystick
        );
        GD.Print(LevelManager.ControlType.Keyboard == LevelManager.ControlType.Joystick);

        _keyboardPlayerCountLabel.Text =
            numberOfKeyboardPlayers
            + " keyboard player"
            + (numberOfKeyboardPlayers != 1 ? "s" : "");
        _addKeyboardPlayerButton.Disabled = numberOfKeyboardPlayers == 3;
        _removeKeyboardPlayerButton.Disabled = numberOfKeyboardPlayers == 0;

        _joystickPlayerCountLabel.Text =
            numberOfJoystickPlayers
            + " joystick player"
            + (numberOfJoystickPlayers != 1 ? "s" : "");
        _addJoystickPlayerButton.Disabled = JoystickController.AvailableIds.Count == 0;
        _removeJoystickPlayerButton.Disabled = numberOfJoystickPlayers == 0;
    }

    public void OnAddKeyboardPlayer()
    {
        LevelManager.Instance.MainMenuAddPlayer(LevelManager.ControlType.Keyboard);
        UpdateUI();
    }

    public void OnRemoveKeyboardPlayer()
    {
        LevelManager.Instance.MainMenuRemoveLastPlayerOfType(LevelManager.ControlType.Keyboard);
        UpdateUI();
    }

    public void OnAddJoystickPlayer()
    {
        LevelManager.Instance.MainMenuAddPlayer(LevelManager.ControlType.Joystick);
        UpdateUI();
    }

    public void OnRemoveJoystickPlayer()
    {
        LevelManager.Instance.MainMenuRemoveLastPlayerOfType(LevelManager.ControlType.Joystick);
        UpdateUI();
    }

    public static void LoadLevel(string levelId)
    {
        LevelManager.Instance.LoadLevel(levelId);
    }
}
