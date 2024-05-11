using System;
using Godot;

public partial class IceHockeyPlayer : RigidBody2D, IActor
{
    const float ACCELERATION = 250.0f;
    const float MAX_SPEED = 800.0f;
    const float LAUNCH_SPEED = 500.0f;

    public Vector2 Heading { get; private set; }
    public HockeyPuck Puck { get; set; }

    private Controls _controls;
    private AnimationPlayer _animationPlayer;
    private ShaderMaterial _materialClone;
    private AnimatedSprite2D _sprite;
    private Sprite2D _hockeyStickSprite;

    public void Initialize(Player player)
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _hockeyStickSprite = GetNode<Sprite2D>("Hockey Stick");
        _controls = player.Controls;

        _materialClone = _sprite.Material.Duplicate() as ShaderMaterial;
        _materialClone.SetShaderParameter("color", player.Color);
        _sprite.Material = _materialClone;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = new Vector2(
            _controls.GetHorizontalAxis(),
            _controls.GetVerticalAxis()
        ).Normalized();

        if (direction.X != 0 && direction.Y == 0)
        {
            _sprite.Play("side-walk");
            _sprite.FlipH = direction.X < 0;
        }
        else if (direction.Y < 0)
            _sprite.Play("back-walk");
        else if (direction.Y > 0)
            _sprite.Play("front-walk");
        else
            _sprite.Play("idle");

        if (direction != Vector2.Zero)
            Heading = direction;

        if (_controls.WasJustTriggered(Controls.Axis.Action))
        {
            _animationPlayer.Play("Hockey Stick Spin Fast");
            Puck?.Launch(Heading * LAUNCH_SPEED + LinearVelocity);
        }
        else if (Puck != null)
        {
            _animationPlayer.Play("Hockey Stick Spin");
        }

        _hockeyStickSprite.FlipH = Heading.X < 0 && Puck == null;
        _hockeyStickSprite.Rotation = Heading.Angle();

        if (LinearVelocity.Length() < MAX_SPEED)
            ApplyForce(direction * ACCELERATION * Mass);
    }
}
