using Godot;
using System;

namespace BlackCatAdventure;

public partial class GameCamera : Camera2D
{
    public override void _Ready()
    {
        GlobalPosition = GetViewport().GetVisibleRect().GetCenter();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ZoomIn"))
        {
            GD.Print("ZoomIn");
            
            if (Zoom.X - 0.5f <= 0)
            {
                Zoom = new Vector2(0.1f, 0.1f);
            }
            else
            {
                Zoom = new Vector2(Zoom.X - 0.5f, Zoom.Y - 0.5f);
            }
        }

        if (Input.IsActionJustPressed("ZoomOut"))
        {
            GD.Print("ZoomOut");
            Zoom = new Vector2(Zoom.X + 0.5f, Zoom.Y + 0.5f);
        }
    }
}
