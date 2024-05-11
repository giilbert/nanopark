using System;
using Godot;

public partial class MainMenu : Node2D
{
    private Label _keyboardPlayerCountLabel;
    private Button _addKeyboardPlayerButton;
    private Button _removeKeyboardPlayerButton;

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
    }

    private void UpdateUI()
    {
        LevelManager levelManager = GetTree().Root.GetNode<LevelManager>("LevelManager");
        int numberOfKeyboardPlayers = levelManager.GetNumberOfPlayers(
            LevelManager.ControlType.Keyboard
        );

        _keyboardPlayerCountLabel.Text =
            numberOfKeyboardPlayers
            + " keyboard player"
            + (numberOfKeyboardPlayers != 1 ? "s" : "");
        _addKeyboardPlayerButton.Disabled = numberOfKeyboardPlayers == 3;
        _removeKeyboardPlayerButton.Disabled = numberOfKeyboardPlayers == 0;
    }

    public void OnAddKeyboardPlayer()
    {
        LevelManager.Instance.MainMenuAddPlayer(LevelManager.ControlType.Keyboard);
        UpdateUI();
    }

    public void OnRemoveKeyboardPlayer()
    {
        LevelManager.Instance.MainMenuRemoveLastKeyboardPlayer();
        UpdateUI();
    }

    public static void LoadLevel(string levelId)
    {
        LevelManager.Instance.LoadLevel(levelId);
    }
}
