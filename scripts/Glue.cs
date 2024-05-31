using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

public partial class Glue : GodotObject
{
    public static Dictionary<string, Controller> State { get; private set; }
    private static GodotObject _singleton = Engine.GetSingleton("ControllerInput");

    public static string[] ListPorts()
    {
        return _singleton.Call("list_ports").AsStringArray();
    }

    public static void Connect(string port)
    {
        _singleton.Call("connect", port);
    }

    public static void Poll()
    {
        _singleton.Call("poll");
    }

    public static void UpdateControllersState()
    {
        string result = _singleton.Call("get_controllers_state").AsString();
        State = JsonSerializer.Deserialize<Dictionary<string, Controller>>(result);
    }
}

public struct Controller
{
    [JsonInclude]
    public int id;

    [JsonInclude]
    public int x;

    [JsonInclude]
    public int y;

    [JsonInclude]
    public bool is_pressed;
}
