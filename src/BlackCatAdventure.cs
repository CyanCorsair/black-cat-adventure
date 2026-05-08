using Godot;
using System;

namespace BlackCatAdventure;

public partial class BlackCatAdventure : Node
{
    public override void _Ready()
    {
        PackedScene mainMenuScene = GD.Load<PackedScene>("res://scenes/main_menu.tscn");
        MainMenu sceneInstance = mainMenuScene.Instantiate<MainMenu>();
        AddChild(sceneInstance);
    }
}
