using Godot;
using System;

namespace BlackCatAdventure;

public partial class GameHud : Control
{
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Close"))
        {
            GD.Print("Open game menu");
        }
    }
}
