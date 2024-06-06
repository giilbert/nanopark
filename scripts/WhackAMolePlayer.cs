using System;
using Godot;

public partial class WhackAMolePlayer : RigidBody2D, IActor
{
    const float ACCELERATION = 200.0f;

    private Controls _controls;
    private ShaderMaterial _materialClone;
    private Sprite2D _sprite;

    public void Initialize(Player player)
    {
        _sprite = GetNode<Sprite2D>("Sprite");
        _controls = player.Controls;

        _materialClone = _sprite.Material.Duplicate() as ShaderMaterial;
        _materialClone.SetShaderParameter("color", player.Color);
        _sprite.Material = _materialClone;
    }

    public override void _PhysicsProcess(double delta)
    {
        ApplyForce(
            new Vector2(_controls.GetHorizontalAxis(), _controls.GetVerticalAxis())
                * Mass
                * ACCELERATION
        );
    }
}
