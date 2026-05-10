using Godot;
using System;

public partial class GameCamera : Camera2D
{
    public override void _Ready()
    {
        GlobalPosition = GetViewport().GetVisibleRect().GetCenter();
    }
}
