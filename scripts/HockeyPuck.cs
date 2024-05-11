using System;
using Godot;

public partial class HockeyPuck : RigidBody2D
{
    public IceHockeyPlayer PuckOwner;
    public static HockeyPuck Instance { get; private set; }

    private Area2D _pickUpArea;
    private CollisionShape2D _collider;
    private bool _canPickUp = true;

    public override void _Ready()
    {
        _collider = GetNode<CollisionShape2D>("CollisionShape2D");
        _pickUpArea = GetNode<Area2D>("PickUpArea");
        Camera camera = GetParent().GetNode<Camera>("Camera");
        camera.AddPointOfInterest(this);
        Instance = this;
    }

    public override void _PhysicsProcess(double delta)
    {
        _collider.Disabled = PuckOwner != null;
        if (PuckOwner != null)
        {
            Vector2 targetPosition = PuckOwner.GlobalPosition + PuckOwner.Heading * 50;

            Vector2 difference = targetPosition - GlobalPosition;

            if (difference.Length() < 20)
                return;

            LinearVelocity =
                GlobalPosition.DirectionTo(targetPosition) * 100 + PuckOwner.LinearVelocity;
        }
        else if (_canPickUp)
        {
            foreach (Node2D body in _pickUpArea.GetOverlappingBodies())
            {
                if (body is IceHockeyPlayer)
                {
                    PuckOwner = body as IceHockeyPlayer;
                    PuckOwner.Puck = this;
                }
            }
        }
    }

    public async void Launch(Vector2 velocity)
    {
        LinearVelocity = velocity;
        PuckOwner.Puck = null;
        PuckOwner = null;

        _canPickUp = false;
        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
        _canPickUp = true;
    }
}
