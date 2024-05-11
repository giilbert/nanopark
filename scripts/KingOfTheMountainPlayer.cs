using System;
using Godot;

public partial class KingOfTheMountainPlayer : CharacterBody2D, IActor
{
    private const float SPEED = 300;
    private const float JUMP_VELOCITY = -450;
    private readonly float G = ProjectSettings.GetSetting("physics/2d/default_gravity").As<float>();

    private Controls _controls;
    private ShaderMaterial _materialClone;
    private GpuParticles2D _particles;
    private AnimatedSprite2D _sprite;

    private bool _canDoubleJump = true;

    public void Initialize(Player player)
    {
        _sprite = GetNode("AnimatedSprite2D") as AnimatedSprite2D;
        _particles = GetNode("GPUParticles2D") as GpuParticles2D;
        _controls = player.Controls;

        _materialClone = _sprite.Material.Duplicate() as ShaderMaterial;
        _sprite.Material = _materialClone;
        _materialClone.SetShaderParameter("color", player.Color);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 nextVelocity = Velocity;

        if (!IsOnFloor())
        {
            nextVelocity.Y += G * (float)delta;

            if (_controls.WasJustTriggered(Controls.Axis.Up) && _canDoubleJump)
            {
                _particles.Emitting = true;
                nextVelocity.Y = JUMP_VELOCITY * 1.1f;
                _canDoubleJump = false;
            }
        }
        else
        {
            _canDoubleJump = true;
            if (_controls.WasJustTriggered(Controls.Axis.Up))
                nextVelocity.Y = JUMP_VELOCITY;
        }

        float direction = _controls.GetHorizontalAxis();
        if (direction != 0)
        {
            nextVelocity.X = direction * SPEED;
            _sprite.FlipH = direction < 0;
            _sprite.Animation = "walk";
        }
        else
        {
            _sprite.Animation = "idle";
            nextVelocity.X = Mathf.MoveToward(Velocity.X, 0, SPEED);
        }

        Velocity = nextVelocity;

        MoveAndSlide();
    }
}
