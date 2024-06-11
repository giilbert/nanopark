using System;
using Godot;

public partial class WhackAMolePlayer : RigidBody2D, IActor
{
    const float ACCELERATION = 200.0f;

    public bool IsHitting { get; private set; }
    public int Id { get; private set; }

    private Controls _controls;
    private ShaderMaterial _materialClone;
    private Sprite2D _sprite;

    public void Initialize(Player player)
    {
        _sprite = GetNode<Sprite2D>("Sprite");
        _controls = player.Controls;
        Id = player.Id;

        _materialClone = _sprite.Material.Duplicate() as ShaderMaterial;
        _materialClone.SetShaderParameter("color", player.Color);
        _sprite.Material = _materialClone;
    }

    public override void _PhysicsProcess(double delta)
    {
        ApplyCentralForce(
            new Vector2(_controls.GetHorizontalAxis(), _controls.GetVerticalAxis())
                * Mass
                * ACCELERATION
        );

        if (_controls.IsActionPressed())
        {
            if (!IsHitting)
            {
                ApplyTorque(Inertia * 800.0f);
                IsHitting = true;
                ResetHitCooldown();
            }
        }
    }

    public async void ResetHitCooldown()
    {
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
        IsHitting = false;
    }
}
