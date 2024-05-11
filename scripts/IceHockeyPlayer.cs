using System;
using Godot;

public partial class IceHockeyPlayer : RigidBody2D, IActor
{
    const float ACCELERATION = 250.0f;
    const float MAX_SPEED = 800.0f;

    private Controls _controls;
    private ShaderMaterial _materialClone;

    public void Initialize(Player player)
    {
        AnimatedSprite2D sprite = GetNode("AnimatedSprite2D") as AnimatedSprite2D;
        _controls = player.Controls;
        _materialClone = sprite.Material.Duplicate() as ShaderMaterial;
        _materialClone.SetShaderParameter("color", player.Color);
        sprite.Material = _materialClone;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = new(_controls.GetHorizontalAxis(), _controls.GetVerticalAxis());
        ApplyForce(direction * ACCELERATION * Mass);
    }
}
