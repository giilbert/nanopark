using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class LevelManager : Node
{
    private readonly PackedScene _playerTemplate = GD.Load<PackedScene>(
        "res://templates/player.tscn"
    );
    private readonly PackedScene _platformerPlayerTemplate = GD.Load<PackedScene>(
        "res://templates/king-of-the-mountain-player.tscn"
    );

    public enum ControlType
    {
        Keyboard,
        Joystick
    }

    public static LevelManager Instance { get; private set; }

    private string _currentLevelId = "main-menu";
    private readonly Stack<Color> _availableColors =
        new(
            new[]
            {
                Color.FromHtml("#ec7064"),
                Color.FromHtml("#28b463"),
                Color.FromHtml("#3498db"),
                Color.FromHtml("#f39c12"),
                Color.FromHtml("#8e44ad"),
                Color.FromHtml("#f1c40f"),
            }
        );
    private int _currentPlayerId = 0;

    private readonly Dictionary<int, Player> _players = new();

    public int GetNumberOfPlayers(ControlType? ofControlType)
    {
        if (ofControlType == null)
            return _players.Count;

        return _players.Values.Count(player => player.ControlType == ofControlType);
    }

    public void MainMenuAddPlayer(ControlType controlType)
    {
        if (_currentLevelId != "main-menu")
            throw new InvalidOperationException("not in main menu");

        Node2D mainMenuScene = GetTree().CurrentScene as Node2D;

        int id = _currentPlayerId += 1;
        Color nextColor = _availableColors.Pop();
        Player player = _playerTemplate.Instantiate() as Player;

        Node2D actor = _platformerPlayerTemplate.Instantiate() as Node2D;
        actor.Name = "Player " + id;
        player.Initialize(id, controlType, nextColor, actor);
        mainMenuScene.AddChild(player);
        player.AddChild(actor);

        _players[id] = player;
    }

    public void MainMenuRemoveLastPlayerOfType(ControlType type)
    {
        if (_currentLevelId != "main-menu")
            throw new InvalidOperationException("not in main menu");

        Player player = _players.Values.Where(p => p.ControlType == type).OrderBy(p => p.Id).Last();
        Player removed = _players[player.Id];
        _availableColors.Push(removed.Color);
        _players.Remove(player.Id);
        removed.QueueFree();
    }

    public static Vector2[] GetSpawnPoints(Node2D scene)
    {
        return scene
            .FindChild("Spawn Points")
            .GetChildren()
            .Select(node => (node as Node2D).Position)
            .ToArray();
    }

    public void LoadLevel(string id)
    {
        Node2D levelScene =
            GD.Load<PackedScene>("res://scenes/levels/" + id + ".tscn").Instantiate() as Node2D;
        GetTree().Root.AddChild(levelScene);

        Camera camera = levelScene.FindChild("Camera") as Camera;

        Node2D currentScene = GetTree().CurrentScene as Node2D;
        currentScene.QueueFree();

        // Spawn the players again
        Vector2[] spawnPoints = levelScene
            .FindChild("Spawn Points")
            .GetChildren()
            .Select(node => (node as Node2D).Position)
            .ToArray();

        Resource templateResource = levelScene.GetMeta("player_type").As<Resource>();
        PackedScene template = GD.Load<PackedScene>(templateResource.ResourcePath);

        int spawnPointIndex = 0;

        foreach (Player player in _players.Values)
        {
            Node2D actor = template.Instantiate() as Node2D;
            actor.Position = spawnPoints[spawnPointIndex];
            spawnPointIndex = (spawnPointIndex + 1) % spawnPoints.Length;
            player.AttachActor(actor);
            levelScene.AddChild(actor);
            camera?.AddPointOfInterest(actor);
        }
    }

    public override void _Ready()
    {
        CallDeferred(nameof(DeferredReady));
        Instance = this;
    }

    public override void _Process(double _dt)
    {
        Glue.Poll();
        Glue.UpdateControllersState();
    }

    public void DeferredReady()
    {
        string[] ports = Glue.ListPorts();

        foreach (string point in ports)
        {
            GD.Print("Available port: " + point);
        }

        Glue.Connect("/dev/ttyACM0");

        // MainMenuAddPlayer(ControlType.Keyboard);
        // MainMenuAddPlayer(ControlType.Keyboard);
        // MainMenuAddPlayer(ControlType.Keyboard);
        // LoadLevel("ice-hockey");
    }
}
